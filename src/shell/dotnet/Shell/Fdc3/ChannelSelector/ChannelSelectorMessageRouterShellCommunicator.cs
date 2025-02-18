using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Finos.Fdc3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Converters;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Messaging.Abstractions;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    internal class ChannelSelectorMessageRouterShellCommunicator : IChannelSelectorCommunicator
    {
        private readonly ILogger<ChannelSelectorMessageRouterShellCommunicator> _logger;
        private readonly IMessageRouter _messageRouter;
        private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(2);

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Converters = { new AppMetadataJsonConverter() }
        };

        public ChannelSelectorMessageRouterShellCommunicator(
            IMessageRouter messageRouter,
            ILogger<ChannelSelectorMessageRouterShellCommunicator>? logger = null)
        {
            _messageRouter = messageRouter;
            _logger = logger ?? NullLogger<ChannelSelectorMessageRouterShellCommunicator>.Instance;
        }

        public async Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await SendChannelSelectorRequestCore(channelId, instanceId, cancellationToken);
            }
            catch (TimeoutException ex)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(ex, "MessageRouter didn't receive response from the Channel Selector.");
                }

                return new ChannelSelectorResponse()
                {
                    Error = ResolveError.ResolverTimeout
                };
            }
        }

        private async Task<ChannelSelectorResponse?> SendChannelSelectorRequestCore(string channelId, string instanceId, CancellationToken cancellationToken = default)
        {
            var request = new ChannelSelectorRequest
            {
                

                ChannelId = channelId,
                InstanceId = instanceId
            };

            var responseBuffer = await _messageRouter.InvokeAsync(
                "ComposeUI/fdc3/v2.0/channelSelector",
               
            MessageBuffer.Factory.CreateJson(request, _jsonSerializerOptions),
                cancellationToken: cancellationToken);

            if (responseBuffer == null)
            {
                return null;
            }

            var response = responseBuffer.ReadJson<ChannelSelectorResponse>(_jsonSerializerOptions);

            return response;
        }
    }
}
