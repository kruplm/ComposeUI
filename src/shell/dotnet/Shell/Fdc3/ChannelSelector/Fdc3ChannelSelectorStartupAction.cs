using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Shell.Messaging;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    internal class Fdc3ChannelSelectorStartupAction : IStartupAction
    {
        private Fdc3ChannelSelectorControl _fdc3ChannelSelectorControl;
        private IMessageRouter _messageRouter;
        private string _color;
        private string _instanceId;
        private readonly IChannelSelectorInstanceCommunicator _channelSelectorInstanceCommunicator;


        public Fdc3ChannelSelectorStartupAction( IMessageRouter messageRouter)
        {
           _messageRouter = messageRouter;
           _channelSelectorInstanceCommunicator = new ChannelSelectorInstanceCommunicator(_messageRouter);
        }

        public async Task InvokeAsync(StartupContext startupContext, Func<Task> next)
        {
            if (startupContext.ModuleInstance.Manifest.ModuleType == ModuleType.Web)
            {
                var webStartupProperties = startupContext.GetOrAddProperty<WebStartupProperties>();
                var color = webStartupProperties.ChannelColor;
                var instanceId = webStartupProperties.InstanceId;


                _fdc3ChannelSelectorControl = new Fdc3ChannelSelectorControl(_channelSelectorInstanceCommunicator, color, instanceId);


                await Task.Run(async () =>
                {
                    await _channelSelectorInstanceCommunicator.RegisterMessageRouterForInstance(instanceId, _fdc3ChannelSelectorControl.ColorUpdate);
                });


                webStartupProperties.Fdc3ChannelSelectorControl = _fdc3ChannelSelectorControl;
            }

            await (next.Invoke());
        }
    }
}
