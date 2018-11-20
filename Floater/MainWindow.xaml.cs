using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Floater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainBrowser.LoadingStateChanged += MainBrowser_LoadingStateChanged;
            //MainBrowser.DownloadHandler = new DownloadHandler();
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

        private void MainBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    urlTextbox.Text = MainBrowser.Address;
                    titleLabel.Content = MainBrowser.Title;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            });
            
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
            content_pane.RowDefinitions[1].Height = new GridLength(0);
        }

        private void ShowUIElements()
        {
            content_pane.RowDefinitions[0].Height = new GridLength(32);
            content_pane.RowDefinitions[1].Height = new GridLength(28);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ShowUIElements();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            HideUiElements();
        }
    }

}
