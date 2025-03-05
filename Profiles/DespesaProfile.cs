using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;
public class DespesaProfile : Profile
{
    public DespesaProfile()
    {
        CreateMap<Despesa, DespesaDTO>()
            .ForMember(dest => dest.Fornecedor, opt => opt.MapFrom(src => src.Fornecedor))
            .ForMember(dest => dest.Pagamentos, opt => opt.MapFrom(src => src.Pagamentos));

        CreateMap<DespesaDTO, Despesa>();
        
        CreateMap<DespesaCreateDTO, Despesa>();
        CreateMap<DespesaUpdateDTO, Despesa>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));
    }
}