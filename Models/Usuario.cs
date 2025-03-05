using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasWeb2.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [Column("nome")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [Column("role")]
        [StringLength(50)]
        public string Role { get; set; } = "user";

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Column("ultimo_acesso")]
        public DateTime? UltimoAcesso { get; set; }
    }
}
