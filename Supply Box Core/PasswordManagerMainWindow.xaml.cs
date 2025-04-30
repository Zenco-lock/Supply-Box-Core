using System;
using System.Collections.Generic;
using System.IO;
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
using System.Security.Cryptography;
using IOPath = System.IO.Path;
using System.Collections.ObjectModel;

namespace Supply_Box_Core
{
    public partial class PasswordManagerMainWindow : Window
    {
        public ObservableCollection<dynamic> Credentials { get; set; } = new ObservableCollection<dynamic>();
        private readonly string dataFolder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsSecurityCache");

        public PasswordManagerMainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Directory.CreateDirectory(dataFolder);
            AppNameTextbox.IsReadOnly = false; // Garante que o campo seja editável ao iniciar
            LoadCredentials();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string appName = AppNameTextbox.Text;
            string username = UserTextbox.Text;
            string password = PasswordTextbox.Password;

            // Validação: todos os campos devem estar preenchidos
            if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields must be filled.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string plainData = $"{appName}|{username}|{password}";
                string encryptedData = EncryptionService.EncryptData(plainData);
                string fileName = editingCredentialFilePath;

                if (editingCredentialFilePath != null)
                {
                    // Edição
                    PasswordStorage.SaveCredential(editingCredentialFilePath, encryptedData);
                }
                else
                {
                    // Novo ficheiro
                    fileName = IOPath.Combine(dataFolder, $"{Guid.NewGuid()}.dat");
                    PasswordStorage.SaveCredential(fileName, encryptedData);
                }

                // Verificar se o arquivo foi salvo
                if (File.Exists(fileName))
                {
                    string action = editingCredentialFilePath != null ? "edited" : "added";
                    MessageBox.Show($"Credential for {appName} {action} successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    editingCredentialFilePath = null;
                }
                else
                {
                    MessageBox.Show($"Failed to save credential for {appName}. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Limpa campos e recarrega
                AppNameTextbox.Clear();
                UserTextbox.Clear();
                PasswordTextbox.Clear();
                LoadCredentials();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the credential for {AppNameTextbox.Text}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCredentials()
        {
            Credentials.Clear();

            try
            {
                foreach (var file in PasswordStorage.GetAllCredentialFiles(dataFolder))
                {
                    try
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
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load credential from {file}: {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading credentials: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                dynamic item = btn.DataContext;
                if (item != null)
                {
                    Clipboard.SetText(item.Password);
                    MessageBox.Show($"Password for {item.AppName} copied to clipboard.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No credential selected to copy.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while copying the password for {AppNameTextbox.Text}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string editingCredentialFilePath = null;
        private Button currentEditingButton = null;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.DataContext;
            if (item == null)
            {
                MessageBox.Show("No credential selected to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (editingCredentialFilePath != null)
            {
                if (currentEditingButton == btn)
                {
                    string cancelledApp = AppNameTextbox.Text;
                    editingCredentialFilePath = null;
                    currentEditingButton.Content = "✏";
                    currentEditingButton = null;
                    UserTextbox.Clear();
                    PasswordTextbox.Clear();
                    AppNameTextbox.Clear();
                    AppNameTextbox.IsReadOnly = false;
                    MessageBox.Show($"Edit canceled for {cancelledApp}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (currentEditingButton != null)
                        currentEditingButton.Content = "✏";

                    UserTextbox.Text = item.Username;
                    PasswordTextbox.Password = item.Password;
                    AppNameTextbox.Text = item.AppName;
                    editingCredentialFilePath = item.FilePath;
                    btn.Content = "X";
                    currentEditingButton = btn;
                    AppNameTextbox.IsReadOnly = false;
                    MessageBox.Show($"Editing credential for {item.AppName}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                UserTextbox.Text = item.Username;
                PasswordTextbox.Password = item.Password;
                AppNameTextbox.Text = item.AppName;
                editingCredentialFilePath = item.FilePath;
                btn.Content = "X";
                currentEditingButton = btn;
                AppNameTextbox.IsReadOnly = false;
                MessageBox.Show($"Editing credential for {item.AppName}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                dynamic item = btn.DataContext;
                if (item != null)
                {
                    if (File.Exists(item.FilePath))
                    {
                        File.Delete(item.FilePath);
                        Credentials.Remove(item);
                        MessageBox.Show($"Credential for {item.AppName} deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Credential file for {item.AppName} not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("No credential selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the credential for {AppNameTextbox.Text}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}