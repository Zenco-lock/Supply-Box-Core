using System;
using System.Collections.Generic;
using System.IO; // Necessário para manipulação de arquivos (File, Directory)
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
using System.Windows.Shapes;
using System.Security.Cryptography; // Necessário para criptografia AES
using IOPath = System.IO.Path;
using System.Collections.ObjectModel;

namespace Supply_Box_Core
{
    public partial class PasswordManagerMainWindow : Window
    {
        // ObservableCollection permite que a interface se atualize automaticamente quando os itens mudam
        // Isso facilita a sincronização entre a lógica de dados e a interface do usuário
        public ObservableCollection<dynamic> Credentials { get; set; } = new ObservableCollection<dynamic>();

        // Caminho para o diretório onde as credenciais serão armazenadas localmente
        // O diretório é armazenado na pasta de dados locais do sistema
        private readonly string dataFolder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsSecurityCache");

        public PasswordManagerMainWindow()
        {
            InitializeComponent();
            this.DataContext = this; // Vincula os dados à interface para facilitar o binding
            Directory.CreateDirectory(dataFolder); // Cria o diretório para armazenar as credenciais, caso não exista
            LoadCredentials(); // Carrega as credenciais salvas ao inicializar a aplicação
        }

        // Evento chamado quando o botão "SUBMIT" é clicado para salvar ou editar uma credencial
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string appName = AppNameTextbox.Text; // Nome do aplicativo
            string username = UserTextbox.Text; // Nome de usuário
            string password = PasswordTextbox.Password; // Senha

            // Verifica se todos os campos foram preenchidos
            if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Se estiver no modo de edição (um item já foi selecionado para ser editado)
            if (editingCredentialFilePath != null)
            {
                // Criptografa e salva a credencial editada no arquivo existente
                string encryptedData = EncryptData($"{appName}|{username}|{password}");
                File.WriteAllText(editingCredentialFilePath, encryptedData);

                // Limpa os campos após salvar e recarrega a lista de credenciais
                AppNameTextbox.Clear();
                UserTextbox.Clear();
                PasswordTextbox.Clear();
                LoadCredentials();

                // Limpa o estado de edição
                editingCredentialFilePath = null;
            }
            else
            {
                // Caso contrário, cria uma nova credencial (novo arquivo)
                string encryptedData = EncryptData($"{appName}|{username}|{password}");
                string fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat"); // Nome do arquivo gerado com um GUID único
                File.WriteAllText(fileName, encryptedData);

                // Limpa os campos após salvar e recarrega a lista de credenciais
                AppNameTextbox.Clear();
                UserTextbox.Clear();
                PasswordTextbox.Clear();
                LoadCredentials();
            }
        }

        // Carrega as credenciais salvas no diretório de dados
        private void LoadCredentials()
        {
            Credentials.Clear(); // Limpa a lista antes de recarregar
            foreach (string file in Directory.GetFiles(dataFolder, "*.dat")) // Obtém todos os arquivos .dat no diretório
            {
                string encryptedData = File.ReadAllText(file); // Lê os dados criptografados do arquivo
                string decryptedData = DecryptData(encryptedData); // Descriptografa os dados
                if (!string.IsNullOrEmpty(decryptedData))
                {
                    // Se a descriptografia for bem-sucedida, divide os dados em três partes: appName, username, e password
                    string[] parts = decryptedData.Split('|');
                    if (parts.Length == 3) // Confirma que todos os dados foram recuperados
                    {
                        // Adiciona o item à coleção de credenciais para ser exibido na interface
                        Credentials.Add(new { AppName = parts[0], Username = parts[1], Password = parts[2], FilePath = file });
                    }
                }
            }
        }

