using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace ImpressoraEtiquetas
{
    public partial class MainWindow : Window
    {
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
            SociosGrid.ItemsSource = null;
            ContribuintesGrid.ItemsSource = null;
            SociosGrid.ItemsSource = DatabaseHelper.GetAllSocios();
            ContribuintesGrid.ItemsSource = DatabaseHelper.GetAllContribuintes();
        }

        private string GetSelectedTabName() => (MainTabControl.SelectedItem as TabItem)?.Header.ToString();
        private DataGrid GetSelectedGrid() => GetSelectedTabName() == "Sócios" ? SociosGrid : ContribuintesGrid;

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            string tipo = GetSelectedTabName();
            if (string.IsNullOrEmpty(tipo)) return;

            var editorWindow = new PessoaEditorWindow();
            editorWindow.Title = $"Adicionar Novo {tipo.TrimEnd('s')}";
            if (editorWindow.ShowDialog() == true)
            {
                if (tipo == "Sócios")
                    DatabaseHelper.AddSocio(editorWindow.Pessoa);
                else
                    DatabaseHelper.AddContribuinte(editorWindow.Pessoa);
                LoadGrids();
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            string tipo = GetSelectedTabName();
            var grid = GetSelectedGrid();
            if (grid.SelectedItem is Pessoa selectedPessoa)
            {
                var editorWindow = new PessoaEditorWindow(selectedPessoa);
                editorWindow.Title = $"Editar {tipo.TrimEnd('s')}";
                if (editorWindow.ShowDialog() == true)
                {
                    if (tipo == "Sócios")
                        DatabaseHelper.UpdateSocio(editorWindow.Pessoa);
                    else
                        DatabaseHelper.UpdateContribuinte(editorWindow.Pessoa);
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
            if (grid.SelectedItem is Pessoa selectedPessoa)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir {selectedPessoa.NomeCompleto}?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (tipo == "Sócios")
                        DatabaseHelper.DeleteSocio(selectedPessoa.Id);
                    else
                        DatabaseHelper.DeleteContribuinte(selectedPessoa.Id);
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

            if (grid.SelectedItem is Pessoa pessoaSelecionada)
            {
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

                        if (tipoSelecionado == "Sócios")
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

                            // --- CORREÇÃO APLICADA AQUI ---
                            // O "?? """ garante que, se o campo for nulo, um texto vazio será enviado.
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, nome_Y, nome_X, pessoaSelecionada.NomeCompleto ?? "");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, funcao_Y, funcao_X, pessoaSelecionada.Funcao ?? "");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, matricula_Y, matricula_X, pessoaSelecionada.Matricula ?? "");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, empresa_Y, empresa_X, pessoaSelecionada.Empresa ?? "");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, ctps_Y, ctps_X, pessoaSelecionada.Ctps ?? "");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, dep_Y, dep_X, pessoaSelecionada.NumeroDependentes ?? "");
                        }
                        else if (tipoSelecionado == "Contribuintes")
                        {
                            int fonteH = 3;
                            int fonteV = 3;

                            // Coordenadas fixas para Contribuintes (as mesmas que você definiu)
                            int nome_Y = 415, nome_X = 30;
                            int mat_Y = 331, mat_X = 30;
                            int cpf_Y = 245, cpf_X = 30;
                            int mail_Y = 170, mail_X = 30;
                            int obs_Y = 100, obs_X = 30;

                            // --- ALTERAÇÃO EXECUTADA AQUI ---

                            // 1. Junta Matrícula e Cidade
                            string textoMatCidade = $"Mat: {pessoaSelecionada.Matricula ?? ""} - Cidade: {pessoaSelecionada.Cidade ?? ""}";

                            // 2. Junta CPF e Celular
                            string textoCpfCelular = $"CPF: {pessoaSelecionada.Cpf ?? ""}    Cel: {pessoaSelecionada.Telefone ?? ""}";

                            // Impressão dos campos
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, nome_Y, nome_X, pessoaSelecionada.NomeCompleto ?? "");

                            // Imprime a string combinada de Matrícula e Cidade
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, mat_Y, mat_X, textoMatCidade);

                            // Imprime a string combinada de CPF e Celular
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, cpf_Y, cpf_X, textoCpfCelular);

                            // O resto continua igual, com os rótulos corrigidos
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, mail_Y, mail_X, $"Email: {pessoaSelecionada.Email ?? ""}");
                            E1Etiqueta.GerarTexto(1, 0, fonteH, fonteV, obs_Y, obs_X, $"Obs: {pessoaSelecionada.Observacao ?? ""}");
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
            }
            else
            {
                MessageBox.Show($"Por favor, selecione um item da lista de {tipoSelecionado} para imprimir.");
            }
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