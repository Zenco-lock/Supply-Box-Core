using System;
using System.Collections.Generic;
using System.IO;                        // Necessário para File, Directory, MemoryStream, StreamReader, StreamWriter
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
using System.Security.Cryptography;      // Necessário para Aes, CipherMode, PaddingMode, ICryptoTransform, CryptoStream
using IOPath = System.IO.Path;


namespace Supply_Box_Core
{
    public partial class PasswordManagerMainWindow : Page
    {
        // Define o caminho para armazenar as credenciais: pasta no LocalApplicationData
        private readonly string dataFolder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsSecurityCache");
        private List<dynamic> credentials = new List<dynamic>();

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

            string fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat");
            string encryptedData = EncryptData($"{username}|{password}");
            File.WriteAllText(fileName, encryptedData);

            // Adiciona a nova credencial no início da lista
            credentials.Insert(0, new { Username = username, Password = password });
            RefreshCredentialList();

            UserTextbox.Clear();
            PasswordTextbox.Clear();
        }

        private void LoadCredentials()
        {
            credentials.Clear();
            foreach (string file in Directory.GetFiles(dataFolder, "*.dat"))
            {
                string encryptedData = File.ReadAllText(file);
                string decryptedData = DecryptData(encryptedData);
                if (!string.IsNullOrEmpty(decryptedData))
                {
                    string[] parts = decryptedData.Split('|');
                    if (parts.Length == 2)
                    {
                        credentials.Add(new { Username = parts[0], Password = parts[1] });
                    }
                }
            }
            RefreshCredentialList();
        }

        private void RefreshCredentialList()
        {
            CredentialList.ItemsSource = null;
            CredentialList.ItemsSource = credentials;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            // O DataContext do botão é o item de dados (o objeto anônimo com Username e Password)
            dynamic item = btn.DataContext;
            if (item != null)
            {
                Clipboard.SetText(item.Password);
            }
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
    }
}