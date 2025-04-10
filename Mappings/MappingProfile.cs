using AutoMapper;
using TransactionsAPI.Models;
using TransactionsAPI.DTOs;

namespace TransactionsAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserInfoDTO>();
        CreateMap<Transaction, TransactionReadDTO>();
    }
}
