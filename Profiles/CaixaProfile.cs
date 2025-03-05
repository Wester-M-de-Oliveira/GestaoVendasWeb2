using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Caixa, CaixaDTO>();
            
            CreateMap<CaixaCreateUpdateDTO, Caixa>();
            
            CreateMap<Caixa, CaixaCreateUpdateDTO>();
        }
    }

    public class CaixaProfile : Profile
    {
        public CaixaProfile()
        {
            CreateMap<Caixa, CaixaDTO>();
            CreateMap<CaixaDTO, Caixa>();
        }
    }
}

