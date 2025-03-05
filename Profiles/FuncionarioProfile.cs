using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;

public class FuncionarioProfile : Profile
{
    public FuncionarioProfile()
    {
        CreateMap<Funcionario, FuncionarioDTO>();
        CreateMap<FuncionarioDTO, Funcionario>();
    }
}
