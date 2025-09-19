namespace ImpressoraEtiquetas
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Matricula { get; set; }
        public string Cidade { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }

        // --- NOVOS CAMPOS PARA O CARTÃO DE SÓCIO ---
        public string Funcao { get; set; }
        public string Empresa { get; set; }
        public string Ctps { get; set; }
        public string NumeroDependentes { get; set; } // Usando string para flexibilidade
    }
}