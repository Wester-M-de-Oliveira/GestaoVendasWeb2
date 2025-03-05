using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles
{
    public class RecebimentoProfile : Profile
    {
        public RecebimentoProfile()
        {
            CreateMap<Recebimento, RecebimentoDTO>()
                .ForMember(dest => dest.Caixa, opt => opt.MapFrom(src => src.Caixa))
                .ForMember(dest => dest.Venda, opt => opt.MapFrom(src => src.Venda))
                .ForMember(dest => dest.Funcionario, opt => opt.MapFrom(src => src.Funcionario));

            CreateMap<RecebimentoDTO, Recebimento>();
            
            // Map for RecebimentoCreateDTO with explicit property mapping
            CreateMap<RecebimentoCreateDTO, Recebimento>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.CaixaId, opt => opt.MapFrom(src => src.CaixaId))
                .ForMember(dest => dest.VendaId, opt => opt.MapFrom(src => src.VendaId))
                .ForMember(dest => dest.FuncionarioId, opt => opt.MapFrom(src => src.FuncionarioId));
                
            // Map for RecebimentoUpdateDTO with conditional mapping
            CreateMap<RecebimentoUpdateDTO, Recebimento>()
                .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null));

            // Maps for related entities
            CreateMap<Caixa, CaixaDTO>();
            CreateMap<Venda, VendaDTO>();
            CreateMap<Funcionario, FuncionarioDTO>();
        }
    }
}