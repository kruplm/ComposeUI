using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finos.Fdc3;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Protocol;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent
{
    public interface IChannelSelectorDACommunicator
    {
        public Task<ChannelSelectorResponse?> SendChannelSelectorRequest(string channelId, string instanceId, CancellationToken cancellationToken = default);
        public Task<ChannelSelectorResponse?> SendChannelSelectorColorUpdateRequest(JoinUserChannelRequest req, string? color, CancellationToken cancellationToken = default);
    }
}