using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoVendasWeb2.Dtos
{
    public class CaixaDTO
    {
        public int Id { get; set; }

        public DateTime DataAbertura { get; set; }

        public DateTime? DataFechamento { get; set; }

        public decimal ValorAbertura { get; set; }

        public decimal ValorFechamento { get; set; }

        public decimal ValorCreditos { get; set; }

        public decimal ValorDebitos { get; set; }

        public decimal SaldoFinal { get; set; }

        public bool Status { get; set; }

        public int FuncionarioId { get; set; }
    }

    public class CaixaCreateUpdateDTO
    {
        [Required(ErrorMessage = "A data de abertura é obrigatória")]
        [Display(Name = "Data de Abertura")]
        [DataType(DataType.DateTime)]
        public DateTime DataAbertura { get; set; }

        [Display(Name = "Data de Fechamento")]
        [DataType(DataType.DateTime)]
        public DateTime? DataFechamento { get; set; }

        [Required(ErrorMessage = "O saldo inicial é obrigatório")]
        [Display(Name = "Saldo Inicial")]
        [Range(0, double.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo")]
        public decimal SaldoInicial { get; set; }

        [Display(Name = "Troco")]
        [Range(0, double.MaxValue, ErrorMessage = "O troco não pode ser negativo")]
        public decimal Troco { get; set; }

        [Display(Name = "Valor Créditos")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor de créditos não pode ser negativo")]
        public decimal ValorCreditos { get; set; }

        [Display(Name = "Valor Débitos")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor de débitos não pode ser negativo")]
        public decimal ValorDebitos { get; set; }

        [Display(Name = "Saldo Final")]
        public decimal SaldoFinal { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
}