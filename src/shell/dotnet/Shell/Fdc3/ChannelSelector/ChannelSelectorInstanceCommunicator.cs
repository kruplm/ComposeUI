using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Finos.Fdc3;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;
using MorganStanley.ComposeUI.Messaging.Protocol.Messages;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using MorganStanley.ComposeUI.Messaging.Protocol;
using System.Text.Json.Nodes;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    internal class ChannelSelectorInstanceCommunicator : IChannelSelectorInstanceCommunicator
    {
        private readonly ILogger<ChannelSelectorInstanceCommunicator> _logger;
        public IMessageRouter MessageRouter { get; }
        private readonly object _disposeLock = new();

        private readonly List<Func<ValueTask>> _disposeTask = new(); 
        private string? _instanceId;
        private string? _channelId;
        private string? _color;
        private IDesktopAgent _desktopAgent;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter(), new IconJsonConverter() }
        };

        IChannelSelectorInstanceCommunicator _channelSelectorComm;

        public ChannelSelectorInstanceCommunicator(IMessageRouter messageRouter) {
            MessageRouter = messageRouter;
        }
         public async void InvokeChannelSelectorRequest(ChannelSelectorRequest request, CancellationToken cancellationToken = default)
         { 

             /*var responseBuffer = await MessageRouter.InvokeAsync(
                 $"ComposeUI/fdc3/v2.0/channelSelector-{request.InstanceId}",
                 MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                 cancellationToken: cancellationToken
             );*/

             //todo invoke the actual service instead
             await MessageRouter.PublishAsync(
                 "ComposeUI/fdc3/v2.0/channelSelector2",
                 MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                 cancellationToken: cancellationToken

             );
         }
        
         public async Task RegisterMessageRouterForInstance(string instanceId, Fdc3ChannelSelectorColorupdateEventHandler eventHandler)
         {
             await MessageRouter.RegisterServiceAsync(
             $"ComposeUI/fdc3/v2.0/channelSelector-{instanceId}",
             async (endpoint, payload, context) =>
             {
                 var request = payload?.ReadJson<ChannelSelectorRequest>(_jsonSerializerOptions);
                 if (request == null)
                 {
                     return null;
                 }
                 else
                 {
                     _instanceId = request.InstanceId;
                 }

                 return null;
             },
             cancellationToken: default);

            
            var subscription = await MessageRouter.SubscribeAsync($"ComposeUI/fdc3/v2.0/channelSelectorColor-{instanceId}", 
                
                async (payloadBuffer) => {
                    var request = payloadBuffer?.ReadJson<JoinUserChannelRequest>(_jsonSerializerOptions);
                    if (request == null)
                    {
                        return;
                    }

                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        eventHandler(request.Color);
                    });
                },
                cancellationToken: default);

             lock (_disposeLock)
             {
                 _disposeTask.Add(
                     async () =>
                     {
                         if (MessageRouter != null)
                         {
                             await MessageRouter.UnregisterServiceAsync($"ComposeUI/fdc3/v2.0/channelSelector-{instanceId}");
                             await MessageRouter.DisposeAsync();
                         }
                     });
             }
         }
    }
}