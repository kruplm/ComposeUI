using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.Contracts;
using Windows.Media.AppBroadcasting;

namespace MorganStanley.ComposeUI.Fdc3.DesktopAgent
{
    public delegate Task Fdc3ChannelSelectorColorupdateEventHandler(string color);
    public interface IChannelSelectorInstanceCommunicator
    {
        public Task RegisterMessageRouterForInstance(string instanceId, Fdc3ChannelSelectorColorupdateEventHandler eventHandler, CancellationToken cancellationToken = default);

        public void InvokeChannelSelectorRequest(ChannelSelectorRequest request, CancellationToken cancellationToken = default);
    }
}
