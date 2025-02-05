
using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;
public class DespesaProfile : Profile
{
    public DespesaProfile()
    {
        CreateMap<Despesa, DespesaDTO>();
        CreateMap<DespesaDTO, Despesa>();
    }
}