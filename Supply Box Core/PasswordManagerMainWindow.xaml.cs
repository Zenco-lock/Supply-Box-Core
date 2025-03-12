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
using System.Collections.ObjectModel;


namespace Supply_Box_Core
{
    public partial class PasswordManagerMainWindow : Window
    {
        // Use ObservableCollection para atualização automática da interface
        public ObservableCollection<dynamic> Credentials { get; set; } = new ObservableCollection<dynamic>();

        private readonly string dataFolder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsSecurityCache");

        public PasswordManagerMainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Directory.CreateDirectory(dataFolder);
            LoadCredentials();
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

            if (editingCredentialFilePath != null)
            {
                // Atualiza o arquivo existente
                string encryptedData = EncryptData($"{username}|{password}");
                File.WriteAllText(editingCredentialFilePath, encryptedData);
                editingCredentialFilePath = null;
                MessageBox.Show("Credencial atualizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat");
                string encryptedData = EncryptData($"{username}|{password}");
                File.WriteAllText(fileName, encryptedData);
            }

            UserTextbox.Clear();
            PasswordTextbox.Clear();

            // Recarrega as credenciais da pasta
            LoadCredentials();
        }

        private void LoadCredentials()
        {
            Credentials.Clear();
            foreach (string file in Directory.GetFiles(dataFolder, "*.dat"))
            {
                string encryptedData = File.ReadAllText(file);
                string decryptedData = DecryptData(encryptedData);
                if (!string.IsNullOrEmpty(decryptedData))
                {
                    string[] parts = decryptedData.Split('|');
                    if (parts.Length == 2)
                    {
                        // Adiciona cada credencial, incluindo o FilePath para referência
                        Credentials.Add(new { Username = parts[0], Password = parts[1], FilePath = file });
                    }
                }
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext;
            if (item != null)
            {
                Clipboard.SetText(item.Password);
            }
        }

        // Declaração de variáveis de edição (na classe, fora dos métodos)
        private string editingCredentialFilePath = null;
        private Button currentEditingButton = null;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext;
            if (item == null)
                return;

            // Se já estivermos em modo de edição...
            if (editingCredentialFilePath != null)
            {
                // Se o mesmo botão foi clicado, cancelar a edição
                if (currentEditingButton == btn)
                {
                    editingCredentialFilePath = null;
                    currentEditingButton.Content = "✏"; // Volta ao ícone de edição
                    currentEditingButton = null;
                    UserTextbox.Clear();
                    PasswordTextbox.Clear();
                    MessageBox.Show("Edição cancelada", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Se outro botão foi clicado enquanto já há um item em edição:
                    if (currentEditingButton != null)
                    {
                        currentEditingButton.Content = "✏"; // Reseta o botão que estava em edição
                    }
                    // Inicia a edição para o novo item
                    UserTextbox.Text = item.Username;
                    PasswordTextbox.Password = item.Password;
                    editingCredentialFilePath = item.FilePath;
                    btn.Content = "X"; // Atualiza o botão atual para mostrar "X"
                    currentEditingButton = btn;
                }
            }
            else
            {
                // Nenhum item está sendo editado: inicia a edição
                UserTextbox.Text = item.Username;
                PasswordTextbox.Password = item.Password;
                editingCredentialFilePath = item.FilePath;
                btn.Content = "X";
                currentEditingButton = btn;
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext;
            if (item != null)
            {
                if (File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath);
                }
                // Remove o item da coleção
                Credentials.Remove(item);
            }
        }

        // As funções de encriptação e desencriptação seguem como antes.
        private string EncryptData(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            byte[] iv = Encoding.UTF8.GetBytes("A1B2C3D4E5F60708");

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