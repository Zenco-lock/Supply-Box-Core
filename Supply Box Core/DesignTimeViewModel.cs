using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supply_Box_Core
{
    public class DesignTimeViewModel
    {
        public List<CredentialModel> Credentials { get; set; }

        public DesignTimeViewModel()
        {
            Credentials = new List<CredentialModel>
        {
            new CredentialModel { Username = "user1", Password = "password123" },
            new CredentialModel { Username = "user2", Password = "mySecret" }
        };
        }
    }

    public class CredentialModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}