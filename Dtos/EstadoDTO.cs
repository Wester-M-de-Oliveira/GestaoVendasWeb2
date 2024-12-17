using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class EstadoDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MinLength(1, ErrorMessage = "Nome deve ter no mínimo 1 caractere")]
        [MaxLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(2, ErrorMessage = "Sigla deve ter exatamente 2 caracteres")]
        public string Sigla { get; set; }
    }
}
