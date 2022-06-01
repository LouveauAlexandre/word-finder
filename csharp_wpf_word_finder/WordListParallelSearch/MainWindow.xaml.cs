using System.Windows;

using System.Windows.Input;


namespace WordListParallelSearch
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel(new WordList().List);
            DataContext = viewModel;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                viewModel.SearchWordList();
            }
        }
    }
}
