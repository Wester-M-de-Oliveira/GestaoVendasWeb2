
using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

namespace GestaoVendasWeb2.Profiles;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<Cliente, ClienteDTO>();
        CreateMap<ClienteDTO, Cliente>();
    }
}