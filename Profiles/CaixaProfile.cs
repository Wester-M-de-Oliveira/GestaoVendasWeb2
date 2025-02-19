using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Caixa, CaixaDTO>().ReverseMap();
        }
    }
}
