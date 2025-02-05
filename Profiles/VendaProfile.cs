using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;
public class VendaProfile : Profile
{
    public VendaProfile()
    {
        CreateMap<Venda, VendaDTO>();
        CreateMap<VendaDTO, Venda>();
        CreateMap<ItensVenda, ItensVendaDTO>();
        CreateMap<ItensVendaDTO, ItensVenda>();
    }
}