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

                    //if (IsUrlYoutubeVideo(MainBrowser.Address))
                    //{
                    //    LoadYoutubeMode(MainBrowser.Address)
                    //}
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
            MainBrowser.LoadHtml(@"
<!DOCTYPE html>
<html>
<body>

<div id='player'></div>

 <script>
      // 2. This code loads the IFrame Player API code asynchronously.
      var tag = document.createElement('script');

      tag.src = 'https://www.youtube.com/iframe_api';
      var firstScriptTag = document.getElementsByTagName('script')[0];
            firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

            // 3. This function creates an <iframe> (and YouTube player)
            //    after the API code downloads.
            var player;
            function onYouTubeIframeAPIReady()
            {
                player = new YT.Player('player', {
          height: '390',
          width: '640',
          videoId: 'M7lc1UVf-VE',
          events:
            {
                'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
          }
        });
      }

    // 4. The API will call this function when the video player is ready.
    function onPlayerReady(event)
    {
        event.target.playVideo();
    }
    </script>

</body>
</html>
            ");
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
            //content_pane.RowDefinitions[1].Height = new GridLength(0);
        }

        private void ShowUIElements()
        {
            content_pane.RowDefinitions[0].Height = new GridLength(32);
            //content_pane.RowDefinitions[1].Height = new GridLength(28);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ShowUIElements();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            HideUiElements();
        }

        class MenuHandler : IContextMenuHandler
        {
            //private const int ShowDevTools = 26501;
            private const int GoBack = 26502;
            private const int GoForward = 26503;
            private const int Reload = 26504;

            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                //To disable the menu then call clear
                model.Clear();

                //Removing existing menu item
                //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

                //Add new custom menu items
                //model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
                model.AddItem((CefMenuCommand)GoBack, "Back");
                model.AddItem((CefMenuCommand)GoForward, "Forward");
                model.AddItem((CefMenuCommand)Reload, "Reload");
            }

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                switch ((int)commandId)
                {
                    //case ShowDevTools:
                    //    browser.ShowDevTools();
                    //    break;
                    case GoBack:
                        if (browser.CanGoBack)
                            browser.GoBack();
                        break;
                    case GoForward:
                        if (browser.CanGoForward)
                            browser.GoForward();
                        break;
                    case Reload:
                        browser.Reload();
                        break;
                }
                return false;
            }

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
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
    }

}
