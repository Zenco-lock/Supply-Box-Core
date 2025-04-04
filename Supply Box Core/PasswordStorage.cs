using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Supply_Box_Core
{
    public class PasswordStorage
    {
        // Caminho onde as credenciais encriptadas serão armazenadas
        private static string folderPath = @"C:\ProgramData\Microsoft\Windows\Explorer\";

        // Método para guardar credenciais encriptadas
        public static void SaveCredential(string site, string username, string password, string appName)
        {
            // Garante que o diretório existe
            Directory.CreateDirectory(folderPath);

            // Gera um identificador único para o ficheiro
            string fileName = Guid.NewGuid().ToString();
            string filePath = Path.Combine(folderPath, fileName);

            // Encripta os dados fornecidos
            string encryptedData = EncryptionService.Encrypt(site, username, password, appName);

            // Escreve os dados encriptados no ficheiro
            File.WriteAllText(filePath, encryptedData);

#if WINDOWS
            // Início do bloco condicional para plataformas Windows
            // Este código será executado apenas em sistemas Windows.

            // Configuração das permissões do ficheiro para maior segurança
            FileInfo fileInfo = new FileInfo(filePath);
            FileSecurity security = new FileSecurity();

            // Protege o ficheiro contra alterações de permissões não autorizadas
            security.SetAccessRuleProtection(true, false);

            // Permite acesso total ao sistema e administradores
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));

            // Nega acesso de leitura a utilizadores autenticados comuns
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                FileSystemRights.Read, AccessControlType.Deny));

            // Aplica as configurações de segurança ao ficheiro
            fileInfo.SetAccessControl(security);

            // Define o ficheiro como oculto e do sistema para evitar deteção fácil
            fileInfo.Attributes = FileAttributes.Hidden | FileAttributes.System;

            // Fim do bloco condicional para plataformas Windows
#endif
        }

        // Método para carregar credenciais de um ficheiro encriptado
        public static (string site, string username, string password, string appName) LoadCredential(string filePath)
        {
            // Verifica se o ficheiro existe antes de tentar ler
            if (File.Exists(filePath))
            {
                // Lê os dados encriptados do ficheiro
                string encryptedData = File.ReadAllText(filePath);

                // Desencripta e retorna os dados
                return EncryptionService.Decrypt(encryptedData);
            }

            // Lança uma exceção caso o ficheiro não seja encontrado
            throw new FileNotFoundException("File not found.");
        }

        // Método para apagar um ficheiro de credenciais
        public static void DeleteCredential(string filePath)
        {
            // Verifica se o ficheiro existe antes de tentar apagar
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}