using System.Windows;
using TTS2Voice.ViewModel;

namespace TTS2Voice.Views
{
    /// <summary>
    /// Lógica interna para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel();
        }
    }
}
