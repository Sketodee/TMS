using TMS.DTOs;
using TMS.Request;
using TMS.Responses;

namespace TMS.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResponse> AddProject(ProjectDto request);
        Task<ServiceResponse> GetProjects();
        Task<ServiceResponse> AddTaskToProject(AddProjectTaskDto request);
        Task<ServiceResponse> GetTaskByStatusOrPriority(string query);
        Task<ServiceResponse> DeleteProject(Guid query); 
        Task<ServiceResponse> DeleteTask(Guid query);  
        Task<ServiceResponse> UpdateTaskStatus(UpdateTaskStatus request);
        Task<ServiceResponse> UpdateTaskPriority(UpdateTaskPriority request);
        Task<ServiceResponse> GetDueTasksByWeek();
        Task<ServiceResponse> GetNotifications(); 
        Task<ServiceResponse> ReadNotification(Guid id);
        Task<ServiceResponse> RunBackgroundNotifications();

    }
}
