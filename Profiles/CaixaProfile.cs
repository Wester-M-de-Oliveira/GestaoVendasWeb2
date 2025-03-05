using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map from Model to DTO
            CreateMap<Caixa, CaixaDTO>();
            
            // Map from CreateUpdateDTO to Model
            CreateMap<CaixaCreateUpdateDTO, Caixa>();
            
            // Map from Model to CreateUpdateDTO
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

