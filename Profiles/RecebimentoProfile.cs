
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

            CreateMap<Caixa, CaixaDTO>();
            CreateMap<Venda, VendaDTO>();
            CreateMap<Funcionario, FuncionarioDTO>();
        }
    }
}