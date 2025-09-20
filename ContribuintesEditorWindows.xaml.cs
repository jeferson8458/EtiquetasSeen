using ImpressoraEtiquetas;
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
using System.Windows.Shapes;

namespace SeenEtiquetas
{
    /// <summary>
    /// Lógica interna para ContribuintesEditorWindows.xaml
    /// </summary>
    public partial class ContribuintesEditorWindows : Window
    {
        public Contribuintes Contribuintes { get; private set; }
        public ContribuintesEditorWindows(Contribuintes contribuintes = null)
        {
            InitializeComponent();
            Contribuintes = contribuintes ?? new Contribuintes();

            if(contribuintes != null)
            {
                InscricaoTextBox.Text = Contribuintes.Inscricao;
                NomeTextBox.Text = Contribuintes.Nome;
                DependenteTextBox.Text = Contribuintes.Dependente;
                TrabalhoTextBox.Text = Contribuintes.Trabalho;
                EnderecoTextBox.Text = Contribuintes.Endereco;
            }   
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            Contribuintes.Inscricao = InscricaoTextBox.Text;
            Contribuintes.Nome = NomeTextBox.Text;
            Contribuintes.Dependente = DependenteTextBox.Text;
            Contribuintes.Trabalho = TrabalhoTextBox.Text;
            Contribuintes.Endereco = EnderecoTextBox.Text;
            DialogResult = true;
            Close();
        }


        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
