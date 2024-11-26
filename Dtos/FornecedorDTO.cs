using System;

public class FornecedorDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [Phone(ErrorMessage = "Número de telefone inválido")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public string Cidade { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public string Endereco { get; set; }
}
