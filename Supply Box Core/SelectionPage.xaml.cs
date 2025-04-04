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
    public partial class SelectionPage : Page
    {
        // Variável para controlar o estado de visibilidade dos botões
        private bool featuresVisible = false;

        public SelectionPage()
        {
            InitializeComponent();
        }

        // Método chamado ao clicar no botão PasswordManager
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navega para a página de autenticação do Password Manager
            NavigationService.Navigate(new PasswordManagerAuth());
        }

        // Método chamado ao clicar no botão FileEncryptor
        private void FileEncryptor_Click(object sender, RoutedEventArgs e)
        {
            // Exibe uma mensagem informativa sobre a funcionalidade de encriptação de ficheiros
            MessageBox.Show("The File Encryptor application requires the user to input a specific key of their choice. This will allow both encryption of the file and its sharing with a recipient, as well as later decryption and restoring the file to its original state.", "File Encryptor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Método chamado ao clicar no botão CypherChat
        private void CypherChat_Click(object sender, RoutedEventArgs e)
        {
            // Exibe uma mensagem informativa sobre o sistema Cypher Chat
            MessageBox.Show("The Cypher Chat application protects messages with an encryption key. This allows users to encrypt outgoing messages and decrypt incoming ones directly in the interface. With a temporary messaging system, messages will not be stored on a server; communication happens directly between users. Messages will be deleted within twelve hours or as soon as one of the users leaves the chat. This feature will also have enhanced protection against screen captures and recordings, ensuring complete privacy.", "Cypher Chat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Método chamado ao clicar no botão FutureFeatures
        private void FutureFeatures_Click(object sender, RoutedEventArgs e)
        {
            // Alterna a visibilidade dos botões FileEncryptor e CypherChat
            FileEncryptor.Visibility = (FileEncryptor.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            CypherChat.Visibility = (CypherChat.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

            // Se os botões forem tornados visíveis, exibe uma mensagem informativa
            if (FileEncryptor.Visibility == Visibility.Visible || CypherChat.Visibility == Visibility.Visible)
            {
                // Define o título da mensagem
                string buttonTitle = "🚀 Coming Soon!";

                // Exibe a mensagem informando que estas funcionalidades serão adicionadas no futuro
                MessageBox.Show("Coming Soon! These will be future features.", buttonTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Se os botões forem ocultados, não exibe nenhuma mensagem
        }
    }
}