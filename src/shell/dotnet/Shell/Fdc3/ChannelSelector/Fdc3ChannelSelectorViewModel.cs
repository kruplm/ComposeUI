using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Finos.Fdc3;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using CommunityToolkit.Mvvm.Input;
using System.Windows;


namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    public class Fdc3ChannelSelectorViewModel
    {
        public IChannelSelectorCommunicator ChannelSelector;
        private ICommand? _joinChannelCommand;
        public ICommand SelectCurrentChannelCommand { get; }


        public Fdc3ChannelSelectorViewModel(IChannelSelectorCommunicator channelSelector)
        {
            ChannelSelector = channelSelector;
            //SelectCurrentChannelCommand = new RelayCommand(SelectCurrentChannelClick);
        }


        public IChannel? SelectedChannel { get; set; }

        //Todo: Implement Command
        public ICommand JoinChannelCommand
        {
            get
            {
                return _joinChannelCommand ?? (_joinChannelCommand = new RelayCommand(() =>
                {
                }));
            }
        }

    }

}
