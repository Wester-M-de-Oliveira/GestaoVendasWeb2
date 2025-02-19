using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models 
{
    [Table("funcionario")]
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

        [Column("funcao")]
        public string Funcao { get; set; }

        [Column("sexo")]
        public string Sexo { get; set; }

        [ForeignKey("endereco_id")]
        [Column("endereco_id")]
        public int EnderecoId { get; set; }

        public Endereco Endereco { get; set; }

        public ICollection<Compra> Compras { get; set; }
        public ICollection<Venda> Vendas { get; set; }
        public ICollection<Recebimento> Recebimentos { get; set; }
        public ICollection<Pagamento> Pagamentos { get; set; }
    }
}