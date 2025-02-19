using Microsoft.EntityFrameworkCore;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.DataContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets das tabelas
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItensCompra> ItensCompras { get; set; }
        public DbSet<Caixa> Caixas { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItensVenda> ItensVendas { get; set; }
        public DbSet<Recebimento> Recebimentos { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relação Estado -> Cidade
            modelBuilder.Entity<Cidade>()
                .HasOne(c => c.Estado)
                .WithMany(e => e.Cidades)
                .HasForeignKey(c => c.EstadoId);

            // Relação Cidade -> Endereco
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Cidade)
                .WithMany(c => c.Enderecos)
                .HasForeignKey(e => e.CidadeId);

            // Relação Endereco -> Cliente
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Endereco)
                .WithMany(e => e.Clientes)
                .HasForeignKey(c => c.EnderecoId);

            // Relação Endereco -> Funcionario
            modelBuilder.Entity<Funcionario>()
                .HasOne(f => f.Endereco)
                .WithMany(e => e.Funcionarios)
                .HasForeignKey(f => f.EnderecoId);

            // Relação Endereco -> Fornecedor
            modelBuilder.Entity<Fornecedor>()
                .HasOne(f => f.Endereco)
                .WithMany(e => e.Fornecedores)
                .HasForeignKey(f => f.EnderecoId);

            // Relação Compra -> Fornecedor
            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Fornecedor)
                .WithMany(f => f.Compras)
                .HasForeignKey(c => c.FornecedorId);

            // Relação Compra -> Funcionario
            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Funcionario)
                .WithMany(f => f.Compras)
                .HasForeignKey(c => c.FuncionarioId);

            // Relação ItensCompra -> Compra e Produto
            modelBuilder.Entity<ItensCompra>()
                .HasOne(ic => ic.Compra)
                .WithMany(c => c.ItensCompras)
                .HasForeignKey(ic => ic.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItensCompra>()
                .HasOne(ic => ic.Produto)
                .WithMany(p => p.ItensCompras)
                .HasForeignKey(ic => ic.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação Venda -> Cliente e Funcionario
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteId);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Funcionario)
                .WithMany(f => f.Vendas)
                .HasForeignKey(v => v.FuncionarioId);

            // Relação ItensVenda -> Venda e Produto
            modelBuilder.Entity<ItensVenda>()
                .HasOne(iv => iv.Venda)
                .WithMany(v => v.ItensVendas)
                .HasForeignKey(iv => iv.VendaId);

            modelBuilder.Entity<ItensVenda>()
                .HasOne(iv => iv.Produto)
                .WithMany(p => p.ItensVendas)
                .HasForeignKey(iv => iv.ProdutoId);

            // Relação Recebimento -> Caixa, Venda e Funcionario
            modelBuilder.Entity<Recebimento>()
                .HasOne(r => r.Caixa)
                .WithMany(c => c.Recebimentos)
                .HasForeignKey(r => r.CaixaId);

            modelBuilder.Entity<Recebimento>()
                .HasOne(r => r.Venda)
                .WithMany(v => v.Recebimentos)
                .HasForeignKey(r => r.VendaId);

            modelBuilder.Entity<Recebimento>()
                .HasOne(r => r.Funcionario)
                .WithMany(f => f.Recebimentos)
                .HasForeignKey(r => r.FuncionarioId);

            // Relação Pagamento -> Caixa, Compra, Despesa e Funcionario
            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Caixa)
                .WithMany(c => c.Pagamentos)
                .HasForeignKey(p => p.CaixaId);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Compra)
                .WithMany(c => c.Pagamentos)
                .HasForeignKey(p => p.CompraId);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Despesa)
                .WithMany(d => d.Pagamentos)
                .HasForeignKey(p => p.DespesaId);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Funcionario)
                .WithMany(f => f.Pagamentos)
                .HasForeignKey(p => p.FuncionarioId);
        }
    }
}
