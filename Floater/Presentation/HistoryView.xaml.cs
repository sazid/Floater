using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Floater.Data.Entities;
using Floater.Utils;
using System.Threading;
using System.Collections.Generic;

namespace Floater.Presentation
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : Window
    {
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher();
        public MainWindow mainWindow;
        private readonly SynchronizationContext synchronizationContext;

        public HistoryView()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            new Thread(_ =>
            {
                Thread.CurrentThread.IsBackground = true;
                LoadHistory();
            }).Start();
        }

        private void LoadHistory(string filter = null)
        {
            List<History> data = History.GetHistories(filter);

            HistoryDataGrid.Dispatcher.Invoke(
                new Action(() => HistoryDataGrid.ItemsSource = data));
        }

        private void HistoryDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Title":
                    e.Column.Width = 200;
                    break;
                case "Url":
                    e.Column.Width = 400;
                    break;
            }
        }

        // debounce user input otherwise for 300ms, it'll slow down the UI and make the app unresponsive
        private void SearchHistoryTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            debounceDispatcher.Debounce(300, _ => LoadHistory(SearchHistoryTextBox.Text));

        private void ClearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(
                "Are you sure you want to clear the history?",
                "Clear History Confirmation",
                MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            History.DeleteHistory();
            LoadHistory();
            SearchHistoryTextBox.Text = string.Empty;
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (mainWindow == null || !(HistoryDataGrid.SelectedItem is History h)) return;

            mainWindow.MainBrowser.Address = h.Url;
            mainWindow.Show();
            Close();
        }
    }
}
