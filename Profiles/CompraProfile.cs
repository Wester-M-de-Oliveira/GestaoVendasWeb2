using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;

public class CompraProfile : Profile
{
    public CompraProfile()
    {
        CreateMap<Compra, CompraDTO>();
        CreateMap<CompraDTO, Compra>();
        CreateMap<CompraCreateDto, Compra>();
        CreateMap<CompraUpdateDto, Compra>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));
                
        CreateMap<ItensCompra, ItensCompraDTO>();
        CreateMap<ItensCompraDTO, ItensCompra>();
    }
}

