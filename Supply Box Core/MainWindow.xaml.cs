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

namespace Supply_Box_Core
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Construtor da janela principal
        public MainWindow()
        {
            InitializeComponent();         // Inicializa os componentes visuais definidos em MainWindow.xaml
            InitializeMainWindow();        // Chama o método que inicia a lógica da janela principal
        }

        // Método responsável por iniciar a janela com o ecrã de carregamento
        private async void InitializeMainWindow()
        {
            // Navega inicialmente para a página SplashScreen (ecrã de carregamento)
            MainFrame.Navigate(new SplashScreenPage());

            // Aguarda alguns segundos antes de mudar para a página de seleção
            await ShowSelectionPageAfterDelay();
        }

        // Método assíncrono que aguarda 5 segundos e depois navega para a página de seleção
        public async Task ShowSelectionPageAfterDelay()
        {
            await Task.Delay(5000); // Espera de 5 segundos (5000 milissegundos)
            MainFrame.Navigate(new SelectionPage()); // Navega para a página de seleção
        }
    }
}