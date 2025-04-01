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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordManagerAuth());
        }

        private void FileEncryptor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A File Encryptor application, which means the user will be required to input a specific key of their choice. This will allow both encryption of the file and its sharing with a recipient, as well as later decryption and restoring the file to its original state.", "File Encryptor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CypherChat_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A Cypher Chat application where messages are protected by an encryption key. This allows users to encrypt outgoing messages and decrypt incoming ones directly in the interface. With a temporary messaging system, messages will not be stored on a server; communication happens directly between users. Messages will be deleted within twelve hours or as soon as one of the users leaves the chat. This feature will also have enhanced protection against screen captures and recordings, ensuring complete privacy.", "Cypher Chat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FutureFeatures_Click(object sender, RoutedEventArgs e)
        {
            // Alterna a visibilidade dos botões de FileEncryptor e CypherChat
            FileEncryptor.Visibility = (FileEncryptor.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            CypherChat.Visibility = (CypherChat.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

            // Se os botões foram tornados visíveis, mostramos a mensagem
            if (FileEncryptor.Visibility == Visibility.Visible || CypherChat.Visibility == Visibility.Visible)
            {
                // Pegamos o nome do botão para usar como título
                string buttonTitle = "🚀 Coming Soon!";

                // Agora chamamos o MessageBox com o título correto
                MessageBox.Show("Coming Soon! These will be future features", buttonTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Se os botões forem ocultados, não fazemos nada (não mostramos a mensagem)
        }
    }
}