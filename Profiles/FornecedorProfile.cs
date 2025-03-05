
using AutoMapper;
using GestaoVendasWeb2.Dtos;
using GestaoVendasWeb2.Models;

namespace GestaoVendasWeb2.Profiles;

public class FornecedorProfile : Profile
{
    public FornecedorProfile()
    {
        CreateMap<Fornecedor, FornecedorDTO>();
        CreateMap<FornecedorDTO, Fornecedor>();
    }
}