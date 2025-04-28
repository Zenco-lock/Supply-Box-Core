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
            string appName = AppNameTextbox.Text;
            string username = UserTextbox.Text;
            string password = PasswordTextbox.Password;

            // ... validação omitida ...

            string plainData = $"{appName}|{username}|{password}";
            string encryptedData = EncryptionService.EncryptData(plainData);

            if (editingCredentialFilePath != null)
            {
                // Edição
                PasswordStorage.SaveCredential(editingCredentialFilePath, encryptedData);
                editingCredentialFilePath = null;
            }
            else
            {
                // Novo ficheiro no dataFolder
                string fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat");
                PasswordStorage.SaveCredential(fileName, encryptedData);
            }

            // Limpa campos e recarrega
            AppNameTextbox.Clear();
            UserTextbox.Clear();
            PasswordTextbox.Clear();
            LoadCredentials();
        }

        // Carrega as credenciais salvas no diretório de dados
        private void LoadCredentials()
        {
            Credentials.Clear();

            // Usa dataFolder já criado no construtor
            foreach (var file in PasswordStorage.GetAllCredentialFiles(dataFolder))
            {
                string encryptedData = PasswordStorage.LoadCredential(file);
                string decryptedData = EncryptionService.DecryptData(encryptedData);

                if (!string.IsNullOrEmpty(decryptedData))
                {
                    string[] parts = decryptedData.Split('|');
                    if (parts.Length == 3)
                    {
                        Credentials.Add(new
                        {
                            AppName = parts[0],
                            Username = parts[1],
                            Password = parts[2],
                            FilePath = file
                        });
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
    }
}