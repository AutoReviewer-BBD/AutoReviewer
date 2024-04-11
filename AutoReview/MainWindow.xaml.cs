using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoReview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            RepoComboBox.Dispatcher.Invoke(() => {
                RepoComboBox.ItemsSource = new List<string> { "AutoReviewer-BBD/AutoReviewer" };
            });

            CreatePrButton.Dispatcher.Invoke(async () => {
                CreatePrButton.IsEnabled = false;
            });
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GitHubAuther.SetAuthLink(AuthButton, More);
        }

        private void RepoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BranchComboBox.Dispatcher.Invoke(async () => {
                BranchComboBox.ItemsSource = await GitHubAPI.GetRepoBanches(
                        RepoComboBox.SelectedItem.ToString().Split("/")[0],
                        RepoComboBox.SelectedItem.ToString().Split("/")[1]
                        );
            });
        }
        private void BranchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TypeComboBox.Dispatcher.Invoke(async () => {
                TypeComboBox.ItemsSource = new List<string> { "Frontend", "Backend", "Infrastructure", "Database" };
            });
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreatePrButton.Dispatcher.Invoke(async () => {
                CreatePrButton.IsEnabled = true;
            });
        }

        private async Task Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string message = await GitHubAPI.CreatePR(
                    RepoComboBox.SelectedItem.ToString().Split("/")[0],
                    RepoComboBox.SelectedItem.ToString().Split("/")[1],
                    prNameTextBox.Text,
                    BranchComboBox.SelectedItem.ToString(),
                    TypeComboBox.SelectedItem.ToString()
                );

            prPostedText.Dispatcher.Invoke(() => {
                prPostedText.Text = message;
            });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button_ClickAsync(sender, e);
        }
    }
}