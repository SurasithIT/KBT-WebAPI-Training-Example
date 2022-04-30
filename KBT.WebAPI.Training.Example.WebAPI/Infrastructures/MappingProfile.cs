using System.Data;
using System.Net.Mime;
using AutoMapper;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.WebAPI.Models.Requests.Users;
using KBT.WebAPI.Training.Example.WebAPI.Models.Users;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace KBT.WebAPI.Training.Example.WebAPI.Infrastructures;

public class MappingProfile : Profile 
{
    public MappingProfile()
    {
        CreateMap<IDataReader, User>()
            .ForMember(dest => dest.UserKey, opt => opt.MapFrom(src => src["UserKey"]))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src["UserName"]))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src["Password"]))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src["IsActive"] == DBNull.Value ? null : src["IsActive"]))
            .ForMember(dest => dest.EmployeeKey,
                opt => opt.MapFrom(src => src["EmployeeKey"] == DBNull.Value ? null : src["EmployeeKey"]));

        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<User, UserReq>().ReverseMap();
    }
}