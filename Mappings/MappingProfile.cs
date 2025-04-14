using AutoMapper;
using StaffTrack.DTOs;
using StaffTrack.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Department, DepartmentDTO>().ReverseMap();
        CreateMap<Report, ReportDTO>().ReverseMap();
    }
}
