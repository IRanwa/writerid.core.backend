using AutoMapper;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Model.DTOs.User;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Service.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Task Mappings
        CreateMap<WriterIdentificationTask, TaskDto>();
        CreateMap<CreateTaskDto, WriterIdentificationTask>();
        CreateMap<UpdateTaskDto, WriterIdentificationTask>();

        // Dataset Mappings
        CreateMap<Dataset, DatasetDto>();
        CreateMap<CreateDatasetDto, Dataset>();
        CreateMap<UpdateDatasetDto, Dataset>();

        // Model Mappings
        CreateMap<WriterIdentificationModel, ModelDto>();
        CreateMap<CreateModelDto, WriterIdentificationModel>();
        CreateMap<UpdateModelDto, WriterIdentificationModel>();

        // User Mappings
        CreateMap<UserForRegistrationDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));
    }
} 