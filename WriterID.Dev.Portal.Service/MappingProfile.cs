using AutoMapper;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Model.DTOs.User;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<UserForRegistrationDto, User>();

        // Dataset Mappings
        CreateMap<Dataset, DatasetDto>().ReverseMap();
        CreateMap<CreateDatasetRequestDto, Dataset>();

        // Model Mappings
        CreateMap<WriterIdentificationModel, ModelDto>().ReverseMap();

        // Task Mappings
        CreateMap<WriterIdentificationTask, TaskDto>().ReverseMap();
    }
} 