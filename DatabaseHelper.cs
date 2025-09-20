using Microsoft.Data.Sqlite;
using SeenEtiquetas;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImpressoraEtiquetas
{
    public class ConfiguracaoImpressao
    {
        public string Nome { get; set; }
        public int Calor { get; set; } = 12;
        public int Velocidade { get; set; } = 4;
        public int OffsetLinha { get; set; } = 20;
        public int OffsetColuna { get; set; } = 30;
    }

    public static class DatabaseHelper
    {
        private static string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "associados_v2.db");
        private static string connectionString = $"Data Source={databasePath}";

        // Este método é chamado apenas se o banco de dados não existir.
        // A criação principal é feita pelo seu programa Importador.
        public static void InitializeDatabase()
        {
            if (File.Exists(databasePath)) return;

            // Se o arquivo não existe, cria um banco vazio para o programa não dar erro.
            // O ideal é sempre usar o banco gerado pelo Importador.
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Apenas para garantir que o arquivo seja criado se estiver faltando.
            }
        }

        public static string GetNextSocioMatricula()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // A lógica é a mesma do GetNextContribuinteId, mas para a tabela Socios e a coluna Matricula.
                // CAST(Matricula AS INTEGER) é crucial para garantir que os números sejam comparados corretamente (ex: 10 > 9).
                var query = "SELECT IFNULL(MAX(CAST(Matricula AS INTEGER)), 0) FROM Socios;";
                using (var command = new SqliteCommand(query, connection))
                {
                    long maxMatricula = (long)command.ExecuteScalar();
                    // Retorna o próximo número da matrícula como string
                    return (maxMatricula + 1).ToString("D6");
                }
            }
        }

        public static string GetNextContribuinteInscricao()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // A query busca o maior valor numérico na coluna Inscricao
                var query = "SELECT IFNULL(MAX(CAST(Inscricao AS INTEGER)), 0) FROM Contribuintes;";
                using (var command = new SqliteCommand(query, connection))
                {
                    long maxInscricao = (long)command.ExecuteScalar();
                    // Retorna o próximo número formatado com 4 dígitos (ex: "0001", "0159")
                    return (maxInscricao + 1).ToString("D4");
                }
            }
        }

        // --- MÉTODOS PARA SÓCIOS ---
        public static List<Socio> GetAllSocios()
        {
            var socios = new List<Socio>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("SELECT * FROM Socios ORDER BY Id DESC;", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        socios.Add(new Socio
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NomeCompleto = reader["NomeCompleto"].ToString(),
                            Cpf = reader["Cpf"].ToString(),
                            Matricula = reader["Matricula"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            Telefone = reader["Telefone"].ToString(),
                            Email = reader["Email"].ToString(),
                            Observacao = reader["Observacao"].ToString(),
                            Funcao = reader["Funcao"].ToString(),
                            Empresa = reader["Empresa"].ToString(),
                            Ctps = reader["Ctps"].ToString(),
                            NumeroDependentes = reader["NumeroDependentes"].ToString()
                        });
                    }
                }
            }
            return socios;
        }

        public static void AddSocio(Socio socio)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Socios (NomeCompleto, Cpf, Matricula, Cidade, Telefone, Email, Observacao, Funcao, Empresa, Ctps, NumeroDependentes)
                    VALUES (@Nome, @Cpf, @Matricula, @Cidade, @Telefone, @Email, @Obs, @Funcao, @Empresa, @Ctps, @Dep);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nome", socio.NomeCompleto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cpf", socio.Cpf ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Matricula", socio.Matricula ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cidade", socio.Cidade ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefone", socio.Telefone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", socio.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Obs", socio.Observacao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Funcao", socio.Funcao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Empresa", socio.Empresa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Ctps", socio.Ctps ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Dep", socio.NumeroDependentes ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateSocio(Socio socio)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE Socios SET
                        NomeCompleto = @Nome, Cpf = @Cpf, Matricula = @Matricula, Cidade = @Cidade,
                        Telefone = @Telefone, Email = @Email, Observacao = @Obs, Funcao = @Funcao, 
                        Empresa = @Empresa, Ctps = @Ctps, NumeroDependentes = @Dep
                    WHERE Id = @Id;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", socio.Id);
                    command.Parameters.AddWithValue("@Nome", socio.NomeCompleto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cpf", socio.Cpf ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Matricula", socio.Matricula ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cidade", socio.Cidade ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefone", socio.Telefone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", socio.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Obs", socio.Observacao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Funcao", socio.Funcao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Empresa", socio.Empresa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Ctps", socio.Ctps ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Dep", socio.NumeroDependentes ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSocio(int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("DELETE FROM Socios WHERE Id = @Id;", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // --- MÉTODOS PARA CONTRIBUINTES ---
        public static List<Contribuintes> GetAllContribuintes()
        {
            var contribuintes = new List<Contribuintes>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("SELECT * FROM Contribuintes ORDER BY DbId DESC;", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contribuintes.Add(new Contribuintes
                        {
                            ID_Original = reader["ID_Original"].ToString(),
                            Inscricao = reader["Inscricao"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            Dependente = reader["Dependente"].ToString(),
                            Trabalho = reader["Trabalho"].ToString(),
                            Endereco = reader["Endereco"].ToString(),
                        });
                    }
                }
            }
            return contribuintes;
        }

        public static string GetNextContribuinteId()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT IFNULL(MAX(CAST(ID_Original AS INTEGER)), 0) FROM Contribuintes;";
                using (var command = new SqliteCommand(query, connection))
                {
                    long maxId = (long)command.ExecuteScalar();
                    return (maxId + 1).ToString();
                }
            }
        }

        public static void AddContribuinte(Contribuintes contribuinte)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Contribuintes (ID_Original, Inscricao, Nome, Dependente, Trabalho, Endereco) VALUES (@ID_Original, @Inscricao, @Nome, @Dependente, @Trabalho, @Endereco);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Original", contribuinte.ID_Original ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Inscricao", contribuinte.Inscricao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nome", contribuinte.Nome ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Dependente", contribuinte.Dependente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Trabalho", contribuinte.Trabalho ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Endereco", contribuinte.Endereco ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateContribuinte(Contribuintes contribuinte)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var query = @"UPDATE Contribuintes SET 
                                Inscricao = @Inscricao, Nome = @Nome, Dependente = @Dependente, 
                                Trabalho = @Trabalho, Endereco = @Endereco 
                              WHERE ID_Original = @ID_Original;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Original", contribuinte.ID_Original);
                    command.Parameters.AddWithValue("@Inscricao", contribuinte.Inscricao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nome", contribuinte.Nome ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Dependente", contribuinte.Dependente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Trabalho", contribuinte.Trabalho ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Endereco", contribuinte.Endereco ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteContribuinte(string id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("DELETE FROM Contribuintes WHERE ID_Original = @ID_Original;", connection))
                {
                    command.Parameters.AddWithValue("@ID_Original", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // --- MÉTODOS PARA CONFIGURAÇÕES ---
        public static ConfiguracaoImpressao GetConfiguracao(string nome)
        {
            ConfiguracaoImpressao config = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("SELECT * FROM Configuracoes WHERE Nome = @Nome", connection))
                {
                    command.Parameters.AddWithValue("@Nome", nome);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            config = new ConfiguracaoImpressao
                            {
                                Nome = reader["Nome"].ToString(),
                                Calor = Convert.ToInt32(reader["Calor"]),
                                Velocidade = Convert.ToInt32(reader["Velocidade"]),
                                OffsetLinha = Convert.ToInt32(reader["OffsetLinha"]),
                                OffsetColuna = Convert.ToInt32(reader["OffsetColuna"])
                            };
                        }
                    }
                }
            }
            return config ?? new ConfiguracaoImpressao { Nome = nome };
        }

        public static void SalvarConfiguracao(ConfiguracaoImpressao config)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Configuracoes SET Calor = @Calor, Velocidade = @Velocidade, OffsetLinha = @OffsetLinha, OffsetColuna = @OffsetColuna WHERE Nome = @Nome;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nome", config.Nome);
                    command.Parameters.AddWithValue("@Calor", config.Calor);
                    command.Parameters.AddWithValue("@Velocidade", config.Velocidade);
                    command.Parameters.AddWithValue("@OffsetLinha", config.OffsetLinha);
                    command.Parameters.AddWithValue("@OffsetColuna", config.OffsetColuna);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}