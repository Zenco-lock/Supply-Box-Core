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
using Windows.Security.Credentials.UI;

namespace Supply_Box_Core
{
    public partial class PasswordManagerAuth : Page
    {
        public PasswordManagerAuth()
        {
            InitializeComponent();

            // Inicia automaticamente o processo de autenticação com o Windows Hello ao carregar a página
            AuthenticateWithWindowsHello();
        }

        // Método assíncrono responsável por autenticar o utilizador através do Windows Hello
        private async void AuthenticateWithWindowsHello()
        {
            try
            {
                // Atualiza a mensagem de interface para informar o utilizador
                AuthMessageText.Text = "Authenticating with Windows Hello...";

                // Inicia o processo de verificação de identidade através do Windows Hello
                var result = await UserConsentVerifier.RequestVerificationAsync("Please verify your identity to proceed.");

                if (result == UserConsentVerificationResult.Verified)
                {
                    // Autenticação bem-sucedida: abre a janela principal do gestor de palavras-passe
                    PasswordManagerMainWindow mainWindow = new PasswordManagerMainWindow();
                    mainWindow.Show();

                    // Fecha a janela atual (autenticação)
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // Autenticação falhada: atualiza a interface e mostra o botão de nova tentativa
                    AuthMessageText.Text = "Authentication failed. Please try again.";
                    LoginButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro na autenticação, mostra a mensagem de erro e permite nova tentativa
                AuthMessageText.Text = "Error: " + ex.Message;
                LoginButton.Visibility = Visibility.Visible;
            }
        }

        // Evento associado ao clique no botão de login
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Esconde o botão durante nova tentativa de autenticação
            LoginButton.Visibility = Visibility.Collapsed;

            // Recomeça o processo de autenticação
            AuthenticateWithWindowsHello();
        }
    }
}