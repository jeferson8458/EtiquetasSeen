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

        public static void InitializeDatabase()
        {
            if (File.Exists(databasePath)) return;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // ################### INÍCIO DA CORREÇÃO ###################
                // Adicionando os novos campos na criação da tabela Socios
                string createSociosTable = @"
                    CREATE TABLE Socios (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        NomeCompleto TEXT NOT NULL,
                        Cpf TEXT,
                        Matricula TEXT,
                        Cidade TEXT,
                        Telefone TEXT,
                        Email TEXT,
                        Observacao TEXT,
                        Funcao TEXT,
                        Empresa TEXT,
                        Ctps TEXT,
                        NumeroDependentes TEXT
                    );";
                new SqliteCommand(createSociosTable, connection).ExecuteNonQuery();
                // ################### FIM DA CORREÇÃO ###################

                // ################### INÍCIO DA CORREÇÃO DA TABELA CONTRIBUINTES ###################
                // Corrigindo a criação da tabela Contribuintes para corresponder ao JSON/CSV.
                string createContribuintesTable = @"
                    CREATE TABLE Contribuintes (
                        ID_Original TEXT PRIMARY KEY,
                        Inscricao TEXT,
                        Nome TEXT,
                        Dependente TEXT,
                        Trabalho TEXT,
                        Endereco TEXT
                    );";
                new SqliteCommand(createContribuintesTable, connection).ExecuteNonQuery();
                // ################### FIM DA CORREÇÃO DA TABELA CONTRIBUINTES ###################

                string createConfigTable = "CREATE TABLE Configuracoes (Nome TEXT PRIMARY KEY, Calor INTEGER, Velocidade INTEGER, OffsetLinha INTEGER, OffsetColuna INTEGER);";
                new SqliteCommand(createConfigTable, connection).ExecuteNonQuery();

                string insertSociosConfig = "INSERT INTO Configuracoes (Nome, Calor, Velocidade, OffsetLinha, OffsetColuna) VALUES ('Socios', 12, 4, 20, 30);";
                new SqliteCommand(insertSociosConfig, connection).ExecuteNonQuery();

                string insertContribConfig = "INSERT INTO Configuracoes (Nome, Calor, Velocidade, OffsetLinha, OffsetColuna) VALUES ('Contribuintes', 12, 4, 20, 30);";
                new SqliteCommand(insertContribConfig, connection).ExecuteNonQuery();
            }
        }

        // --- MÉTODOS PARA SÓCIOS ---
        public static List<Socio> GetAllSocios()
        {
            var pessoas = new List<Socio>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand("SELECT * FROM Socios;", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pessoas.Add(new Socio
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NomeCompleto = reader["NomeCompleto"].ToString(),
                            Cpf = reader["Cpf"].ToString(),
                            Matricula = reader["Matricula"].ToString(),
                            Cidade = reader["Cidade"].ToString(),
                            Telefone = reader["Telefone"].ToString(),
                            Email = reader["Email"].ToString(),
                            Observacao = reader["Observacao"].ToString(),
                            // ################### INÍCIO DA CORREÇÃO ###################
                            Funcao = reader["Funcao"].ToString(),
                            Empresa = reader["Empresa"].ToString(),
                            Ctps = reader["Ctps"].ToString(),
                            NumeroDependentes = reader["NumeroDependentes"].ToString()
                            // ################### FIM DA CORREÇÃO ###################
                        });
                    }
                }
            }
            return pessoas;
        }

        public static void AddSocio(Socio pessoa)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // ################### INÍCIO DA CORREÇÃO ###################
                string query = @"
                    INSERT INTO Socios (NomeCompleto, Cpf, Matricula, Cidade, Telefone, Email, Observacao, Funcao, Empresa, Ctps, NumeroDependentes)
                    VALUES (@Nome, @Cpf, @Matricula, @Cidade, @Telefone, @Email, @Obs, @Funcao, @Empresa, @Ctps, @Dep);";
                // ################### FIM DA CORREÇÃO ###################
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nome", pessoa.NomeCompleto);
                    command.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
                    command.Parameters.AddWithValue("@Matricula", pessoa.Matricula);
                    command.Parameters.AddWithValue("@Cidade", pessoa.Cidade);
                    command.Parameters.AddWithValue("@Telefone", pessoa.Telefone);
                    command.Parameters.AddWithValue("@Email", pessoa.Email);
                    command.Parameters.AddWithValue("@Obs", pessoa.Observacao);
                    // ################### INÍCIO DA CORREÇÃO ###################
                    command.Parameters.AddWithValue("@Funcao", pessoa.Funcao);
                    command.Parameters.AddWithValue("@Empresa", pessoa.Empresa);
                    command.Parameters.AddWithValue("@Ctps", pessoa.Ctps);
                    command.Parameters.AddWithValue("@Dep", pessoa.NumeroDependentes);
                    // ################### FIM DA CORREÇÃO ###################
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateSocio(Socio pessoa)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // ################### INÍCIO DA CORREÇÃO ###################
                var query = @"
                    UPDATE Socios SET
                        NomeCompleto = @Nome, Cpf = @Cpf, Matricula = @Matricula, Cidade = @Cidade,
                        Telefone = @Telefone, Email = @Email, Observacao = @Obs, Funcao = @Funcao, Empresa = @Empresa, Ctps = @Ctps, NumeroDependentes = @Dep
                    WHERE Id = @Id;";
                // ################### FIM DA CORREÇÃO ###################
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", pessoa.Id);
                    command.Parameters.AddWithValue("@Nome", pessoa.NomeCompleto);
                    command.Parameters.AddWithValue("@Cpf", pessoa.Cpf);
                    command.Parameters.AddWithValue("@Matricula", pessoa.Matricula);
                    command.Parameters.AddWithValue("@Cidade", pessoa.Cidade);
                    command.Parameters.AddWithValue("@Telefone", pessoa.Telefone);
                    command.Parameters.AddWithValue("@Email", pessoa.Email);
                    command.Parameters.AddWithValue("@Obs", pessoa.Observacao);
                    // ################### INÍCIO DA CORREÇÃO ###################
                    command.Parameters.AddWithValue("@Funcao", pessoa.Funcao);
                    command.Parameters.AddWithValue("@Empresa", pessoa.Empresa);
                    command.Parameters.AddWithValue("@Ctps", pessoa.Ctps);
                    command.Parameters.AddWithValue("@Dep", pessoa.NumeroDependentes);
                    // ################### FIM DA CORREÇÃO ###################
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSocio(int id) => Delete("Socios", id);

        // --- MÉTODOS PARA CONTRIBUINTES ---
        public static List<Contribuintes> GetAllContribuintes() => GetAll("Contribuintes");
        public static void AddContribuinte(Contribuintes p) => Add("Contribuintes", p);
        public static void UpdateContribuinte(Contribuintes p) => Update("Contribuintes", p);
        public static void DeleteContribuinte(string id) => DeleteContribuinteById(id);


        // --- MÉTODOS GENÉRICOS ---
        private static List<Contribuintes> GetAll(string tableName)
        {
            var contribuintes = new List<Contribuintes>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand($"SELECT * FROM {tableName};", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var contribuinte = new Contribuintes
                        {
                            // Assegura que o tipo de dado da coluna 'ID_Original' no banco de dados (TEXT)
                            // seja compatível com a propriedade da classe (string).
                            ID_Original = reader["ID_Original"].ToString(),
                            Inscricao = reader["Inscricao"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            Dependente = reader["Dependente"].ToString(),
                            Trabalho = reader["Trabalho"].ToString(),
                            Endereco = reader["Endereco"].ToString(),
                        };
                        contribuintes.Add(contribuinte);
                    }
                }
            }
            return contribuintes;
        }
        private static void Add(string tableName, Contribuintes contribuinte)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // ################### INÍCIO DA CORREÇÃO ###################
                // Corrigindo a query de INSERT para incluir o ID_Original
                string query = $"INSERT INTO {tableName} (ID_Original, Inscricao, Nome, Dependente, Trabalho, Endereco) VALUES (@ID_Original, @Inscricao, @Nome, @Dependente, @Trabalho, @Endereco);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Original", contribuinte.ID_Original);
                    command.Parameters.AddWithValue("@Inscricao", contribuinte.Inscricao);
                    command.Parameters.AddWithValue("@Nome", contribuinte.Nome);
                    command.Parameters.AddWithValue("@Dependente", contribuinte.Dependente);
                    command.Parameters.AddWithValue("@Trabalho", contribuinte.Trabalho);
                    command.Parameters.AddWithValue("@Endereco", contribuinte.Endereco);
                    // ################### FIM DA CORREÇÃO ###################
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void Update(string tableName, Contribuintes contribuinte)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // ################### INÍCIO DA CORREÇÃO ###################
                // Corrigindo a query de UPDATE para corresponder às colunas do JSON e a cláusula WHERE
                var query = $@"UPDATE {tableName} SET Inscricao = @Inscricao, Nome = @Nome, Dependente = @Dependente, Trabalho = @Trabalho,
                                  Endereco = @Endereco WHERE ID_Original = @ID_Original;";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Original", contribuinte.ID_Original);
                    command.Parameters.AddWithValue("@Inscricao", contribuinte.Inscricao);
                    command.Parameters.AddWithValue("@Nome", contribuinte.Nome);
                    command.Parameters.AddWithValue("@Dependente", contribuinte.Dependente);
                    command.Parameters.AddWithValue("@Trabalho", contribuinte.Trabalho);
                    command.Parameters.AddWithValue("@Endereco", contribuinte.Endereco);
                    // ################### FIM DA CORREÇÃO ###################
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void Delete(string tableName, int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand($"DELETE FROM {tableName} WHERE Id = @Id;", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Método de deleção para a tabela de Contribuintes, que usa ID_Original como chave
        private static void DeleteContribuinteById(string id)
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

        // --- MÉTODOS PARA CONFIGURAÇÕES (NÃO PRECISAM DE ALTERAÇÃO) ---
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