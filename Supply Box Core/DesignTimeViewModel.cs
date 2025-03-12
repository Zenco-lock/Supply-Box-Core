using System.Collections.ObjectModel;

namespace Supply_Box_Core
{
    public class DesignTimeViewModel
    {
        public ObservableCollection<Credential> Credentials { get; set; }

        public DesignTimeViewModel()
        {
            Credentials = new ObservableCollection<Credential>
            {
                new Credential { Username = "JohnDoe", Password = "password123" },
                new Credential { Username = "AliceSmith", Password = "qwerty" }
            };
        }
    }

    public class Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}