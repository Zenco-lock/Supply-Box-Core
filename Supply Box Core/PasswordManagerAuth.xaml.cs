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
    /// Interaction logic for PasswordManagerAuth.xaml
    /// </summary>
    public partial class PasswordManagerAuth : Page
    {
        public PasswordManagerAuth()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputText = AuthPasswordBox.Password;

            if (inputText == "1234")
            {
                // Criar e exibir a nova janela (PasswordManagerMainWindow)
                PasswordManagerMainWindow mainWindow = new PasswordManagerMainWindow();
                mainWindow.Show();

                // Fechar a janela atual (PasswordManagerAuth)
                Window.GetWindow(this)?.Close();
            }
            else
            {
                MessageBox.Show("Error: Wrong key!", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}