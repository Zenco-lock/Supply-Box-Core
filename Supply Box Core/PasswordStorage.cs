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
    public static class PasswordStorage
    {
        public static void SaveCredential(string filePath, string encryptedData)
        {
            File.WriteAllText(filePath, encryptedData);
        }

        public static string LoadCredential(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public static IEnumerable<string> GetAllCredentialFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.dat");
        }
    }
}