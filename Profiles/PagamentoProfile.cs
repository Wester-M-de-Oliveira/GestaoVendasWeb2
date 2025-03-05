using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;

public class PagamentoProfile : Profile
{
    public PagamentoProfile()
    {
        CreateMap<Pagamento, PagamentoDTO>();
        CreateMap<PagamentoDTO, Pagamento>();
        
        CreateMap<PagamentoCreateDTO, Pagamento>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateTime.Now));
            
        CreateMap<PagamentoUpdateDTO, Pagamento>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));
    }
}