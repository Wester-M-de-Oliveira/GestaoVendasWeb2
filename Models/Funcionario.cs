using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public ICollection<Compra> Compras { get; set; }

        [JsonIgnore]
        public ICollection<Venda> Vendas { get; set; }

        [JsonIgnore]
        public ICollection<Recebimento> Recebimentos { get; set; }

        [JsonIgnore]
        public ICollection<Pagamento> Pagamentos { get; set; }
    }
}