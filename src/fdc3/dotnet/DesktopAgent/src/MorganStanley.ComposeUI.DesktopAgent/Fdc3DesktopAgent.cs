﻿/*
 * Morgan Stanley makes this available to you under the Apache License,
 * Version 2.0 (the "License"). You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0.
 *
 * See the NOTICE file distributed with this work for additional information
 * regarding copyright ownership. Unless required by applicable law or agreed
 * to in writing, software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions
 * and limitations under the License.
 */

using System.Collections.Concurrent;
using System.Reactive.Linq;
using Finos.Fdc3;
using Finos.Fdc3.AppDirectory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Channels;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.DependencyInjection;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Exceptions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.ModuleLoader;
using AppChannel = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Channels.AppChannel;
using AppIdentifier = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.AppIdentifier;
using AppIntent = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.AppIntent;
using AppMetadata = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.AppMetadata;
using Constants = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Infrastructure.Internal.Constants;
using ContextMetadata = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.ContextMetadata;
using Icon = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.Icon;
using ImplementationMetadata = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.ImplementationMetadata;
using Screenshot = MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol.Screenshot;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent;

internal class Fdc3DesktopAgent : IFdc3DesktopAgentBridge
{
    private readonly ILogger<Fdc3DesktopAgent> _logger;
    private readonly IResolverUICommunicator _resolverUI;
    private readonly IChannelSelector _channelSelector;
    private readonly IUserChannelSetReader _userChannelSetReader;
    private readonly ConcurrentDictionary<string, UserChannel> _userChannels = new();
    private readonly ConcurrentDictionary<string, PrivateChannel> _privateChannels = new();
    private readonly ConcurrentDictionary<string, AppChannel> _appChannels = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly Fdc3DesktopAgentOptions _options;
    private readonly IAppDirectory _appDirectory;
    private readonly IModuleLoader _moduleLoader;
    private readonly ConcurrentDictionary<Guid, Fdc3App> _runningModules = new();
    private readonly ConcurrentDictionary<Guid, RaisedIntentRequestHandler> _raisedIntentResolutions = new();
    private readonly ConcurrentDictionary<StartRequest, TaskCompletionSource<IModuleInstance>> _pendingStartRequests = new();
    private readonly ConcurrentDictionary<Guid, List<ContextListener>> _contextListeners = new();
    private readonly ConcurrentDictionary<Guid, string> _openedAppContexts = new();
    private IAsyncDisposable? _subscription;
    private readonly object _contextListenerLock = new();
    private readonly IntentResolver _intentResolver;

    public Fdc3DesktopAgent(
        IAppDirectory appDirectory,
        IModuleLoader moduleLoader,
        IOptions<Fdc3DesktopAgentOptions> options,
        IResolverUICommunicator resolverUI,
        IChannelSelector channelSelector,
        IUserChannelSetReader userChannelSetReader,
        ILoggerFactory? loggerFactory = null)
    {
        _appDirectory = appDirectory;
        _moduleLoader = moduleLoader;
        _options = options.Value;
        _resolverUI = resolverUI;
        _channelSelector = channelSelector;
        _userChannelSetReader = userChannelSetReader;
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<Fdc3DesktopAgent>() ?? NullLogger<Fdc3DesktopAgent>.Instance;

        _intentResolver = new(appDirectory, _runningModules);
    }

