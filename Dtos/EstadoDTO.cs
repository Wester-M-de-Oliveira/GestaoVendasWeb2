using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class EstadoDTO
    {
        [Required(ErrorMessage = "Campo obrigat�rio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [MinLength(1, ErrorMessage = "Nome deve ter no m�nimo 1 caractere")]
        [MaxLength(200, ErrorMessage = "Nome pode ter no m�ximo 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigat�rio")]
        [StringLength(2, ErrorMessage = "Sigla deve ter exatamente 2 caracteres")]
        public string Sigla { get; set; }
    }
}
