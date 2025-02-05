using AutoMapper;
using GestaoVendasWeb2.Models;
using GestaoVendasWeb2.Dtos;

public class PagamentoProfile : Profile
{
    public PagamentoProfile()
    {
        CreateMap<Pagamento, PagamentoDTO>();
        CreateMap<PagamentoDTO, Pagamento>();
    }
}