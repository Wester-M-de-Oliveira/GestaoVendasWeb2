using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CidadeDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caractere")]
    [MaxLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public int IdEstado { get; set; }
}