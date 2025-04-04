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
    public partial class GlobalMenu : UserControl
    {
        // Construtor do controlo GlobalMenu
        public GlobalMenu()
        {
            InitializeComponent(); // Inicializa os componentes definidos no ficheiro XAML
        }

        // Evento que é executado quando o botão de menu (☰) é clicado
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se a janela principal atual é do tipo MainWindow
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // Se for, utiliza o Frame da MainWindow para navegar para a página SelectionPage
                mainWindow.MainFrame.Navigate(new SelectionPage());
            }
        }
    }
}