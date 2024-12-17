using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class FuncionarioDTO
    {
        [Required(ErrorMessage = "Campo obrigat�rio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [MinLength(1, ErrorMessage = "Nome deve ter no m�nimo 1 caractere")]
        [MaxLength(200, ErrorMessage = "Nome pode ter no m�ximo 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [StringLength(20, ErrorMessage = "CPF pode ter no m�ximo 20 caracteres")]
        public string Cpf { get; set; }

        [StringLength(20, ErrorMessage = "RG pode ter no m�ximo 20 caracteres")]
        public string? Rg { get; set; }

        public DateTime? DataNasc { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sal�rio deve ser positivo")]
        public double Salario { get; set; }

        [StringLength(50, ErrorMessage = "Telefone pode ter no m�ximo 50 caracteres")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [StringLength(50, ErrorMessage = "Celular pode ter no m�ximo 50 caracteres")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [StringLength(50, ErrorMessage = "Fun��o pode ter no m�ximo 50 caracteres")]
        public string Funcao { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [StringLength(20, ErrorMessage = "Sexo pode ter no m�ximo 20 caracteres")]
        public string Sexo { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        public int IdEndereco { get; set; }
    }
}

