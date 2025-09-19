using System.Windows;

namespace ImpressoraEtiquetas
{
    public partial class PessoaEditorWindow : Window
    {
        public Pessoa Pessoa { get; private set; }

        public PessoaEditorWindow(Pessoa pessoa = null)
        {
            InitializeComponent();
            Pessoa = pessoa ?? new Pessoa();

            // Carrega os dados da pessoa nos campos da janela
            if (pessoa != null)
            {
                // Campos existentes
                NomeCompletoTextBox.Text = pessoa.NomeCompleto;
                CpfTextBox.Text = pessoa.Cpf;
                MatriculaTextBox.Text = pessoa.Matricula;
                CidadeTextBox.Text = pessoa.Cidade;
                TelefoneTextBox.Text = pessoa.Telefone;
                EmailTextBox.Text = pessoa.Email;
                ObservacaoTextBox.Text = pessoa.Observacao;

                // --- Carrega os dados dos NOVOS CAMPOS ---
                FuncaoTextBox.Text = pessoa.Funcao;
                EmpresaTextBox.Text = pessoa.Empresa;
                CtpsTextBox.Text = pessoa.Ctps;
                NumeroDependentesTextBox.Text = pessoa.NumeroDependentes;
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // Salva os dados dos campos da janela de volta para o objeto Pessoa

            // Campos existentes
            Pessoa.NomeCompleto = NomeCompletoTextBox.Text;
            Pessoa.Cpf = CpfTextBox.Text;
            Pessoa.Matricula = MatriculaTextBox.Text;
            Pessoa.Cidade = CidadeTextBox.Text;
            Pessoa.Telefone = TelefoneTextBox.Text;
            Pessoa.Email = EmailTextBox.Text;
            Pessoa.Observacao = ObservacaoTextBox.Text;

            // --- Salva os dados dos NOVOS CAMPOS ---
            Pessoa.Funcao = FuncaoTextBox.Text;
            Pessoa.Empresa = EmpresaTextBox.Text;
            Pessoa.Ctps = CtpsTextBox.Text;
            Pessoa.NumeroDependentes = NumeroDependentesTextBox.Text;

            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}