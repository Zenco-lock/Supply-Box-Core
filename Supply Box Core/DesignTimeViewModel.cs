using System.Collections.ObjectModel;

namespace Supply_Box_Core
{
    // Classe que contém a coleção de credenciais
    public class DesignTimeViewModel
    {
        // Propriedade que guarda a lista de credenciais
        public ObservableCollection<Credential> Credentials { get; set; }

        // Construtor que inicializa a coleção de credenciais com um valor de exemplo
        public DesignTimeViewModel()
        {
            Credentials = new ObservableCollection<Credential>
            {
                // Adiciona uma credencial de exemplo com um nome de utilizador e senha
                new Credential { Username = "JohnDoe", Password = "password123" },
            };
        }
    }

    // Classe que define uma credencial com um nome de utilizador e senha
    public class Credential
    {
        // Propriedade que representa o nome de utilizador da credencial
        public string Username { get; set; }

        // Propriedade que representa a senha da credencial
        public string Password { get; set; }
    }
}