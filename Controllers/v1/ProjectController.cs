using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.DTOs;
using TMS.HelperFunctions;
using TMS.Interfaces;
using TMS.Request;
using TMS.Responses;

namespace TMS.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Add a new project
        /// </summary>
        [HttpPost("addproject")]
        public async Task<ActionResult<ServiceResponse>> AddProject(ProjectDto request)
        {
            var response = await _projectService.AddProject(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get user's projects
        /// </summary>
        [HttpGet("getprojects")]
        public async Task<ActionResult<ServiceResponse>> GetProjects()
        {
            var response = await _projectService.GetProjects();
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Add Task to an existing Project
        /// </summary>
        [HttpPost("addtasktoproject")]
        public async Task<ActionResult<ServiceResponse>> AddTaskToProject(AddProjectTaskDto request)
        {
            var response = await _projectService.AddTaskToProject(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get Task by status or by priority
        /// </summary>
        [HttpGet("gettaskbystatusorpriority")]
        public async Task<ActionResult<ServiceResponse>> GetTaskByStatusOrPriority([StatusAndPriority] string query)
        {
            var response = await _projectService.GetTaskByStatusOrPriority(query);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Delete a project using its unique GUID
        /// </summary>
        [HttpPost("deleteproject")]
        public async Task<ActionResult<ServiceResponse>> DeleteProject(Guid query)
        {
            var response = await _projectService.DeleteProject(query);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Delete a task from a  project using its unique GUID
        /// </summary>
        [HttpPost("deletetask")]
        public async Task<ActionResult<ServiceResponse>> DeleteTask(Guid query)
        {
            var response = await _projectService.DeleteTask(query);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPost("updatetaskstatus")]
        public async Task<ActionResult<ServiceResponse>> UpdateTaskStatus(UpdateTaskStatus request)
        {
            var response = await _projectService.UpdateTaskStatus(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update task priority
        /// </summary>
        [HttpPost("updatetaskpriority")]
        public async Task<ActionResult<ServiceResponse>> UpdateTaskPriority(UpdateTaskPriority request)
        {
            var response = await _projectService.UpdateTaskPriority(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get due tasks by week
        /// </summary>
        [HttpGet("getduetasksbyweek")]
        public async Task<ActionResult<ServiceResponse>> GetDueTasksByWeek ()
        {
            var response = await _projectService.GetDueTasksByWeek();
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get user notifications
        /// </summary>
        [HttpGet("getnotifications")]
        public async Task<ActionResult<ServiceResponse>> GetNotifications()
        {
            var response = await _projectService.GetNotifications();
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Change a notification status to read
        /// </summary>
        [HttpPost("readnotifications")]
        public async Task<ActionResult<ServiceResponse>> ReadNotification(Guid id)
        {
            var response = await _projectService.ReadNotification(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        } 

        /// <summary>
        /// CRON Job to send notifications about tasks expiring in 48 hours
        /// </summary>
        [HttpPost("runbackgroundnotifications")]
        public async Task<ActionResult<ServiceResponse>> RunBackgroundNotifications()
        {
            var response = await _projectService.RunBackgroundNotifications();
            if (!response.Success)
            {
                return BadRequest(response);
            }

            //this cron job gets triggered every 30 minute every day
            RecurringJob.AddOrUpdate("sendOtpMails", () => _projectService.RunBackgroundNotifications(), "*/30 * * * *");

            return Ok(response);
        }
    }
}
