using TMS.DTOs;
using TMS.Request;
using TMS.Responses;

namespace TMS.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResponse> AddProject(ProjectDto request);
        Task<ServiceResponse> GetProjects();
        Task<ServiceResponse> AddTaskToProject(ProjectTaskDto request);
        Task<ServiceResponse> GetTaskByStatusOrPriority(string query);
        Task<ServiceResponse> DeleteProject(Guid query); 
        Task<ServiceResponse> DeleteTask(Guid query);  
        Task<ServiceResponse> UpdateTaskStatus(UpdateTask request);
        Task<ServiceResponse> UpdateTaskPriority(UpdateTask request);
        Task<ServiceResponse> GetDueTasksByWeek();
        Task<ServiceResponse> GetNotifications(); 
        Task<ServiceResponse> ReadNotification(Guid id);
        Task<ServiceResponse> RunBackgroundNotifications();

    }
}
