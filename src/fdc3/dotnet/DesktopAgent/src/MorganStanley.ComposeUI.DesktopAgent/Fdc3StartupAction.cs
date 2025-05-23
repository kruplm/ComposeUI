﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.ModuleLoader;
using Finos.Fdc3.AppDirectory;
using ResourceReader = MorganStanley.ComposeUI.Utilities.ResourceReader;
using Microsoft.Extensions.Options;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.DependencyInjection;
using System.Text;

namespace MorganStanley.ComposeUI.Shell.Fdc3;

internal sealed class Fdc3StartupAction : IStartupAction
{
    private readonly IAppDirectory _appDirectory;
    private readonly IUserChannelSetReader _userChannelSetReader;
    private readonly Fdc3DesktopAgentOptions _options;
    private readonly ILogger<Fdc3StartupAction> _logger;

    public Fdc3StartupAction(
        IAppDirectory appDirectory,
        IUserChannelSetReader userChannelSetReader,
        IOptions<Fdc3DesktopAgentOptions> options,
        ILogger<Fdc3StartupAction>? logger = null)
    {
        _appDirectory = appDirectory;
        _userChannelSetReader = userChannelSetReader;
        _options = options.Value;
        _logger = logger ?? NullLogger<Fdc3StartupAction>.Instance;
    }

    public async Task InvokeAsync(StartupContext startupContext, Func<Task> next)
    {
        if (startupContext.ModuleInstance.Manifest.ModuleType == ModuleType.Web)
        {
            //TODO: should add some identifier to the query => "fdc3:" + startupContext.StartRequest.ModuleId
            try
            {
                var appId = (await _appDirectory.GetApp(startupContext.StartRequest.ModuleId)).AppId;
                var userChannelSet = await _userChannelSetReader.GetUserChannelSet();
                var fdc3InstanceId = startupContext.StartRequest.Parameters.FirstOrDefault(parameter => parameter.Key == Fdc3StartupParameters.Fdc3InstanceId).Value ?? Guid.NewGuid().ToString();
                
                //TODO: decide if we want to join to a channel automatically on startup of an fdc3 module
                //First we inject the channel id if we set as startup parameter eg.: when we open an app with `fdc3.open`
                var channelId = startupContext.StartRequest.Parameters.FirstOrDefault(parameter => parameter.Key == Fdc3StartupParameters.Fdc3ChannelId).Value ?? _options.ChannelId ?? userChannelSet.FirstOrDefault().Key;
                
                var openedAppContextId =
                    startupContext.StartRequest.Parameters.FirstOrDefault(
                        x => x.Key == Fdc3StartupParameters.OpenedAppContextId).Value;

                var fdc3StartupProperties = new Fdc3StartupProperties() { InstanceId = fdc3InstanceId, ChannelId = channelId, OpenedAppContextId = openedAppContextId };
                fdc3InstanceId = startupContext.GetOrAddProperty<Fdc3StartupProperties>(_ => fdc3StartupProperties).InstanceId;

                var webProperties = startupContext.GetOrAddProperty<WebStartupProperties>();

                var stringBuilder = new StringBuilder();
                stringBuilder.Append($$"""
                            window.composeui.fdc3 = {
                                ...window.composeui.fdc3, 
                                config: {
                                    appId: "{{appId}}",
                                    instanceId: "{{fdc3InstanceId}}"
                                }
                          """);

                if (channelId != null)
                {
                    stringBuilder.Append($$"""
                        ,
                        channelId: "{{channelId}}"
                        """);
                }

                if (openedAppContextId != null)
                {
                    stringBuilder.Append($$"""
                        ,
                        openAppIdentifier: {
                            openedAppContextId: "{{openedAppContextId}}"
                        }
                        """);
                }

                stringBuilder.Append($$"""
                    };
                    """);

                stringBuilder.AppendLine();
                stringBuilder.Append(ResourceReader.ReadResource(ResourceNames.Fdc3Bundle));

                webProperties.ScriptProviders.Add(_ => new ValueTask<string>(stringBuilder.ToString()));
            }
            catch (AppNotFoundException exception)
            {
                _logger.LogError(exception, $"Fdc3 bundle js could be not added to the {startupContext.StartRequest.ModuleId}.");
            }
        }

        await (next.Invoke());
    }
}