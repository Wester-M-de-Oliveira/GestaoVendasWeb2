using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles;

public class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<Produto, ProdutoDTO>();
        CreateMap<ProdutoDTO, Produto>();
    }
}
