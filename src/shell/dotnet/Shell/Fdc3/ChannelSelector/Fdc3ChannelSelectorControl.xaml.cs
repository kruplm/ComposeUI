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
        //This is just testing the connection, the DA shouldn§t be passed to the UserControl
        //private readonly Fdc3DesktopAgent _desktopAgent; //=

        public Fdc3ChannelSelectorControl(Fdc3ChannelSelectorViewModel? viewModel)
        {

            _viewModel = viewModel;
           
            this.DataContext = this;
            InitializeComponent();

        }

        public Fdc3ChannelSelectorControl() {
            this.DataContext = this;
            InitializeComponent();
        }

        //TODO: populate the list from GetUserChannelSet() instead of providing static data in the xaml
        //private void initializeChannelSelector()


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            var channelNumber = (string)btn.Content;
            var color = btn.Background;

            ChannelSelector.BorderBrush = color;

            
            
            //this is just for proving we can connect to the DA, and switch channels. Once this works, we populate this with the selected channel ID


            //_desktopAgent.JoinUserChannel("fdc3.channel."+channelNumber );
        }
    }
}
