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

            //Opacity = 0.8;
        }

        //private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var move = content_pane as System.Windows.Controls.StackPanel;
        //    var win = Window.GetWindow(move);
        //    win.DragMove();
        //}

        private void MainBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    urlTextbox.Text = MainBrowser.Address;
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            //if (MainBrowser.CanGoBack)
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
