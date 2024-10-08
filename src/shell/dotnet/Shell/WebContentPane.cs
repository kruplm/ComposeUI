// /*
//  * Morgan Stanley makes this available to you under the Apache License,
//  * Version 2.0 (the "License"). You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0.
//  *
//  * See the NOTICE file distributed with this work for additional information
//  * regarding copyright ownership. Unless required by applicable law or agreed
//  * to in writing, software distributed under the License is distributed on an
//  * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//  * or implied. See the License for the specific language governing permissions
//  * and limitations under the License.
//  */

using System.Threading.Tasks;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;
using MorganStanley.ComposeUI.ModuleLoader;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Infragistics.Windows.DockManager.Events;
using Infragistics.Windows.DockManager;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Shell.ImageSource;
using MorganStanley.ComposeUI.Shell.Utilities;

namespace MorganStanley.ComposeUI.Shell;

internal class WebContentPane : ContentPane
{
    public WebContentPane(WebContent webContent, IModuleLoader moduleLoader)
    {
        WebContent = webContent;
        _moduleLoader = moduleLoader;

        Header = webContent.ModuleInstance?.Manifest.Name ?? WebContent.Title ?? "New tab";
        Content = webContent.Content;
        Image = WebContent.Icon;

        Closing += Pane_Closing;
        Closed += Pane_Closed;
        WebContent.CloseRequested += WebContent_CloseRequested;
        OptionsMenuOpening += ContentPane_OptionsMenuOpening;
    }

    /*
     
     <igDock:ContentPane Header="Extra Options"
                                            OptionsMenuOpening="ContentPane_OptionsMenuOpening">
                            <!-- The code will add a Save menu item to the pane's options menu so make sure the pane can indicate  if the command is enabled. -->
                            <igDock:ContentPane.CommandBindings>
                                <CommandBinding Command="Save"
                                    CanExecute="CommandBinding_CanExecute"
                                    Executed="CommandBinding_Executed"/>
                            </igDock:ContentPane.CommandBindings>
                        </igDock:ContentPane>
     */

    private void ContentPane_OptionsMenuOpening(object sender, PaneOptionsMenuOpeningEventArgs e)
    {
        ContentPane cp = e.OriginalSource as ContentPane;

        if (e.Items.Count > 0)
        {
            Separator sep = new Separator();
            sep.SetResourceReference(Separator.StyleProperty, XamDockManager.MenuItemSeparatorStyleKey);
            e.Items.Add(sep);
        }


       

        // the OptionsMenuOpening event is raised when you right click on a
        // pane's caption or tab item as well as when you click the window
        // position menu item in the caption and allows you to add/remove
        // items from the menu.
        MenuItem mi = new MenuItem();


        //// todo replace with channel selector menu and channel selection
        CommandBinding saveCmdBinding = new CommandBinding(ApplicationCommands.Save, CommandBinding_Executed, CommandBinding_CanExecute);
        mi.Command = ApplicationCommands.Save; 
        mi.CommandBindings.Add(saveCmdBinding);

        




        //  Binding bindig = new Binding("CommandBinding_CanExecute");
        //  BindingOperations.SetBinding(item, comProp, binding);

        // to have the menu items use the same styling as the default
        // menu items, we'll associate it with the same style that
        // the dockmanager uses for its menu items
        mi.SetResourceReference(MenuItem.StyleProperty, XamDockManager.MenuItemStyleKey);

        e.Items.Add(mi);
    }

    private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        // for the purposes of this demonstration, the save command
        // associated with the binding that raises this event is always
        // enabled
        e.CanExecute = true;
        e.Handled = true;
    }

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // implement save functionality here
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        dlg.ShowDialog();
    }

    private void WebContent_CloseRequested(object? sender, System.EventArgs e)
    {
        ExecuteCommand(ContentPaneCommands.Close);
    }

    private void Pane_Closed(object? sender, PaneClosedEventArgs e)
    {
        WebContent.Dispose();
    }

    private void Pane_Closing(object? sender, PaneClosingEventArgs e)
    {
        if (WebContent.ModuleInstance == null)
            return;

        switch (WebContent.LifetimeEvent)
        {
            case LifetimeEventType.Stopped:
                return;

            case LifetimeEventType.Stopping:
                e.Cancel = true;
                Visibility = System.Windows.Visibility.Hidden;
                return;

            default:
                Visibility = System.Windows.Visibility.Hidden;
                Task.Run(() => _moduleLoader.StopModule(new StopRequest(WebContent.ModuleInstance.InstanceId)));
                return;
        }
    }

    public WebContent WebContent { get; }

    private readonly IModuleLoader _moduleLoader;
}
