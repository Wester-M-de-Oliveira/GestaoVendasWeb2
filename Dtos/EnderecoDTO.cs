using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class EnderecoDTO
    {
        [Required(ErrorMessage = "Rua é obrigatória")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "Número é obrigatório")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "CidadeId é obrigatório")]
        public int CidadeId { get; set; }
    }
}
