using AutoMapper;
using TMS.DTOs;
using TMS.Models;

namespace TMS
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProjectTaskDto, ProjectTask>(); 
            CreateMap<ProjectTask, ProjectTaskDto>(); 

            CreateMap<ProjectDto, Project>();   
            CreateMap<Project, ProjectDto>();   

        }
    }
}
