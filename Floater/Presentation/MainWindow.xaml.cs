/**
 * Floater: A minimalistic floating web browser with cool superpowers :p
 * Developer: Mohammed Sazid Al Rashid
 * LICENSE: MIT | See the LICENSE file for more information
 * https://github.com/sazid/Floater/
 * https://linkedin.com/in/sazidz/
 */

using CefSharp;
using CefSharp.Wpf;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Floater.Data.Entities;
using Floater.Presentation;
using Floater.Utils;

namespace Floater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher();

        public MainWindow()
        {
            InitializeCEFSettings();
            InitializeComponent();

            MainBrowser.LoadingStateChanged += MainBrowser_LoadingStateChanged;
            MainBrowser.MenuHandler = new MenuHandler();
            //MainBrowser.DownloadHandler = new DownloadHandler();

            BindShortcutKeys();
        }

        /// <summary>
        /// This method binds the shortcut keys to the Window
        /// </summary>
        private void BindShortcutKeys()
        {
            // Bind the Ctrl-L to focus on the url bar
            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = Shortcut_Ctrl_L
                }, new KeyGesture(Key.L, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = Shortcut_Ctrl_R
                }, new KeyGesture(Key.R, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = Shortcut_Ctrl_P
                }, new KeyGesture(Key.P, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = Shortcut_Ctrl_B
                }, new KeyGesture(Key.B, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = Shortcut_Ctrl_F
                }, new KeyGesture(Key.F, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                new WindowCommand()
                {
                    ExecuteDelegate = ShowHistory
                }, new KeyGesture(Key.H, ModifierKeys.Control)));
        }

        /// <summary>
        /// Initialize the Chromium Embed Framework and enable/disable various important command line flags
        /// </summary>
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
                if (sender is Rectangle senderRect && senderRect.Tag is Window mainWindow)
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
                    titleLabel.Content = MainBrowser.Title;
                    titleLabel.ToolTip = MainBrowser.Title;

                    if (MainBrowser == null) { }
                    else if (MainBrowser.Title == string.Empty) { }
                    else if (MainBrowser.Address.StartsWith("data")) { }
                    else
                    {
                        // debounce creation of history for 2s, so that it doesn't log history for unnecessary redirects
                        debounceDispatcher.Debounce(2000, _ =>
                        {
                            History.CreateHistory(new History
                            {
                                Url = MainBrowser.Address,
                                Title = MainBrowser.Title,
                                Timestamp = DateTime.Now
                            });
                        });
                    }

                    if (IsUrlYoutubeVideo(MainBrowser.Address))
                        YoutubeModeMenuItem.IsEnabled = true;
                    else
                        YoutubeModeMenuItem.IsEnabled = false;
                }
                catch (Exception exc)
                {
                    //MessageBox.Show(exc.ToString());
                }
            });
            
        }

        private bool IsUrlYoutubeVideo(string url)
        {
            return url.StartsWith("https://www.youtube.com/watch?v=");
        }

        /// <summary>
        /// Loads only the YouTube player removing all kinds of distractions
        /// </summary>
        /// <param name="url"></param>
        private void LoadYoutubeMode(string url)
        {
            // convert url to youtube embed
            url = url.Replace("watch?v=", "embed/");

            MainBrowser.LoadHtml($@"
<!DOCTYPE html>
<html>
  <body style=""overflow: hidden;"">
    <iframe id=""vid"" src=""{url}"" frameborder=""0""></iframe>

    <script type=""text/javascript"">
        var el = document.getElementById(""vid"");
        el.style.width=window.innerWidth-15+'px';
        el.style.height=window.innerHeight-15+'px';
    </script>
  </body>
</html>");
        }

        /// <summary>
        /// Dummy class for disabling context menu, we're doing everything through the top level menu
        /// </summary>
        class MenuHandler : IContextMenuHandler
        {
            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model) => model.Clear();

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) => false;

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame) { }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback) => false;
        }

        /// <summary>
        /// Use regular expression to validate a given string as url.
        /// It also tries to convert the given string into a valid http url for cases like: google.com which doesn't include http://
        /// at the start
        /// </summary>
        /// <param name="s">string to validate</param>
        /// <param name="resultUri">(Possibly converted) Uri if successful</param>
        /// <returns></returns>
        private static bool ValidHttpUrl(string s, out Uri resultUri)
        {
            if (!Regex.IsMatch(s, @"^https?:\/\/.+\..+", RegexOptions.IgnoreCase))
                if (Regex.IsMatch(s, @".+\...+", RegexOptions.IgnoreCase))
                    s = "http://" + s;

            if (Uri.TryCreate(s, UriKind.Absolute, out resultUri))
                return (resultUri.Scheme == Uri.UriSchemeHttp ||
                        resultUri.Scheme == Uri.UriSchemeHttps);

            return false;
        }

        private void urlTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Key)
            {
                case Key.Return:
                {
                    MainBrowser.Stop();

                    var result = ValidHttpUrl(urlTextbox.Text, out var uriResult);
                    if (!result)
                    {
                        MainBrowser.Address = "https://google.com/search?q=" + urlTextbox.Text;
                    }
                    else
                    {
                        MainBrowser.Address = uriResult?.AbsoluteUri;
                    }

                    MainBrowser.Focus();
                    break;
                }
                case Key.Escape:
                    MainBrowser.Focus();
                    break;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var move = content_pane;
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
            if (HideUiCheckBox.IsChecked)
                ShowUIElements();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (HideUiCheckBox.IsChecked)
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
            Topmost = FloatCheckBox.IsChecked;
        }
        
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Developer: Mohammed Sazid Al Rashid\nContact: sazidozon@gmail.com\nhttps://github.com/sazid/", "Floater");

        private void HistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowHistory();
        }

        private void ShowHistory()
        {
            Hide();
            HistoryView historyView = new HistoryView
            {
                Topmost = Topmost,
                mainWindow = this
            };
            historyView.ShowDialog();
            Show();
        }

        private void ScreenRecordMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"captura\captura.exe");
            }
            catch (Exception)
            {
                MessageBox.Show("Error. Screen recorder not found");
            }
        }

        private void ReloadMenuItem_Click(object sender, RoutedEventArgs e) => MainBrowser.Reload();

        private void YoutubeModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadYoutubeMode(MainBrowser.Address);
        }

        private void ProjectPageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainBrowser.Address = "https://github.com/sazid/Floater/";
        }

        private void PrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainBrowser.Print();
        }

        private void Shortcut_Ctrl_L()
        {
            urlTextbox.Focus();
            urlTextbox.Select(0, urlTextbox.Text.Length);
        }

        private void Shortcut_Ctrl_R()
        {
            MainBrowser.Reload();
        }

        private void Shortcut_Ctrl_P()
        {
            MainBrowser.Print();
        }

        private void Shortcut_Ctrl_B()
        {
            if (MainBrowser.CanGoBack)
                MainBrowser.Back();
        }

        private void Shortcut_Ctrl_F()
        {
            if (MainBrowser.CanGoForward)
                MainBrowser.Forward();
        }
    }

}
