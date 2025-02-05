using AutoMapper;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles
{
    public class CompraProfile : Profile
    {
        public CompraProfile()
        {
            CreateMap<Compra, CompraDTO>();
            CreateMap<CompraDTO, Compra>();
            CreateMap<ItensCompra, ItensCompraDTO>();
            CreateMap<ItensCompraDTO, ItensCompra>();
        }
    }
}