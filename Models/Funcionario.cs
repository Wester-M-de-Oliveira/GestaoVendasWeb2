using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models 
{
    public class Funcionario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("cpf")]
        public string CPF { get; set; }
        [Column("rg")]
        public string RG { get; set; }
        [Column("data_nasc")]
        public DateTime? DataNascimento { get; set; }
        [Column("salario")]
        public double Salario { get; set; }
        [Column("telefone")]
        public string Telefone { get; set; }
        [Column("celular")]
        public string Celular { get; set; }
        [Column("funcao")]
        public string Funcao { get; set; }
        [Column("sexo")]
        public string Sexo { get; set; }
        [ForeignKey("endereco_id")]
        public int IdEndereco { get; set; }
        [Column("endereco")]
        public Endereco Endereco { get; set; }
    }
}