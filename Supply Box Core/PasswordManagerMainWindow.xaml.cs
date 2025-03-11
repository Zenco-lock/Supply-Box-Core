using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace Supply_Box_Core
{
    public partial class PasswordManagerMainWindow : Page
    {
        // Define o caminho para armazenar as credenciais: dentro do LocalApplicationData do utilizador
        private readonly string dataFolder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsSecurityCache");

        public PasswordManagerMainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(dataFolder); // Cria a pasta se não existir
            LoadCredentials(); // Carrega as credenciais guardadas
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextbox.Text;
            string password = PasswordTextbox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha todos os campos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gera um nome de ficheiro único para a credencial
            string fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat");
            // Encripta os dados no formato "username|password"
            string encryptedData = EncryptData($"{username}|{password}");

            // Guarda o ficheiro com os dados encriptados
            File.WriteAllText(fileName, encryptedData);

            MessageBox.Show("Credencial guardada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpa os inputs
            UserTextbox.Clear();
            PasswordTextbox.Clear();

            LoadCredentials(); // Atualiza a lista de credenciais
        }

        private string EncryptData(string plainText)
        {
            // Chave e IV fixos (para este protótipo)
            byte[] key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");   // 32 bytes para AES-256
            byte[] iv = Encoding.UTF8.GetBytes("A1B2C3D4E5F60708");                    // 16 bytes

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptData(string encryptedText)
        {
            byte[] key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            byte[] iv = Encoding.UTF8.GetBytes("A1B2C3D4E5F60708");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        private void LoadCredentials()
        {
            // Limpa o contêiner para evitar duplicação
            CredentialStackPanel.Children.Clear();

            // Obtém os ficheiros de credencial e os ordena para que os mais recentes fiquem no topo
            var files = Directory.GetFiles(dataFolder, "*.dat");
            Array.Sort(files, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));

            foreach (string file in files)
            {
                string encryptedData = File.ReadAllText(file);
                string decryptedData = DecryptData(encryptedData);

                if (!string.IsNullOrEmpty(decryptedData))
                {
                    string[] parts = decryptedData.Split('|');
                    if (parts.Length == 2)
                    {
                        string username = parts[0];
                        string password = parts[1];

                        // Cria o cartão (Border) com as mesmas dimensões e estilo do formulário superior
                        Border card = new Border
                        {
                            Background = Brushes.White,
                            CornerRadius = new CornerRadius(10),
                            Width = 600,
                            Height = 200,
                            Margin = new Thickness(20),
                            Padding = new Thickness(10),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        // Cria um StackPanel horizontal para imitar o layout do formulário superior
                        StackPanel cardPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        // Ícone (retângulo cinzento) igual ao da área superior
                        Rectangle iconRect = new Rectangle
                        {
                            Fill = Brushes.Gray,
                            Width = 60,
                            Height = 60,
                            Margin = new Thickness(10)
                        };

                        // StackPanel vertical para os textos
                        StackPanel textPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(20, 0, 0, 0)
                        };

                        // Rótulos e valores (User e Password)
                        TextBlock userLabel = new TextBlock
                        {
                            Text = "User:",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5)
                        };
                        TextBlock userValue = new TextBlock
                        {
                            Text = username,
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5, 0, 5, 10)
                        };

                        TextBlock passLabel = new TextBlock
                        {
                            Text = "Password:",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5, 10, 5, 0)
                        };
                        TextBlock passValue = new TextBlock
                        {
                            Text = "********",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5)
                        };

                        // Adiciona os rótulos e valores ao textPanel
                        textPanel.Children.Add(userLabel);
                        textPanel.Children.Add(userValue);
                        textPanel.Children.Add(passLabel);
                        textPanel.Children.Add(passValue);

                        // Adiciona o ícone e o textPanel ao cardPanel
                        cardPanel.Children.Add(iconRect);
                        cardPanel.Children.Add(textPanel);

                        // Cria o botão de copiar com as mesmas dimensões e estilo do botão SUBMIT
                        Button copyButton = new Button
                        {
                            Content = "📋",
                            Width = 80,
                            Height = 73,
                            Background = Brushes.Gray,
                            Foreground = Brushes.White,
                            FontSize = 16,
                            FontFamily = new FontFamily("Bahnschrift SemiBold"),
                            FontWeight = FontWeights.Bold,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(20, 0, 20, -20)
                        };
                        copyButton.Click += (s, e) => Clipboard.SetText(password);

                        // Adiciona o botão de copiar ao cardPanel (ao final, mantendo o layout horizontal)
                        cardPanel.Children.Add(copyButton);

                        // Define o conteúdo do cartão como o cardPanel
                        card.Child = cardPanel;

                        // Insere o cartão no contêiner, de forma que os itens mais recentes apareçam no topo
                        CredentialStackPanel.Children.Insert(0, card);
                    }
                }
            }
        }
    }
}