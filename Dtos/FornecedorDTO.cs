using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class FornecedorDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public int Id { get; set; }

        [StringLength(200, ErrorMessage = "Razão Social pode ter no máximo 200 caracteres")]
        public string? RazaoSocial { get; set; }

        [StringLength(100, ErrorMessage = "Nome Fantasia pode ter no máximo 100 caracteres")]
        public string? NomeFantasia { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public int IdEndereco { get; set; }
    }
}