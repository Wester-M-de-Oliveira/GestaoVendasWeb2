using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CidadeDTO
{
    [Required(ErrorMessage = "Campo obrigat�rio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Campo obrigat�rio")]
    [MinLength(3, ErrorMessage = "Nome deve ter no m�nimo 3 caractere")]
    [MaxLength(200, ErrorMessage = "Nome pode ter no m�ximo 200 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigat�rio")]
    public int IdEstado { get; set; }
}