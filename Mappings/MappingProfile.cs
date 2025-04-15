using AutoMapper;
using StaffTrack.API.DTOs;
using StaffTrack.API.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Department, DepartmentDTO>().ReverseMap();
        CreateMap<Report, ReportDTO>().ReverseMap();
    }
}
