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

        public static void SaveCredential(string site, string username, string password, string appName)
        {
            Directory.CreateDirectory(folderPath);
            string fileName = Guid.NewGuid().ToString();
            string filePath = Path.Combine(folderPath, fileName);

            string encryptedData = EncryptionService.Encrypt(site, username, password, appName);
            File.WriteAllText(filePath, encryptedData);

#if WINDOWS
            FileInfo fileInfo = new FileInfo(filePath);
            FileSecurity security = new FileSecurity();
            security.SetAccessRuleProtection(true, false);
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),
                FileSystemRights.Read, AccessControlType.Deny));

            fileInfo.SetAccessControl(security);
            fileInfo.Attributes = FileAttributes.Hidden | FileAttributes.System;
#endif
        }

        public static (string site, string username, string password, string appName) LoadCredential(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptedData = File.ReadAllText(filePath);
                return EncryptionService.Decrypt(encryptedData);
            }
            throw new FileNotFoundException("Arquivo não encontrado.");
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