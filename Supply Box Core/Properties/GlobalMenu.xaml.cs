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
        public GlobalMenu()
        {
            InitializeComponent();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se a janela principal possui um Frame para navegar
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // Navega para a SelectionPage dentro do Frame
                mainWindow.MainFrame.Navigate(new SelectionPage());
            }
        }
    }
}