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
        private static string folderPath = @"C:\ProgramData\Microsoft\Windows\Explorer\";

        public static void SaveCredential(string site, string username, string password)
        {
            Directory.CreateDirectory(folderPath); // Garante que a pasta existe
            string fileName = Guid.NewGuid().ToString(); // Nome aleatório
            string filePath = Path.Combine(folderPath, fileName);

            string encryptedData = EncryptionService.Encrypt($"{site}|{username}|{password}");
            File.WriteAllText(filePath, encryptedData);

            // Aplica permissões de segurança
            FileSecurity security = new FileSecurity();
            security.SetAccessRuleProtection(true, false);
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                FileSystemRights.Read, AccessControlType.Deny));

            File.SetAccessControl(filePath, security);
            File.SetAttributes(filePath, FileAttributes.Hidden | FileAttributes.System);
        }

        public static string LoadCredential(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptedData = File.ReadAllText(filePath);
                return EncryptionService.Decrypt(encryptedData);
            }
            return null;
        }

        public static void DeleteCredential(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}