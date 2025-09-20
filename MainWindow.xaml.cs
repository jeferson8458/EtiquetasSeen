using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using SeenEtiquetas;

namespace ImpressoraEtiquetas
{
    public partial class MainWindow : Window
    {

        // NOVO: Listas para guardar todos os registros originais do banco
        private List<Socio> todosOsSocios = new List<Socio>();
        private List<Contribuintes> todosOsContribuintes = new List<Contribuintes>();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                DatabaseHelper.InitializeDatabase();
                LoadGrids();
                StatusText.Text = "Pronto. Impressora não foi configurada ao iniciar.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro fatal na inicialização: " + ex.Message, "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Limpa o texto da caixa de busca
            SearchTextBox.Clear();

            // 2. Coloca o cursor de volta na caixa de busca para uma nova pesquisa
            SearchTextBox.Focus();
        }

        private void ConfigurarHardware_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Isso enviará os comandos de configuração inicial para a impressora. Deve ser usado apenas uma vez ou se a impressora for resetada.\n\nDeseja continuar?",
                                "Confirmação de Configuração de Hardware", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                StatusText.Text = "Configurando hardware da impressora...";
                E1Etiqueta.Limpa(0);
                E1Etiqueta.SetTipoTransferencia(1, "L42PRO", "USB", 0, 1);
                E1Etiqueta.SetMedidas(0);
                E1Etiqueta.SetLength(500);
                E1Etiqueta.SetAlturaGap(1, "L42PRO", "USB", 0, 30);
                E1Etiqueta.SetSensor(0);
                StatusText.Text = "Hardware da impressora configurado com sucesso.";
                MessageBox.Show("Configuração de hardware enviada com sucesso para a impressora!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível comunicar com a impressora.\n\nDetalhes: " + ex.Message, "Erro de Comunicação", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Erro ao configurar hardware.";
            }
        }

        private void LoadGrids()
        {
            // CORREÇÃO PRINCIPAL:
            // 1. Carrega os dados do banco para as nossas listas "master"
            todosOsSocios = DatabaseHelper.GetAllSocios();
            todosOsContribuintes = DatabaseHelper.GetAllContribuintes();

            // 2. Chama o filtro, que vai popular as grades com a lista completa inicialmente
            ApplyFilter();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            // Garante que a caixa de busca exista antes de tentar usá-la
            if (SearchTextBox == null) return;

            string searchText = SearchTextBox.Text.ToLower();

            // Filtra a lista de Sócios
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SociosGrid.ItemsSource = todosOsSocios;
            }
            else
            {
                SociosGrid.ItemsSource = todosOsSocios.Where(s =>
                    (s.NomeCompleto?.ToLower().Contains(searchText) ?? false) ||
                    (s.Cpf?.ToLower().Contains(searchText) ?? false) ||
                    (s.Matricula?.ToLower().Contains(searchText) ?? false)
                ).ToList();
            }

            // Filtra a lista de Contribuintes
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ContribuintesGrid.ItemsSource = todosOsContribuintes;
            }
            else
            {
                ContribuintesGrid.ItemsSource = todosOsContribuintes.Where(c =>
                    (c.Nome?.ToLower().Contains(searchText) ?? false) ||
                    (c.Inscricao?.ToLower().Contains(searchText) ?? false) ||
                    (c.Trabalho?.ToLower().Contains(searchText) ?? false)
                ).ToList();
            }

            // Atualiza a exibição das grades
            SociosGrid.Items.Refresh();
            ContribuintesGrid.Items.Refresh();
        }


        private string GetSelectedTabName() => (MainTabControl.SelectedItem as TabItem)?.Header.ToString();
        private DataGrid GetSelectedGrid() => GetSelectedTabName() == "Sócios" ? SociosGrid : ContribuintesGrid;

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            string tipo = GetSelectedTabName();
            if (string.IsNullOrEmpty(tipo)) return;

            if (tipo == "Sócios")
            {
                // 1. Cria um novo objeto Sócio
                var novoSocio = new Socio();

                // 2. Busca a próxima matrícula disponível no banco de dados e a atribui ao novo sócio
                novoSocio.Matricula = DatabaseHelper.GetNextSocioMatricula();

                // 3. Passa o objeto Sócio (já com a matrícula preenchida) para a janela de edição
                var editorWindow = new SocioEditorWindow(novoSocio);
                editorWindow.Title = "Adicionar Novo Sócio";

                if (editorWindow.ShowDialog() == true)
                {
                    // O editorWindow já contém o objeto atualizado, basta salvar
                    DatabaseHelper.AddSocio(editorWindow.Socio);
                    LoadGrids(); // Recarrega os dados e atualiza a tela
                }
            }
            else // "Contribuintes"
            {
                // 1. Cria o novo objeto
                var novoContribuinte = new Contribuintes();

                // 2. Busca e atribui o próximo ID Original E a próxima Inscrição
                novoContribuinte.ID_Original = DatabaseHelper.GetNextContribuinteId();
                novoContribuinte.Inscricao = DatabaseHelper.GetNextContribuinteInscricao();

                // 3. Passa o objeto já preenchido para a janela de edição
                var editorWindow = new ContribuintesEditorWindows(novoContribuinte);
                editorWindow.Title = "Adicionar Novo Contribuinte";

                if (editorWindow.ShowDialog() == true)
                {
                    DatabaseHelper.AddContribuinte(editorWindow.Contribuintes);
                    LoadGrids(); // Recarrega os dados e atualiza a tela
                }
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            string tipo = GetSelectedTabName();
            var grid = GetSelectedGrid();

            if (tipo == "Sócios" && grid.SelectedItem is Socio selectedSocio)
            {
                var editorWindow = new SocioEditorWindow(selectedSocio);
                editorWindow.Title = "Editar Sócio";
                if (editorWindow.ShowDialog() == true)
                {
                    DatabaseHelper.UpdateSocio(editorWindow.Socio);
                    LoadGrids();
                }
            }
            else if (tipo == "Contribuintes" && grid.SelectedItem is Contribuintes selectedContribuinte)
            {
                var editorWindow = new ContribuintesEditorWindows(selectedContribuinte);
                editorWindow.Title = "Editar Contribuinte";
                if (editorWindow.ShowDialog() == true)
                {
                    DatabaseHelper.UpdateContribuinte(editorWindow.Contribuintes);
                    LoadGrids();
                }
            }
            else
            {
                MessageBox.Show($"Selecione um item da lista de {tipo} para editar.");
            }
        }


        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            string tipo = GetSelectedTabName();
            var grid = GetSelectedGrid();

            if (tipo == "Sócios" && grid.SelectedItem is Socio selectedSocio)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir {selectedSocio.NomeCompleto}?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DatabaseHelper.DeleteSocio(selectedSocio.Id);
                    LoadGrids();
                }
            }
            else if (tipo == "Contribuintes" && grid.SelectedItem is Contribuintes selectedContribuinte)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir {selectedContribuinte.Nome}?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DatabaseHelper.DeleteContribuinte(selectedContribuinte.ID_Original);
                    LoadGrids();
                }
            }
            else
            {
                MessageBox.Show($"Selecione um item da lista de {tipo} para excluir.");
            }
        }

        private async void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            string tipoSelecionado = GetSelectedTabName();
            var grid = GetSelectedGrid();

            // ################### INÍCIO DA CORREÇÃO ###################
            // A lógica de impressão também deve ser corrigida para lidar com os tipos Pessoa e Contribuintes
            object selectedItem = grid.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show($"Por favor, selecione um item da lista de {tipoSelecionado} para imprimir.");
                return;
            }

            var printButton = sender as Button;
            printButton.IsEnabled = false;
            StatusText.Text = "Enviando para impressão...";

            try
            {
                await Task.Run(() =>
                {
                    ConfiguracaoImpressao config = DatabaseHelper.GetConfiguracao(tipoSelecionado);

                    E1Etiqueta.SetCalor(config.Calor);
                    E1Etiqueta.SetVelImpressao(config.Velocidade);
                    E1Etiqueta.SetOffsetLinha(config.OffsetLinha);
                    E1Etiqueta.SetOffsetColuna(config.OffsetColuna);

                    E1Etiqueta.Limpa(1);

                    if (tipoSelecionado == "Sócios" && selectedItem is Socio pessoaSelecionada)
                    {
                        int fonteH = 2;
                        int fonteV = 2;

                        // Coordenadas (ajuste conforme necessário)
                        int nome_Y = 415, nome_X = 30;
                        int funcao_Y = 331, funcao_X = 30;
                        int matricula_Y = 331, matricula_X = 440;
                        int empresa_Y = 245, empresa_X = 30;
                        int ctps_Y = 160, ctps_X = 30;
                        int dep_Y = 160, dep_X = 440;

                        // O "?? """ garante que, se o campo for nulo, um texto vazio será enviado.
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, nome_Y, nome_X, pessoaSelecionada.NomeCompleto ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, funcao_Y, funcao_X, pessoaSelecionada.Funcao ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, matricula_Y, matricula_X, pessoaSelecionada.Matricula ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, empresa_Y, empresa_X, pessoaSelecionada.Empresa ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, ctps_Y, ctps_X, pessoaSelecionada.Ctps ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, dep_Y, dep_X, pessoaSelecionada.NumeroDependentes ?? "");
                    }
                    else if (tipoSelecionado == "Contribuintes" && selectedItem is Contribuintes contribuinteSelecionado)
                    {
                        int fonteH = 3;
                        int fonteV = 3;

                        // Coordenadas fixas para Contribuintes (as mesmas que você definiu)
                        int nome_Y = 415, nome_X = 30;
                        int mat_Y = 331, mat_X = 30;
                        int cpf_Y = 245, cpf_X = 30;
                        int mail_Y = 170, mail_X = 30;
                        int obs_Y = 100, obs_X = 30;

                        // O tipo Contribuintes não tem as propriedades do tipo Pessoa
                        // Usamos as propriedades corretas da classe Contribuintes

                        string textoInscricao = $"Insc: {contribuinteSelecionado.Inscricao ?? ""}";
                        string textoDependente = $"Dep: {contribuinteSelecionado.Dependente ?? ""}";
                        string textoTrabalho = $"Trab: {contribuinteSelecionado.Trabalho ?? ""}";
                        string textoEndereco = $"End: {contribuinteSelecionado.Endereco ?? ""}";

                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, nome_Y, nome_X, contribuinteSelecionado.Nome ?? "");
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, mat_Y, mat_X, textoInscricao);
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, cpf_Y, cpf_X, textoDependente);
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, mail_Y, mail_X, textoTrabalho);
                        E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, obs_Y, obs_X, textoEndereco);
                    }

                    E1Etiqueta.SetQtde(1);
                    int retorno = E1Etiqueta.Imprime(1, "L42PRO", "USB", 0);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = retorno == 0 ? "Impressão enviada com sucesso!" : $"Erro! Código: {retorno}.";
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir: " + ex.Message);
            }
            finally
            {
                printButton.IsEnabled = true;
            }
            // ################### FIM DA CORREÇÃO ###################
        }

        private void Configuracoes_Click(object sender, RoutedEventArgs e)
        {
            string senha = Interaction.InputBox("Digite a senha de administrador:", "Acesso Restrito", "");
            if (senha != "admin123")
            {
                if (!string.IsNullOrEmpty(senha)) MessageBox.Show("Senha incorreta!");
                return;
            }

            string tipoConfig = GetSelectedTabName();
            ConfiguracaoImpressao config = DatabaseHelper.GetConfiguracao(tipoConfig);

            ConfiguracoesWindow configWindow = new ConfiguracoesWindow(config) { Owner = this };
            configWindow.ShowDialog();
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}