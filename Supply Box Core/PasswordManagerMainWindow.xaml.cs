using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Supply_Box_Core
{
    /// <summary>
    /// Interaction logic for PasswordManagerMainWindow.xaml
    /// </summary>
    public partial class PasswordManagerMainWindow : Page
    {
        public PasswordManagerMainWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
/// remover os "exemplos" (jane e joe).
/// construir codigo simples´, com recurso a outros projetos similares
/// parte importante que nao me posso voltar a esquecer, armazenamento em ficheiro local, isto ja tem de ser resolvido com a questao do botao submit.
/// porque a grid de cima ira escrever e a de baxio (jane e joe) iram ler esse arquivo
/// adicionar botao de "copy to clipboard"
/// configurar botao menu