        // Evento chamado quando o botão de copiar a senha é clicado
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext; // Obtém o item (credencial) associado ao botão
            if (item != null)
            {
                Clipboard.SetText(item.Password); // Copia a senha para a área de transferência
            }
        }

        // Variáveis para gerenciar o estado de edição de uma credencial
        private string editingCredentialFilePath = null;
        private Button currentEditingButton = null;

        // Evento chamado quando o botão de editar é clicado
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext; // Obtém os dados associados ao botão
            if (item == null)
                return;

            // Caso já haja um item sendo editado
            if (editingCredentialFilePath != null)
            {
                // Se o mesmo botão foi clicado, cancela a edição (limpa campos e reseta estado)
                if (currentEditingButton == btn)
                {
                    editingCredentialFilePath = null;
                    currentEditingButton.Content = "✏"; // Restaura o ícone de edição
                    currentEditingButton = null;
                    UserTextbox.Clear();
                    PasswordTextbox.Clear();
                    AppNameTextbox.Clear(); // Limpa o campo do nome do aplicativo
                    AppNameTextbox.IsReadOnly = true; // Impede a edição do nome do aplicativo
                    MessageBox.Show("Edição cancelada", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Se outro botão foi clicado enquanto já há um item em edição, reinicia a edição para o novo item
                    if (currentEditingButton != null)
                    {
                        currentEditingButton.Content = "✏"; // Reseta o botão de edição anterior
                    }
                    // Inicia a edição para o novo item
                    UserTextbox.Text = item.Username;
                    PasswordTextbox.Password = item.Password;
                    AppNameTextbox.Text = item.AppName; // Atribui o nome do aplicativo para edição
                    editingCredentialFilePath = item.FilePath; // Armazena o caminho do arquivo da credencial
                    btn.Content = "X"; // Muda o ícone do botão para "X" para cancelar a edição
                    currentEditingButton = btn;

                    // Torna o campo AppName editável
                    AppNameTextbox.IsReadOnly = false;
                }
            }
            else
            {
                // Caso nenhum item esteja sendo editado, começa a edição do novo item
                UserTextbox.Text = item.Username;
                PasswordTextbox.Password = item.Password;
                AppNameTextbox.Text = item.AppName;
                editingCredentialFilePath = item.FilePath; // Armazena o caminho do arquivo
                btn.Content = "X";
                currentEditingButton = btn;

                // Torna o campo AppName editável
                AppNameTextbox.IsReadOnly = false;
            }
        }

        // Evento chamado quando o botão de deletar é clicado
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext; // Obtém os dados associados ao botão
            if (item != null)
            {
                // Deleta o arquivo de credencial
                if (File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath); // Deleta o arquivo da credencial
                }
                // Remove o item da coleção para atualizar a interface
                Credentials.Remove(item);
            }
        }

        // Função para criptografar os dados de texto usando AES
        private string EncryptData(string plainText)
        {
            // Chave e IV para a criptografia, ambos devem ser de tamanho fixo (16 bytes para AES-128)
            byte[] key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916"); // 16 bytes (128 bits) de chave
            byte[] iv = Encoding.UTF8.GetBytes("A1B2C3D4E5F60708"); // 16 bytes (128 bits) de vetor de inicialização

            // Cria o algoritmo AES
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC; // Modo CBC é utilizado para maior segurança
                aesAlg.Padding = PaddingMode.PKCS7; // Preenchimento PKCS7 garante que o texto tenha um múltiplo do tamanho do bloco

                // Cria um transformador para criptografar os dados
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Usa o CryptoStream para realizar a criptografia
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText); // Escreve os dados a serem criptografados
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray()); // Retorna os dados criptografados em base64
                }
            }
        }

        // Função para descriptografar os dados criptografados com AES
        private string DecryptData(string encryptedText)
        {
            // Chave e IV usados na criptografia, devem ser os mesmos para descriptografar corretamente
            byte[] key = Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916");
            byte[] iv = Encoding.UTF8.GetBytes("A1B2C3D4E5F60708");

            // Cria o algoritmo AES para descriptografar os dados
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Cria o transformador para descriptografar os dados
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    // Usa o CryptoStream para realizar a descriptografia
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd(); // Retorna o texto descriptografado
                    }
                }
            }
        }
    }
}