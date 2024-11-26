using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class ClienteDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [Phone(ErrorMessage = "Número de telefone inválido")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public string Endereco { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string? Email { get; set; }
}
