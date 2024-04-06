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
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GitHubAuther.SetAuthLink(AuthButton, More, pfpImage, BranchesComboBox);
        }

        private void BranchesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GitHubAPI.PopulatePRTypeSelection(prTypeComboBox);
        }
    }
}