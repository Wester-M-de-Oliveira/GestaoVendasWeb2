using GestaoVendasWeb2.Models;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class FuncionarioDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MinLength(1, ErrorMessage = "Nome deve ter no mínimo 1 caractere")]
        [MaxLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(20, ErrorMessage = "CPF pode ter no máximo 20 caracteres")]
        public string CPF { get; set; }

        [StringLength(20, ErrorMessage = "RG pode ter no máximo 20 caracteres")]
        public string? RG { get; set; }

        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salário deve ser positivo")]
        public double Salario { get; set; }

        [StringLength(50, ErrorMessage = "Telefone pode ter no máximo 50 caracteres")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50, ErrorMessage = "Função pode ter no máximo 50 caracteres")]
        public string Funcao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(20, ErrorMessage = "Sexo pode ter no máximo 20 caracteres")]
        public string Sexo { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public Endereco Endereco { get; set; }
    }
}
