using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class ClienteDTO
{
    [Required(ErrorMessage = "Campo obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [MinLength(1, ErrorMessage = "Nome deve ter no mínimo 1 caractere")]
    [MaxLength(200, ErrorMessage = "Nome pode ter no máximo 200 caracteres")]
    public string Nome { get; set; }

    [StringLength(50, ErrorMessage = "Estado civil pode ter no máximo 50 caracteres")]
    public string? EstadoCivil { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(20, ErrorMessage = "CPF pode ter no máximo 20 caracteres")]
    public string Cpf { get; set; }

    [StringLength(30, ErrorMessage = "RG pode ter no máximo 30 caracteres")]
    public string? Rg { get; set; }

    public DateTime? DataNasc { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Renda familiar deve ser positiva")]
    public float? RendaFamiliar { get; set; }

    [StringLength(50, ErrorMessage = "Telefone pode ter no máximo 50 caracteres")]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(20, ErrorMessage = "Sexo pode ter no máximo 20 caracteres")]
    public string Sexo { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(50, ErrorMessage = "Celular pode ter no máximo 50 caracteres")]
    public string Celular { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public int IdEndereco { get; set; }
}
