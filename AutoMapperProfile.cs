using AutoMapper;
using TMS.DTOs;
using TMS.Models;

namespace TMS
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddProjectTaskDto, ProjectTask>(); 
            CreateMap<ProjectTask, AddProjectTaskDto>(); 

            CreateMap<ProjectDto, Project>();   
            CreateMap<Project, ProjectDto>();   

        }
    }
}
