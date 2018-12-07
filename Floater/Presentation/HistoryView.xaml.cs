using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Floater.Data.Entities;
using Floater.Utils;

namespace Floater
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : Window
    {
        private DebounceDispatcher debounceDispatcher = new DebounceDispatcher();

        public HistoryView()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void LoadHistory(string filter = null)
        {
            historyDataGrid.ItemsSource = History.GetHistories(filter);
        }

        private void historyDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Title")
                e.Column.Width = 200;
            else if (e.PropertyName == "Url")
                e.Column.Width = 400;
        }

        private void searchHistoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // debounce user input otherwise for 300ms, it'll slow down the UI and make the app unresponsive
            debounceDispatcher.Debounce(300, _ =>
            {
                LoadHistory(searchHistoryTextBox.Text);
            });
        }

        private void clearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to clear the history?", "Clear History Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                History.DeleteHistory();
                LoadHistory();
                searchHistoryTextBox.Text = string.Empty;
            }
        }
    }
}
