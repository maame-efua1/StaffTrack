﻿using AutoMapper;
using StaffTrackShared.DTOs;
using StaffTrackAPI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Department, DepartmentDTO>().ReverseMap();
        CreateMap<Report, ReportDTO>().ReverseMap();
        CreateMap<Notification, NotificationDTO>().ForMember(dest => dest.TimestampFormatted, opt => opt.Ignore()).ReverseMap();
    }
}
