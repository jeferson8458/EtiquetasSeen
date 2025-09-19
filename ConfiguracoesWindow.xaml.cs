using System.Windows;

namespace ImpressoraEtiquetas
{
    public partial class ConfiguracoesWindow : Window
    {
        private ConfiguracaoImpressao _config;

        public ConfiguracoesWindow(ConfiguracaoImpressao config)
        {
            InitializeComponent();
            _config = config;
            CarregarConfiguracoes();
        }

        private void CarregarConfiguracoes()
        {
            TipoLabel.Text = _config.Nome;
            CalorSlider.Value = _config.Calor;
            VelocidadeSlider.Value = _config.Velocidade;
            OffsetLinhaTxt.Text = _config.OffsetLinha.ToString();
            OffsetColunaTxt.Text = _config.OffsetColuna.ToString();
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // ################### INÍCIO DA CORREÇÃO ###################
            // Validação para garantir que os valores das margens são números válidos.
            if (!int.TryParse(OffsetLinhaTxt.Text, out int offsetLinha) ||
                !int.TryParse(OffsetColunaTxt.Text, out int offsetColuna))
            {
                MessageBox.Show("Os valores para 'Margem Superior' e 'Margem Esquerda' devem ser números inteiros.",
                                "Dados Inválidos", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Impede o salvamento se os dados estiverem errados.
            }
            // ################### FIM DA CORREÇÃO ###################

            // Atualiza o objeto de configuração com os novos valores
            _config.Calor = (int)CalorSlider.Value;
            _config.Velocidade = (int)VelocidadeSlider.Value;
            _config.OffsetLinha = offsetLinha;
            _config.OffsetColuna = offsetColuna;

            // Manda o DatabaseHelper salvar as alterações no banco de dados
            DatabaseHelper.SalvarConfiguracao(_config);

            MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}