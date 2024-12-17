using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class EnderecoDTO
    {
        [Required(ErrorMessage = "Campo obrigat�rio")]
        public int Id { get; set; }

        [StringLength(300, ErrorMessage = "Rua pode ter no m�ximo 300 caracteres")]
        public string? Rua { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "N�mero deve ser positivo")]
        public int? Numero { get; set; }

        [StringLength(100, ErrorMessage = "Bairro pode ter no m�ximo 100 caracteres")]
        public string? Bairro { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        public int IdCidade { get; set; }
    }
}
