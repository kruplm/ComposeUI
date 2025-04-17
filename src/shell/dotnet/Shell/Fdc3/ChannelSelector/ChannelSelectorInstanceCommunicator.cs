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
using System.Text.Json.Serialization;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    internal class ChannelSelectorInstanceCommunicator : IChannelSelectorInstanceCommunicator
    {
        private readonly ILogger<ChannelSelectorInstanceCommunicator> _logger;
        private readonly IMessageRouter _messageRouter;
        private readonly object _disposeLock = new();
        private readonly List<Func<ValueTask>> _disposeTask = new();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

        IChannelSelectorInstanceCommunicator _channelSelectorComm;

        public ChannelSelectorInstanceCommunicator(IMessageRouter messageRouter) {
            _messageRouter = messageRouter;
        }
         public async void InvokeChannelSelectorRequest(ChannelSelectorRequest request, CancellationToken cancellationToken = default)
         {
             await _messageRouter.PublishAsync(
                 "ComposeUI/fdc3/v2.0/changeChannel",
                 MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                 cancellationToken: cancellationToken
             );
         }
        
         public async Task RegisterMessageRouterForInstance(string instanceId, Fdc3ChannelSelectorColorupdateEventHandler eventHandler, CancellationToken cancellationToken = default)
         {
             await _messageRouter.RegisterServiceAsync(
             $"ComposeUI/fdc3/v2.0/channelSelector-{instanceId}",
             async (endpoint, payload, context) =>
             {
                 var request = payload?.ReadJson<ChannelSelectorRequest>(_jsonSerializerOptions);
                 if (request == null)
                 {
                     return null;
                 }
                 

                 return null;
             },
             cancellationToken: cancellationToken);

            
            var subscription = await _messageRouter.SubscribeAsync($"ComposeUI/fdc3/v2.0/channelSelectorColor-{instanceId}", 
                
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
                         if (_messageRouter != null)
                         {
                             await _messageRouter.UnregisterServiceAsync($"ComposeUI/fdc3/v2.0/channelSelector-{instanceId}");
                             await _messageRouter.DisposeAsync();
                         }
                     });
             }
         }
    }
}