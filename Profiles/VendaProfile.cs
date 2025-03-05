using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles;

public class VendaProfile : Profile
{
    public VendaProfile()
    {
        CreateMap<Venda, VendaDTO>()
            .ForMember(dest => dest.ItensVendas, opt => opt.MapFrom(src => src.ItensVendas))
            .ForMember(dest => dest.Funcionario, opt => opt.MapFrom(src => src.Funcionario))
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Recebimentos, opt => opt.MapFrom(src => src.Recebimentos));
            
        CreateMap<VendaDTO, Venda>();
        
        CreateMap<VendaCreateDTO, Venda>()
            .ForMember(dest => dest.DataVenda, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.ValorTotal));
            
        CreateMap<VendaUpdateDTO, Venda>()
            .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.ValorTotal))
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));
                
        CreateMap<ItensVenda, ItensVendaDTO>()
            .ForMember(dest => dest.Produto, opt => opt.MapFrom(src => src.Produto));
            
        CreateMap<ItensVendaDTO, ItensVenda>();
        
        CreateMap<ItensVendaCreateDTO, ItensVenda>();
    }
}