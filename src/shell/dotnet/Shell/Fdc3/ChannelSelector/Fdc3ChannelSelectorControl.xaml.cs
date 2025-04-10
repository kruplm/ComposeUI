using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;

namespace MorganStanley.ComposeUI.Shell.Fdc3.ChannelSelector
{
    /// <summary>
    /// Interaction logic for Fdc3ChannelSelectorControl.xaml
    /// </summary>
    public partial class Fdc3ChannelSelectorControl : UserControl
    {
        private readonly Fdc3ChannelSelectorViewModel? _viewModel;
        private string _instanceId;

        public Fdc3ChannelSelectorControl(IChannelSelectorInstanceCommunicator channelSelectorCommunicator, string color, string instanceId) {
            InitializeComponent();

            _viewModel = new Fdc3ChannelSelectorViewModel(channelSelectorCommunicator, instanceId, color);
            _instanceId = instanceId;
            DataContext = _viewModel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            var channelNumber = (string)btn.Content;
            var color = btn.Background;
            await Task.Run(() =>
            {
                _viewModel.SendChannelSelectorRequest(channelNumber, _instanceId);
            });
            ChannelSelector.BorderBrush = color;
        }

        public async Task ColorUpdate(string color) 
        {
            await _viewModel.UpdateChannelSelectorColor(color);   
        }
    }
}