    public async ValueTask<UserChannel?> AddUserChannel(Func<string, UserChannel> addUserChannelFactory, string channelId)
    {
        ChannelItem? channelItem = null;
        var userChannelSet = await _userChannelSetReader.GetUserChannelSet();

        if (!userChannelSet.TryGetValue(channelId, out channelItem) || channelItem == null)
        {
            return null;
        }

        //Checking if the endpoint is already registered, because it can cause issues with multiple threads executing this same task to register the appropriate services storing the latest context messages, etc on the Channel objects.
        if (_userChannels.TryGetValue(channelId, out var userChannel))
        {
            return userChannel;
        }

        userChannel = _userChannels.GetOrAdd(channelId, addUserChannelFactory(channelId));

        try
        {
            await userChannel!.Connect();
            return userChannel;
        }
        catch (MessageRouterDuplicateEndpointException exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(exception, $"{channelId} is already registered as service endpoint.");
            }

            return userChannel;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception thrown while executing {nameof(AddUserChannel)}.");
            _userChannels.TryRemove(channelId, out _);
            throw;
        }
    }

    public async ValueTask AddPrivateChannel(Func<string, PrivateChannel> addPrivateChannelFactory, string privateChannelId)
    {
        //Checking if the endpoint is already registered, because it can cause issues while registering services storing the latest context messages, etc on the Channel objects.
        if (_privateChannels.TryGetValue(privateChannelId, out var privateChannel))
        {
            return;
        }

        privateChannel = _privateChannels.GetOrAdd(privateChannelId, addPrivateChannelFactory(privateChannelId));

        try
        {
            await privateChannel!.Connect();
        }
        catch (MessageRouterDuplicateEndpointException exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(exception, $"{privateChannelId} is already registered as service endpoint.");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception thrown while executing {nameof(AddPrivateChannel)}.");
            _privateChannels.TryRemove(privateChannelId, out _);
            throw;
        }
    }

    public async ValueTask<CreateAppChannelResponse> AddAppChannel(Func<string, AppChannel> addAppChannelFactory, CreateAppChannelRequest request)
    {
        if (!_runningModules.TryGetValue(new Guid(request.InstanceId), out _))
        {
            return CreateAppChannelResponse.Failed(ChannelError.CreationFailed);
        }

        //Conformance tests are expecting to reject the getOrCreateChannel call when we try to create an app channel with an id of a private channel
        //however we do have separate topics for channeltypes
        if (_privateChannels.TryGetValue(request.ChannelId, out _))
        {
            return CreateAppChannelResponse.Failed(ChannelError.AccessDenied);
        }

        //Checking if the endpoint is already registered, because it can cause issues while registering services storing the latest context messages, etc on the Channel objects.
        if (_appChannels.TryGetValue(request.ChannelId, out var appChannel))
        {
            return CreateAppChannelResponse.Created();
        }

        appChannel = _appChannels.GetOrAdd(request.ChannelId, addAppChannelFactory(request.ChannelId));

        try
        {
            await appChannel!.Connect();
        }
        catch (MessageRouterDuplicateEndpointException exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(exception, $"{request.ChannelId} is already registered as service endpoint.");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"An exception was thrown while executing {nameof(AddAppChannel)}.");
            _appChannels.TryRemove(request.ChannelId, out _);
            return CreateAppChannelResponse.Failed(ChannelError.CreationFailed);
        }

        return CreateAppChannelResponse.Created();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var observable = _moduleLoader.LifetimeEvents.ToAsyncObservable();
        var subscription = await observable.SubscribeAsync(async lifetimeEvent =>
        {
            switch (lifetimeEvent)
            {
                case LifetimeEvent.Stopped:
                    await RemoveModuleAsync(lifetimeEvent.Instance);
                    break;

                case LifetimeEvent.Started:
                    await AddOrUpdateModuleAsync(lifetimeEvent.Instance);
                    break;
            }
        });

        _subscription = subscription;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var userChannelDisposeTasks = _userChannels.Select(x => x.Value.DisposeAsync()).ToArray();
        var privateChannelDisposeTasks = _privateChannels.Select(x => x.Value.DisposeAsync()).ToArray();
        var appChannelDisposeTasks = _appChannels.Select(x => x.Value.DisposeAsync()).ToArray();

        await SafeWaitAsync(privateChannelDisposeTasks);
        await SafeWaitAsync(userChannelDisposeTasks);
        await SafeWaitAsync(appChannelDisposeTasks);

        if (_subscription != null)
        {
            _runningModules.Clear();
            await _subscription.DisposeAsync();
        }

        foreach (var pendingStartRequest in _pendingStartRequests)
        {
            pendingStartRequest.Value.TrySetCanceled();
        }

        _pendingStartRequests.Clear();
        _userChannels.Clear();
        _privateChannels.Clear();
        _appChannels.Clear();
        lock (_contextListenerLock)
        {
            _contextListeners.Clear();
        }

        _pendingStartRequests.Clear();
        _raisedIntentResolutions.Clear();
    }

    public bool FindChannel(string channelId, ChannelType channelType)
    {
        return channelType switch
        {
            ChannelType.User => _userChannels.TryGetValue(channelId, out _),
            ChannelType.Private => _privateChannels.TryGetValue(channelId, out _),
            ChannelType.App => _appChannels.TryGetValue(channelId, out _),
            _ => false,
        };
    }

    public async ValueTask<FindIntentResponse> FindIntent(FindIntentRequest? request, string? contextType)
    {
        if (request == null)
        {
            return FindIntentResponse.Failure(ResolveError.IntentDeliveryFailed);
        }

        var result = await GetAppIntentsByRequest(request.Intent, contextType, request.ResultType, null);

        return result.AppIntents.TryGetValue(request.Intent, out var appIntent)
            ? FindIntentResponse.Success(appIntent)
            : FindIntentResponse.Failure(ResolveError.NoAppsFound);
    }

    public async ValueTask<FindIntentsByContextResponse> FindIntentsByContext(FindIntentsByContextRequest? request, string? contextType)
    {
        if (request == null || contextType == null)
        {
            return FindIntentsByContextResponse.Failure(ResolveError.IntentDeliveryFailed);
        }

        var result = await GetAppIntentsByRequest(contextType: contextType, resultType: request.ResultType);

        return !result.AppIntents.Any()
            ? FindIntentsByContextResponse.Failure(ResolveError.NoAppsFound)
            : FindIntentsByContextResponse.Success(result.AppIntents.Values);
    }

    public async ValueTask<GetIntentResultResponse> GetIntentResult(GetIntentResultRequest? request)
    {
        if (request == null)
        {
            return GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed);
        }

        if (request.TargetAppIdentifier?.InstanceId == null)
        {
            return GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed);
        }

        try
        {
            var intentResolution = await GetIntentResolutionResult(request).WaitAsync(_options.IntentResultTimeout);

            if (intentResolution == null)
            {
                return GetIntentResultResponse.Failure(ResolveError.ResolverUnavailable);
            }

            if (intentResolution.ResultError != null)
            {
                return GetIntentResultResponse.Failure(intentResolution.ResultError);
            }

            if (!intentResolution.IsResolved
                && ((_raisedIntentResolutions.TryGetValue(
                         new Guid(request.TargetAppIdentifier.InstanceId!),
                         out var handler)
                        && !handler.IsIntentListenerRegistered(request.Intent))
                    || intentResolution.ResultError == null))
            {
                return GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed);
            }

            return GetIntentResultResponse.Success(
                intentResolution!.ResultChannelId,
                intentResolution!.ResultChannelType,
                intentResolution!.ResultContext,
                intentResolution!.ResultVoid);
        }
        catch (TimeoutException)
        {
            return GetIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed);
        }
    }

    public ValueTask<StoreIntentResultResponse> StoreIntentResult(StoreIntentResultRequest? request)
    {
        try
        {
            if (request == null)
            {
                return ValueTask.FromResult(StoreIntentResultResponse.Failure(ResolveError.IntentDeliveryFailed));
            }

            _raisedIntentResolutions.AddOrUpdate(
                new Guid(request.OriginFdc3InstanceId),
                _ => throw ThrowHelper.MissingAppFromRaisedIntentInvocations(request.OriginFdc3InstanceId),
                (key, oldValue) => oldValue.AddIntentResult(
                    messageId: request.MessageId,
                    intent: request.Intent,
                    channelId: request.ChannelId,
                    channelType: request.ChannelType,
                    context: request.Context,
                    voidResult: request.VoidResult,
                    error: request.Error));

            return ValueTask.FromResult(StoreIntentResultResponse.Success());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception was thrown while executing {nameof(StoreIntentResult)} call.");
            return ValueTask.FromResult(StoreIntentResultResponse.Failure(Fdc3DesktopAgentErrors.MissingId));
        }
    }

    public async ValueTask<RaiseIntentResult<IntentListenerResponse>> AddIntentListener(IntentListenerRequest? request)
    {
        if (request == null)
        {
            return new()
            {
                Response = IntentListenerResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull)
            };
        }

        switch (request.State)
        {
            case SubscribeState.Subscribe:
                if (_raisedIntentResolutions.TryGetValue(new(request.Fdc3InstanceId), out var resolver))
                {
                    resolver.AddIntentListener(request.Intent);

                    var resolutions = new List<RaiseIntentResolutionMessage>();
                    foreach (var raisedIntent in resolver.RaiseIntentResolutions.Where(
                                 invocation => invocation.Intent == request.Intent && !invocation.IsResolved))
                    {
                        if (!_runningModules.TryGetValue(new(request.Fdc3InstanceId), out var app))
                        {
                            continue;
                        }

                        var specification = new RaiseIntentSpecification
                        {
                            Intent = raisedIntent.Intent,
                            Context = raisedIntent.Context,
                            SourceAppInstanceId = new(raisedIntent.OriginFdc3InstanceId),
                            TargetAppMetadata = app.ToAppMetadata(request.Fdc3InstanceId),
                        };

                        var resolution = await GetRaiseIntentResolutionMessage(
                            raisedIntent.RaiseIntentMessageId,
                            specification);

                        if (resolution != null)
                        {
                            resolutions.Add(resolution);
                        }
                    }

                    return new()
                    {
                        Response = IntentListenerResponse.SubscribeSuccess(),
                        RaiseIntentResolutionMessages = resolutions
                    };
                }

                var createdResolver = _raisedIntentResolutions.GetOrAdd(
                    new(request.Fdc3InstanceId),
                    new RaisedIntentRequestHandler(_loggerFactory.CreateLogger<RaisedIntentRequestHandler>()));

                createdResolver.AddIntentListener(request.Intent);

                return new()
                {
                    Response = IntentListenerResponse.SubscribeSuccess(),
                };

            case SubscribeState.Unsubscribe:

                if (_raisedIntentResolutions.TryGetValue(new(request.Fdc3InstanceId), out var resolverToRemove))
                {
                    resolverToRemove.RemoveIntentListener(request.Intent);
                    return new()
                    {
                        Response = IntentListenerResponse.UnsubscribeSuccess()
                    };
                }

                break;
        }

        return new()
        {
            Response = IntentListenerResponse.Failure(Fdc3DesktopAgentErrors.MissingId)
        };
    }

    public async ValueTask<GetUserChannelsResponse> GetUserChannels(GetUserChannelsRequest? request)
    {
        if (request == null)
        {
            return GetUserChannelsResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull);
        }

        if (!Guid.TryParse(request.InstanceId, out var instanceId))
        {
            return GetUserChannelsResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
        }

        if (!_runningModules.TryGetValue(instanceId, out _))
        {
            return GetUserChannelsResponse.Failure(ChannelError.AccessDenied);
        }

        var result = (await _userChannelSetReader.GetUserChannelSet()).Values;
        if (result == null)
        {
            return GetUserChannelsResponse.Failure(Fdc3DesktopAgentErrors.NoUserChannelSetFound);
        }

        return GetUserChannelsResponse.Success(result);
    }

    public async ValueTask<JoinUserChannelResponse?> JoinUserChannel(Func<string, UserChannel> addUserChannelFactory, JoinUserChannelRequest request)
    {
        if (!Guid.TryParse(request.InstanceId, out var id) || !_runningModules.TryGetValue(id, out _))
        {
            return JoinUserChannelResponse.Failed(Fdc3DesktopAgentErrors.MissingId);
        }

        var userChannelSet = await _userChannelSetReader.GetUserChannelSet();
        ChannelItem? channelItem = null;
        if (!userChannelSet.TryGetValue(request.ChannelId, out channelItem))
        {
            return JoinUserChannelResponse.Failed(ChannelError.NoChannelFound);
        }

        try
        {
            await AddUserChannel(addUserChannelFactory, request.ChannelId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception is thrown while executing {nameof(JoinUserChannel)}.");
            return JoinUserChannelResponse.Failed(ChannelError.CreationFailed);
        }

        if (channelItem != null)
        {
            return JoinUserChannelResponse.Joined(channelItem.DisplayMetadata);
        }
        


        return JoinUserChannelResponse.Joined();
    }

    public async ValueTask<GetInfoResponse> GetInfo(GetInfoRequest? request)
    {
        if (request == null)
        {
            return GetInfoResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull);
        }

        if (request.AppIdentifier.InstanceId == null)
        {
            return GetInfoResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
        }

        var result = await GetAppInfo(request.AppIdentifier);

        return result;
    }

    public async ValueTask<FindInstancesResponse> FindInstances(FindInstancesRequest? request)
    {
        if (request == null)
        {
            return FindInstancesResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull);
        }

        if (!Guid.TryParse(request.Fdc3InstanceId, out var instanceId) || !_runningModules.TryGetValue(instanceId, out _))
        {
            return FindInstancesResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
        }

        try
        {
            await _appDirectory.GetApp(request.AppIdentifier.AppId!);
        }
        catch (AppNotFoundException)
        {
            return FindInstancesResponse.Failure(ResolveError.NoAppsFound);
        }

        var apps = _runningModules
            .Where(app => app.Value.AppId == request.AppIdentifier.AppId)
            .Select(x => new AppIdentifier() { AppId = x.Value.AppId, InstanceId = x.Key.ToString() });

        if (apps.Any())
        {
            return FindInstancesResponse.Success(apps);
        }
        else
        {
            return FindInstancesResponse.Success(Enumerable.Empty<AppIdentifier>());
        }
    }

    public async ValueTask<GetAppMetadataResponse> GetAppMetadata(GetAppMetadataRequest? request)
    {
        if (request == null)
        {
            return GetAppMetadataResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull);
        }

        if (!Guid.TryParse(request.Fdc3InstanceId, out var instanceId) || !_runningModules.TryGetValue(instanceId, out _))
        {
            return GetAppMetadataResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
        }

        if (request.AppIdentifier.InstanceId != null)
        {
            if (!Guid.TryParse(request.AppIdentifier.InstanceId, out var fdc3InstanceId))
            {
                return GetAppMetadataResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
            }

            if (!_runningModules.TryGetValue(fdc3InstanceId, out var app))
            {
                return GetAppMetadataResponse.Failure(ResolveError.TargetInstanceUnavailable);
            }

            var appMetadata = app.ToAppMetadata(request.AppIdentifier.InstanceId);
            return GetAppMetadataResponse.Success(appMetadata);
        }

        try
        {
            var app = await _appDirectory.GetApp(request.AppIdentifier.AppId);
            var appMetadata = app.ToAppMetadata();
            return GetAppMetadataResponse.Success(appMetadata);
        }
        catch (AppNotFoundException)
        {
            return GetAppMetadataResponse.Failure(ResolveError.TargetAppUnavailable);
        }
    }

    public ValueTask<AddContextListenerResponse?> AddContextListener(AddContextListenerRequest? request)
    {
        if (request == null)
        {
            return ValueTask.FromResult<AddContextListenerResponse?>(AddContextListenerResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull));
        }

        if (!Guid.TryParse(request.Fdc3InstanceId, out var originFdc3InstanceId) || !_runningModules.TryGetValue(originFdc3InstanceId, out _))
        {
            return ValueTask.FromResult<AddContextListenerResponse?>(AddContextListenerResponse.Failure(Fdc3DesktopAgentErrors.MissingId));
        }

        lock (_contextListenerLock)
        {
            var contextListener = new ContextListener(
                request.ContextType,
                request.ChannelId,
                request.ChannelType);

            _contextListeners.AddOrUpdate(
                originFdc3InstanceId,
                _ => new List<ContextListener> { contextListener },
                (_, contextListeners) =>
                {
                    contextListeners.Add(contextListener);
                    return contextListeners;
                });

            return ValueTask.FromResult<AddContextListenerResponse?>(AddContextListenerResponse.Added(contextListener.Id.ToString()));
        }
    }

    public ValueTask<RemoveContextListenerResponse?> RemoveContextListener(RemoveContextListenerRequest? request)
    {
        if (request == null)
        {
            return ValueTask.FromResult<RemoveContextListenerResponse?>(RemoveContextListenerResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull));
        }

        lock (_contextListenerLock)
        {
            if (!Guid.TryParse(request.Fdc3InstanceId, out var originFdc3InstanceId)
                || !_runningModules.TryGetValue(originFdc3InstanceId, out _)
                || !_contextListeners.TryGetValue(originFdc3InstanceId, out var listeners)
                || request.ListenerId == null
                || !Guid.TryParse(request.ListenerId, out var listenerId))
            {
                return ValueTask.FromResult<RemoveContextListenerResponse?>(RemoveContextListenerResponse.Failure(Fdc3DesktopAgentErrors.MissingId));
            }

            var listener = listeners.FirstOrDefault(x => x.Id == listenerId && x.ContextType == request.ContextType);
            if (listener == null)
            {
                return ValueTask.FromResult<RemoveContextListenerResponse?>(RemoveContextListenerResponse.Failure(Fdc3DesktopAgentErrors.ListenerNotFound));
            }

            listeners.Remove(listener);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("ContextListener has been successfully unsubscribed.");
            }

            return ValueTask.FromResult<RemoveContextListenerResponse?>(RemoveContextListenerResponse.Executed());
        }
    }

    //https://github.com/finos/FDC3/issues/1350
    public async ValueTask<OpenResponse?> Open(OpenRequest? request, string? contextType = null)
    {
        if (request == null)
        {
            return OpenResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull);
        }

        if (!Guid.TryParse(request.InstanceId, out var fdc3InstanceId)
            || !_runningModules.ContainsKey(fdc3InstanceId))
        {
            return OpenResponse.Failure(Fdc3DesktopAgentErrors.MissingId);
        }

        var contextId = Guid.NewGuid();

        try
        {
            var fdc3App = await _appDirectory.GetApp(request.AppIdentifier.AppId);
            var appMetadata = fdc3App.ToAppMetadata();
            var parameters = new Dictionary<string, string>();


            if (request.Context != null)
            {
                parameters.Add(Fdc3StartupParameters.OpenedAppContextId, contextId.ToString());
                _openedAppContexts.TryAdd(contextId, request.Context);
            }

            if (request.ChannelId != null)
            {
                parameters.Add(Fdc3StartupParameters.Fdc3ChannelId, request.ChannelId);
            }

            var target = await StartModule(appMetadata, parameters);

            if (!Guid.TryParse(target.InstanceId, out var targetInstanceId))
            {
                _openedAppContexts.TryRemove(contextId, out _);
                return OpenResponse.Failure(OpenError.ErrorOnLaunch);
            }

            if (request.Context == null)
            {
                return OpenResponse.Success(new AppIdentifier { AppId = target.AppId, InstanceId = target.InstanceId });
            }

            var cancellationToken = new CancellationToken();
            _ = await GetContextListener(targetInstanceId, contextType!, cancellationToken).WaitAsync(_options.ListenerRegistrationTimeout, cancellationToken);

            return OpenResponse.Success(new AppIdentifier { AppId = target.AppId, InstanceId = target.InstanceId });
        }
        catch (AppNotFoundException exception)
        {
            _logger.LogError(exception, $"Exception is thrown while executing the {nameof(Open)} request.");
            return OpenResponse.Failure(OpenError.AppNotFound);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception is thrown while executing the {nameof(Open)} request.");
            _openedAppContexts.TryRemove(contextId, out _);
            return OpenResponse.Failure(OpenError.AppTimeout);
        }
    }

    public ValueTask<GetOpenedAppContextResponse?> GetOpenedAppContext(GetOpenedAppContextRequest? request)
    {
        if (request == null)
        {
            return ValueTask.FromResult<GetOpenedAppContextResponse?>(GetOpenedAppContextResponse.Failure(Fdc3DesktopAgentErrors.PayloadNull));
        }

        if (!Guid.TryParse(request.ContextId, out var contextId))
        {
            return ValueTask.FromResult<GetOpenedAppContextResponse?>(GetOpenedAppContextResponse.Failure(Fdc3DesktopAgentErrors.IdNotParsable));
        }

        return !_openedAppContexts.TryRemove(contextId, out var context)
            ? ValueTask.FromResult<GetOpenedAppContextResponse?>(GetOpenedAppContextResponse.Failure(Fdc3DesktopAgentErrors.OpenedAppContextNotFound))
            : ValueTask.FromResult<GetOpenedAppContextResponse?>(GetOpenedAppContextResponse.Success(context));
    }

    public async ValueTask<RaiseIntentResult<RaiseIntentResponse>> RaiseIntentForContext(RaiseIntentForContextRequest request, string contextType)
    {
        if (request == null || contextType == null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
            };
        }

        var findIntentsByContextResult = await FindIntentsByContext(new FindIntentsByContextRequest() { Context = request.Context, Fdc3InstanceId = request.Fdc3InstanceId }, contextType);

        if (findIntentsByContextResult.AppIntents == null || !findIntentsByContextResult.AppIntents.Any())
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(request.TargetAppIdentifier == null ? ResolveError.NoAppsFound : ResolveError.TargetAppUnavailable)
            };
        }

        List<AppIntent> result;
        if (request.TargetAppIdentifier != null)
        {
            result = FilterAppIntentsByAppId(findIntentsByContextResult.AppIntents, request.TargetAppIdentifier).ToList();
        }
        else
        {
            result = findIntentsByContextResult.AppIntents.ToList();
        }

        RaiseIntentSpecification raiseIntentSpecification;
        if (result.Count > 1)
        {

            using var resolverUiIntentCancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            var resolverUIIntentResponse = await _resolverUI.SendResolverUIIntentRequest(result.Select(x => x.Intent.Name), resolverUiIntentCancellationSource.Token);

            if (resolverUIIntentResponse == null)
            {
                return new()
                {
                    Response = new()
                    {
                        Error = Fdc3DesktopAgentErrors.PayloadNull,
                    }
                };
            }

            if (resolverUIIntentResponse.Error != null)
            {
                return new()
                {
                    Response = new()
                    {
                        Error = resolverUIIntentResponse.Error,
                    }
                };
            }

            raiseIntentSpecification = new()
            {
                Context = request.Context,
                Intent = resolverUIIntentResponse.SelectedIntent!,
                RaisedIntentMessageId = request.MessageId,
                SourceAppInstanceId = new(request.Fdc3InstanceId)
            };
        }
        else
        {
            raiseIntentSpecification = new()
            {
                Context = request.Context,
                Intent = result.ElementAt(0).Intent.Name,
                RaisedIntentMessageId = request.MessageId,
                SourceAppInstanceId = new(request.Fdc3InstanceId)
            };
        }

        var appIntent = result.FirstOrDefault(x => x.Intent.Name == raiseIntentSpecification.Intent);
        if (appIntent == null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
            };
        }

        if (appIntent.Apps.Count() == 1)
        {
            raiseIntentSpecification.TargetAppMetadata = appIntent.Apps.ElementAt(0);
            return await RaiseIntentToApplication(raiseIntentSpecification);
        }

        using var resolverUICancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        var resolverUIResult = await _resolverUI.SendResolverUIRequest(appIntent.Apps, resolverUICancellationTokenSource.Token);
        if (resolverUIResult != null && resolverUIResult.Error == null)
        {
            raiseIntentSpecification.TargetAppMetadata = (AppMetadata) resolverUIResult.AppMetadata!;
            return await RaiseIntentToApplication(raiseIntentSpecification);
        }

        if (resolverUIResult?.Error != null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(resolverUIResult.Error)
            };
        }

        return new()
        {
            Response = RaiseIntentResponse.Failure(ResolveError.UserCancelledResolution)
        };
    }

    private IEnumerable<AppIntent> FilterAppIntentsByAppId(IEnumerable<AppIntent> source, AppIdentifier appId)
    {
        // Semantically we know this is not null or empty
        foreach (var intent in source)
        {
            foreach (var app in intent.Apps)
            {
                var matches = true;
                if (appId.AppId != null && appId.AppId != app.AppId)
                {
                    matches = false;
                }
                if (appId.InstanceId != null && appId.InstanceId != app.InstanceId)
                {
                    matches = false;
                }

                if (matches)
                {
                    yield return intent;
                    break;
                }
            }
        }
    }

    public async ValueTask<RaiseIntentResult<RaiseIntentResponse>> RaiseIntent(RaiseIntentRequest request, string contextType)
    {
        if (request == null || contextType == null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
            };
        }

        var intentQueryResult = await GetAppIntentsByRequest(request.Intent, contextType, targetAppIdentifier: request.TargetAppIdentifier);

        if (intentQueryResult.Error != null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(intentQueryResult.Error)
            };
        }

        //No intents were found which would have the right information to handle the raised intent
        if (!intentQueryResult.AppIntents.TryGetValue(request.Intent, out var appIntent) || !appIntent.Apps.Any())
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(request?.TargetAppIdentifier?.AppId == null || request.Context != null ? ResolveError.NoAppsFound : ResolveError.TargetAppUnavailable)
            };
        }

        RaiseIntentSpecification raiseIntentSpecification = new()
        {
            Context = request.Context,
            Intent = request.Intent,
            RaisedIntentMessageId = request.MessageId,
            SourceAppInstanceId = new(request.Fdc3InstanceId)
        };

        if (appIntent.Apps.Count() == 1)
        {
            raiseIntentSpecification.TargetAppMetadata = appIntent.Apps.ElementAt(0);

            return await RaiseIntentToApplication(raiseIntentSpecification);
        }

        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        //Resolve to one app via ResolverUI.
        var result = await _resolverUI.SendResolverUIRequest(appIntent.Apps, cancellationTokenSource.Token);

        if (result != null && result.Error == null)
        {
            raiseIntentSpecification.TargetAppMetadata = (AppMetadata) result.AppMetadata!;
            return await RaiseIntentToApplication(raiseIntentSpecification);
        }

        if (result?.Error != null)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(result.Error)
            };
        }

        return new()
        {
            Response = RaiseIntentResponse.Failure(ResolveError.UserCancelledResolution)
        };
    }

    //Here we have a specific application which should either start or we should send a intent resolution request
    private async ValueTask<RaiseIntentResult<RaiseIntentResponse>> RaiseIntentToApplication(RaiseIntentSpecification raiseIntentSpecification)
    {
        async Task<RaiseIntentResult<RaiseIntentResponse>?> GetRaiseIntentResponse(RaiseIntentSpecification raiseIntentSpecification, string messageId)
        {
            RaiseIntentResult<RaiseIntentResponse>? response = null;

            while (response == null)
            {
                if (!_raisedIntentResolutions.TryGetValue(new(raiseIntentSpecification.TargetAppMetadata.InstanceId!), out var registeredFdc3App)
                    || !registeredFdc3App.IsIntentListenerRegistered(raiseIntentSpecification.Intent))
                {
                    await Task.Delay(1);
                    continue;
                }

                var resolution = await GetRaiseIntentResolutionMessage(messageId, raiseIntentSpecification);
                response = new()
                {
                    Response = RaiseIntentResponse.Success(messageId, raiseIntentSpecification.Intent, raiseIntentSpecification.TargetAppMetadata),
                    RaiseIntentResolutionMessages = resolution == null
                        ? Enumerable.Empty<RaiseIntentResolutionMessage>()
                        : [resolution]
                };
            }

            return response;
        }

        try
        {
            if (!string.IsNullOrEmpty(raiseIntentSpecification.TargetAppMetadata.InstanceId))
            {
                var raisedIntentMessageId = StoreRaisedIntentForTarget(raiseIntentSpecification);

                var response = await GetRaiseIntentResponse(raiseIntentSpecification, raisedIntentMessageId).WaitAsync(_options.ListenerRegistrationTimeout);
                if (response != null)
                {
                    return response;
                }

                return new()
                {
                    Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
                };
            }

            try
            {
                var module = await StartModule(raiseIntentSpecification.TargetAppMetadata);

                raiseIntentSpecification.TargetAppMetadata = module;

                var raisedIntentMessageId = StoreRaisedIntentForTarget(raiseIntentSpecification);

                if (!_raisedIntentResolutions.TryGetValue(new Guid(module.InstanceId!), out _))
                {
                    return new()
                    {
                        Response = RaiseIntentResponse.Failure(ResolveError.TargetInstanceUnavailable)
                    };
                }

                var response = await GetRaiseIntentResponse(raiseIntentSpecification, raisedIntentMessageId).WaitAsync(_options.ListenerRegistrationTimeout);

                if (response != null)
                {
                    return response;
                }

                return new()
                {
                    Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
                };
            }
            catch (Fdc3DesktopAgentException exception)
            {
                _logger.LogError(exception, "Error while starting module.");

                return new()
                {
                    Response = RaiseIntentResponse.Failure(exception.ToString()),
                };
            }
        }
        catch (TimeoutException)
        {
            return new()
            {
                Response = RaiseIntentResponse.Failure(ResolveError.IntentDeliveryFailed)
            };
        }
    }

    private async ValueTask<AppMetadata> StartModule(AppMetadata targetAppMetadata, IEnumerable<KeyValuePair<string, string>>? additionalStartupParameters = null)
    {
        var startupParameters = additionalStartupParameters?.ToDictionary(x => x.Key, y => y.Value) ?? new Dictionary<string, string>();

        var fdc3InstanceId = Guid.NewGuid().ToString();
        startupParameters.Add(Fdc3StartupParameters.Fdc3InstanceId, fdc3InstanceId);

        var startRequest = new StartRequest(
            targetAppMetadata.AppId, //TODO: possible remove some identifier like @"fdc3."
            startupParameters);

        var taskCompletionSource = new TaskCompletionSource<IModuleInstance>();

        if (_pendingStartRequests.TryAdd(startRequest, taskCompletionSource))
        {
            var moduleInstance = await _moduleLoader.StartModule(startRequest);

            if (moduleInstance == null)
            {
                var exception = ThrowHelper.TargetInstanceUnavailable();

                if (!_pendingStartRequests.TryRemove(startRequest, out _))
                {
                    _logger.LogWarning($"Could not remove {nameof(StartRequest)} from the pending requests. ModuleId: {startRequest.ModuleId}.");
                }

                taskCompletionSource.TrySetException(exception);
            }

            await taskCompletionSource.Task;
        }

        return new AppMetadata
        {
            AppId = targetAppMetadata.AppId,
            InstanceId = fdc3InstanceId,
            Name = targetAppMetadata.Name,
            Version = targetAppMetadata.Version,
            Title = targetAppMetadata.Title,
            Tooltip = targetAppMetadata.Tooltip,
            Description = targetAppMetadata.Description,
            Icons = targetAppMetadata.Icons.Select(Icon.GetIcon),
            Screenshots = targetAppMetadata.Screenshots.Select(Screenshot.GetScreenshot),
            ResultType = targetAppMetadata.ResultType
        };
    }

    private async Task<RaiseIntentResolutionInvocation?> GetIntentResolutionResult(GetIntentResultRequest request)
    {
        RaiseIntentResolutionInvocation? resolution;
        while (
            !_raisedIntentResolutions.TryGetValue(new Guid(request.TargetAppIdentifier.InstanceId!), out var resolver)
            || !resolver.TryGetRaisedIntentResult(request.MessageId, request.Intent, out resolution)
            || (resolution.ResultChannelId == null
                && resolution.ResultChannelType == null
                && resolution.ResultContext == null
                && resolution.ResultVoid == null))
        {
            await Task.Delay(100);
        }

        return resolution;
    }

    private string StoreRaisedIntentForTarget(RaiseIntentSpecification raiseIntentSpecification)
    {
        var invocation = new RaiseIntentResolutionInvocation(
            raiseIntentMessageId: raiseIntentSpecification.RaisedIntentMessageId,
            intent: raiseIntentSpecification.Intent,
            originFdc3InstanceId: raiseIntentSpecification.SourceAppInstanceId.ToString(),
            contextToHandle: raiseIntentSpecification.Context);

        //At this point the InstanceId should be not null.
        _raisedIntentResolutions.AddOrUpdate(
            new(raiseIntentSpecification.TargetAppMetadata.InstanceId!),
            _ =>
            {
                var resolver = new RaisedIntentRequestHandler(_loggerFactory.CreateLogger<RaisedIntentRequestHandler>());
                resolver.AddRaiseIntentToHandle(invocation);
                return resolver;
            },
            (key, oldValue) => oldValue.AddRaiseIntentToHandle(invocation));

        return invocation.RaiseIntentMessageId;
    }

    //Publishing intent resolution request to the fdc3 clients, they will receive the message and start their IntentHandler appropriately, and send a store request back to the backend.
    private Task<RaiseIntentResolutionMessage?> GetRaiseIntentResolutionMessage(
        string raisedIntentMessageId,
        RaiseIntentSpecification raiseIntentSpecification)
    {
        if (!_runningModules.TryGetValue(raiseIntentSpecification.SourceAppInstanceId, out var sourceApp))
        {
            return Task.FromResult<RaiseIntentResolutionMessage?>(null);
        }

        var sourceAppIdentifier = new AppIdentifier
        {
            AppId = sourceApp.AppId,
            InstanceId = raiseIntentSpecification.SourceAppInstanceId.ToString()
        };

        return Task.FromResult<RaiseIntentResolutionMessage?>(
            new()
            {
                Intent = raiseIntentSpecification.Intent,
                TargetModuleInstanceId = raiseIntentSpecification.TargetAppMetadata.InstanceId!,
                Request = new RaiseIntentResolutionRequest
                {
                    MessageId = raisedIntentMessageId,
                    Context = raiseIntentSpecification.Context,
                    ContextMetadata = new ContextMetadata
                    {
                        Source = sourceAppIdentifier
                    }
                }
            });
    }

    private async Task<IntentQueryResult> GetAppIntentsByRequest(
        string? intent = null,
        string? contextType = null,
        string? resultType = null,
        IAppIdentifier? targetAppIdentifier = null)
    {
        var result = new IntentQueryResult();

        Guid instanceId = Guid.Empty;
        if (targetAppIdentifier?.InstanceId == null)
        {
            try
            {
                var filteredApps = await _intentResolver.GetMatchingAppsFromAppDirectory(intent, contextType, resultType, targetAppIdentifier?.AppId);

                var appIntents = GetAppIntentsFromIntentMetadataCollection(filteredApps);
                result.AppIntents = appIntents;
            }
            catch (AppNotFoundException)
            {
                result.Error = ResolveError.TargetAppUnavailable;
                return result;
            }
        }
        else if (!Guid.TryParse(targetAppIdentifier.InstanceId, out instanceId))
        {
            result.Error = ResolveError.TargetInstanceUnavailable;
            return result;
        }

        try
        {
            var filteredInstances = await _intentResolver.GetMatchingAppInstances(intent, contextType, resultType, targetAppIdentifier?.AppId, targetAppIdentifier?.InstanceId == null ? null : instanceId);
            var appIntents = GetAppIntentsFromIntentMetadataCollection(filteredInstances);
            foreach (var app in appIntents)
            {
                // While only semantically, at this point we know the app listens for intents as we filter for that.
                // TODO: this could probably be made more obvious from the code as well.

                if (result.AppIntents.TryGetValue(app.Key, out var appIntent))
                {
                    appIntent.Apps = appIntent.Apps.Concat(app.Value.Apps);
                }
                else
                {
                    result.AppIntents.Add(app.Key, app.Value);
                }
            }
        }
        catch (Fdc3DesktopAgentException fdc3ex)
        {
            result.Error = fdc3ex.ErrorType;
        }

        return result;
    }

    private Dictionary<string, AppIntent> GetAppIntentsFromIntentMetadataCollection(
        IEnumerable<FlatAppIntent> intentMetadataCollection)
    {
        Dictionary<string, AppIntent> appIntents = new Dictionary<string, AppIntent>();

        foreach (var intentMetadata in intentMetadataCollection)
        {
            var appMetadata = intentMetadata.App.ToAppMetadata(intentMetadata.InstanceId?.ToString(), intentMetadata.Intent.ResultType);

            if (!appIntents.TryGetValue(intentMetadata.Intent.Name, out var appIntent))
            {
                appIntent = new AppIntent
                {
                    Intent = new Protocol.IntentMetadata
                    { Name = intentMetadata.Intent.Name, DisplayName = intentMetadata.Intent.DisplayName },
                    Apps = Enumerable.Empty<AppMetadata>()
                };

                appIntents.Add(intentMetadata.Intent.Name, appIntent);
            }

            appIntent.Apps = appIntent.Apps.Append(appMetadata);
        }

        return appIntents;
    }

    private ValueTask<GetInfoResponse> GetAppInfo(IAppIdentifier appIdentifier)
    {
        if (!Guid.TryParse(appIdentifier.InstanceId!, out var instanceId))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Instance id: {appIdentifier.InstanceId} cannot be parsed to {typeof(Guid)}.");
            }

            return ValueTask.FromResult<GetInfoResponse>(GetInfoResponse.Failure(Fdc3DesktopAgentErrors.MissingId));
        }

        if (!_runningModules.TryGetValue(instanceId, out var app))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Instance id: {instanceId} is missing from the tracked modules of {Constants.DesktopAgentProvider}.");
            }

            return ValueTask.FromResult<GetInfoResponse>(GetInfoResponse.Failure(Fdc3DesktopAgentErrors.MissingId));
        }

        var implementationMetadata = new ImplementationMetadata
        {
            AppMetadata = app.ToAppMetadata(appIdentifier.InstanceId),
            Fdc3Version = Constants.SupportedFdc3Version,
            OptionalFeatures = new OptionalDesktopAgentFeatures
            {
                OriginatingAppMetadata = false,
                UserChannelMembershipAPIs = Constants.SupportUserChannelMembershipAPI
            },
            Provider = Constants.DesktopAgentProvider,
            ProviderVersion = Constants.ComposeUIVersion ?? "0.0.0"
        };

        return ValueTask.FromResult<GetInfoResponse>(GetInfoResponse.Success(implementationMetadata));
    }

    private async Task<ContextListener> GetContextListener(Guid instanceId, string contextType, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"ContextListener is not registered in time for context type {contextType}");
            }

            lock (_contextListenerLock)
            {
                if (_contextListeners.TryGetValue(instanceId, out var listeners))
                {
                    var listener = listeners.FirstOrDefault(x => x.ContextType == contextType);

                    if (listener != null)
                    {
                        return listener;
                    }
                }
            }

            await Task.Delay(1);
        }
    }

    private Task RemoveModuleAsync(IModuleInstance instance)
    {
        if (!IsFdc3StartedModule(instance, out var fdc3InstanceId))
        {
            return Task.CompletedTask;
        }

        if (!Guid.TryParse(fdc3InstanceId, out var id))
        {
            return Task.CompletedTask;
        }

        if (!_runningModules.TryRemove(id, out _)) //At this point the fdc3InstanceId shouldn't be null
        {
            _logger.LogError($"Could not remove the closed window with instanceId: {fdc3InstanceId}.");
        }

        if (_pendingStartRequests.TryRemove(instance.StartRequest, out var taskCompletionSource))
        {
            taskCompletionSource.SetException(ThrowHelper.TargetInstanceUnavailable());
        }

        lock (_contextListenerLock)
        {
            if (!_contextListeners.TryRemove(id, out _))
            {
                _logger.LogError($"Could not remove the registered context listeners of id: {fdc3InstanceId}.");
            }
        }

        if (!_raisedIntentResolutions.TryRemove(id, out _))
        {
            _logger.LogError($"Could not remove the stored intent resolutions of id: {fdc3InstanceId} which raised the intents.");
        }

        return Task.CompletedTask;
    }

    private async Task AddOrUpdateModuleAsync(IModuleInstance instance)
    {
        if (!IsFdc3StartedModule(instance, out var fdc3InstanceId))
        {
            return;
        }

        Fdc3App fdc3App;
        try
        {
            fdc3App = await _appDirectory.GetApp(instance.Manifest.Id);

            if (_pendingStartRequests.TryRemove(instance.StartRequest, out var taskCompletionSource))
            {
                taskCompletionSource.SetResult(instance);
            }
        }
        catch (AppNotFoundException exception)
        {
            _logger.LogError(exception, $"Could not retrieve app: {instance.Manifest.Id} from AppDirectory.");
            return;
        }

        //TODO: should add some identifier to the query => "fdc3:" + instance.Manifest.Id
        _runningModules.GetOrAdd(
            new(fdc3InstanceId!), //At this point the fdc3InstanceId shouldn't be null
            _ => fdc3App);
    }

    private bool IsFdc3StartedModule(IModuleInstance instance, out string? instanceId)
    {
        instanceId = string.Empty;
        var fdc3InstanceId = instance.StartRequest.Parameters.FirstOrDefault(parameter => parameter.Key == Fdc3StartupParameters.Fdc3InstanceId);

        if (string.IsNullOrEmpty(fdc3InstanceId.Value))
        {
            var startupProperties = instance.GetProperties().FirstOrDefault(property => property is Fdc3StartupProperties);

            instanceId = startupProperties == null ? null : ((Fdc3StartupProperties) startupProperties).InstanceId;

            return startupProperties == null
                ? false
                : true;
        }

        instanceId = fdc3InstanceId.Value;
        return true;
    }

    private async ValueTask SafeWaitAsync(IEnumerable<ValueTask> tasks)
    {
        foreach (var task in tasks)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception was thrown while waiting for a task to finish.");
            }
        }
    }
}