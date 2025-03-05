using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Dtos
{
    public class FornecedorDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(200, ErrorMessage = "Razão Social pode ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "Nome Fantasia pode ter no máximo 100 caracteres")]
        public string NomeFantasia { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public EnderecoDTO Endereco { get; set; }
    }
    public class FornecedorUpdateDTO
    {
        [StringLength(200, ErrorMessage = "Razão Social pode ter no máximo 200 caracteres")]
        public string? RazaoSocial { get; set; }

        [StringLength(100, ErrorMessage = "Nome Fantasia pode ter no máximo 100 caracteres")]
        public string? NomeFantasia { get; set; }

        public UpdateEnderecoDTO? Endereco { get; set; }
    }
}