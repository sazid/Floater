/**
 * Floater: A minimalistic floating web browser with cool superpowers :p
 * Copyright (c) Mohammed Sazid Al Rashid
 * https://github.com/sazid/
 * https://linkedin.com/in/sazidz/
 */

using CefSharp;
using CefSharp.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Floater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeCEFSettings();
            InitializeComponent();

            MainBrowser.LoadingStateChanged += MainBrowser_LoadingStateChanged;
            MainBrowser.MenuHandler = new MenuHandler();
            //MainBrowser.DownloadHandler = new DownloadHandler();
        }

        private void InitializeCEFSettings()
        {
            CefSettings settings = new CefSettings
            {
                CachePath = "cache"  //always set the cachePath, else this wont work
            };

            //settings.SetOffScreenRenderingBestPerformanceArgs();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
            //settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1"); //Enable WebRTC
            settings.CefCommandLineArgs.Add("no-proxy-server", "1"); //Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
            settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1"); //Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches


            //add an if statement to initialize the settings once. else the app is going to crash
            if (Cef.IsInitialized == false)
                Cef.Initialize(settings);
        }

        #region ResizeWindows
        bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle senderRect)
            {
                ResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (ResizeInProcess)
            {
                Rectangle senderRect = sender as Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                            mainWindow.Width = width;
                    }
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        width -= 5;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;
                        if (width > 0)
                        {
                            mainWindow.Width = width;
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0)
                            mainWindow.Height = height;
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;
                        if (height > 0)
                        {
                            mainWindow.Height = height;
                        }
                    }
                }
            }
        }
        #endregion

        private void MainBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    urlTextbox.Text = MainBrowser.Address;
                    //titleLabel.Content = MainBrowser.Title;

                    if (IsUrlYoutubeVideo(MainBrowser.Address))
                        YoutubeModeMenuItem.IsEnabled = true;
                    else
                        YoutubeModeMenuItem.IsEnabled = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            });
            
        }

        private bool IsUrlYoutubeVideo(string url)
        {
            if (url.StartsWith("https://www.youtube.com/watch?v="))
                return true;
            return false;
        }

        private void LoadYoutubeMode(string url)
        {
            // convert url to youtube embed
            url = url.Replace("watch?v=", "embed/");

            MainBrowser.LoadHtml(string.Format(@"
<!DOCTYPE html>
<html>
  <body style=""overflow: hidden;"">
    <iframe id=""vid"" src=""{0}"" frameborder=""0""></iframe>

    <script type=""text/javascript"">
        var el = document.getElementById(""vid"");
        el.style.width=window.innerWidth-15+'px';
        el.style.height=window.innerHeight-15+'px';
    </script>
  </body>
</html>", url));
        }

        class MenuHandler : IContextMenuHandler
        {
            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) => model.Clear();

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) => false;

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) { }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) => false;
        }

        private void urlTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainBrowser.Address = urlTextbox.Text;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = content_pane as Grid;
            var win = GetWindow(move);
            win.DragMove();
        }

        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = e.NewValue;
        }

        private void HideUiElements()
        {
            content_pane.RowDefinitions[0].Height = new GridLength(0);
        }

        private void ShowUIElements()
        {
            content_pane.RowDefinitions[0].Height = new GridLength(32);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (HideUiCheckBox.IsChecked.GetValueOrDefault())
                ShowUIElements();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (HideUiCheckBox.IsChecked.GetValueOrDefault())
                HideUiElements();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void FloatCheckBox_Click(object sender, RoutedEventArgs e)
        {
            Topmost = FloatCheckBox.IsChecked.GetValueOrDefault();
        }
        
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Developer: Mohammed Sazid Al Rashid\nContact: sazidozon@gmail.com\nhttps://github.com/sazid/", "Floater");

        private void HistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open a new window here or show something to work with
            // Maybe, fillup the submenus with history items
        }

        private void ScreenRecordMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadMenuItem_Click(object sender, RoutedEventArgs e) => MainBrowser.Reload();

        private void YoutubeModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadYoutubeMode(MainBrowser.Address);
        }
    }

}
