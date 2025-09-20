using System.Windows;

namespace ImpressoraEtiquetas
{
    // --- MUDANÇA: O nome da classe da janela foi alterado
    public partial class SocioEditorWindow : Window
    {
        // --- MUDANÇA: A propriedade agora se chama "Socio" (é do tipo Socio, nomeada Socio)
        public Socio Socio { get; private set; }

        // --- MUDANÇA: O construtor foi renomeado e o parâmetro também para consistência
        public SocioEditorWindow(Socio socio = null)
        {
            InitializeComponent();

            // --- MUDANÇA: Usando a nova propriedade e parâmetro
            Socio = socio ?? new Socio();

            // Carrega os dados do sócio nos campos da janela
            if (socio != null)
            {
                NomeCompletoTextBox.Text = socio.NomeCompleto;
                CpfTextBox.Text = socio.Cpf;
                MatriculaTextBox.Text = socio.Matricula;
                CidadeTextBox.Text = socio.Cidade;
                TelefoneTextBox.Text = socio.Telefone;
                EmailTextBox.Text = socio.Email;
                ObservacaoTextBox.Text = socio.Observacao;
                FuncaoTextBox.Text = socio.Funcao;
                EmpresaTextBox.Text = socio.Empresa;
                CtpsTextBox.Text = socio.Ctps;
                NumeroDependentesTextBox.Text = socio.NumeroDependentes;
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // Salva os dados dos campos da janela de volta para o objeto Socio
            // --- MUDANÇA: Atualiza a propriedade "Socio"
            Socio.NomeCompleto = NomeCompletoTextBox.Text;
            Socio.Cpf = CpfTextBox.Text;
            Socio.Matricula = MatriculaTextBox.Text;
            Socio.Cidade = CidadeTextBox.Text;
            Socio.Telefone = TelefoneTextBox.Text;
            Socio.Email = EmailTextBox.Text;
            Socio.Observacao = ObservacaoTextBox.Text;
            Socio.Funcao = FuncaoTextBox.Text;
            Socio.Empresa = EmpresaTextBox.Text;
            Socio.Ctps = CtpsTextBox.Text;
            Socio.NumeroDependentes = NumeroDependentesTextBox.Text;

            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}