using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using TMS.Data;
using TMS.DTOs;
using TMS.HelperFunctions;
using TMS.Interfaces;
using TMS.Models;
using TMS.Request;
using TMS.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TMS.Services
{
    public class ProjectService : IProjectService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly Helpers _helpers;

        public ProjectService(UserManager<AppUser> userManager, IMapper mapper,
            ILogger<UserService> logger, DataContext context, IHttpContextAccessor accessor, Helpers helpers)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _accessor = accessor;
            _helpers = helpers;
        }
        public async Task<ServiceResponse> AddProject(ProjectDto request)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                Project project = new Project
                {
                    UserId = returnedUser.Id,
                };

                _mapper.Map(request, project);
                _context.Add(project);
                await _context.SaveChangesAsync();

                //send notification
                var description = $"{project.Name} Project Added";
                await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);

                response.Success = true;
                response.Message = "Project successfully created"; 
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error creating Project";
                response.Errors = new[] { ex.Message }; 
            }

            return response; 
        }

        public async Task<ServiceResponse> AddTaskToProject(ProjectTaskDto request)
        {
            ServiceResponse response = new();
            List<string> errors = new List<string>();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check if project exist
                var project = await _context.Projects.Where(x => x.Id == request.ProjectId).FirstOrDefaultAsync(); 
                if(project == null)
                {
                    errors.Add("Project doesn't exist");
                } else
                {
                    var projectTask = new ProjectTask
                    {
                        UserId = returnedUser.Id
                    };
                    _mapper.Map(request, projectTask); 
                    _context.Tasks.Add(projectTask);    
                    await _context.SaveChangesAsync();

                    //send notification
                    var description = $"Task Added - {projectTask.Title} has been added to {project.Name}";
                    await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);
                }

                response.Success = true;
                response.Message = "Task successfully added"; 

                // Check if there are any errors
                if (errors.Count > 0)
                {
                    throw new AggregateException(errors.Select(e => new Exception(e)));
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                response.Message = "Can't add Task";
                response.Errors = errors;
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteProject(Guid query)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check if project exists
                var project = await _context.Projects.Where(x => x.Id == query && x.UserId == returnedUser.Id).FirstOrDefaultAsync();
                if (project == null)
                {
                    response.Success = true;
                    response.Message = "Project not found";
                }
                 else
                {
                    _context.Projects.Remove(project);   
                    await _context.SaveChangesAsync();

                    //send notification
                    var description = $"Project Deleted - {project.Name} has been deleted";
                    await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);

                    response.Success = true;
                    response.Message = "Project successfully deleted";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting project";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public async Task<ServiceResponse> DeleteTask(Guid query)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check if project exists
                var task = await _context.Tasks.Where(x => x.Id == query && x.UserId == returnedUser.Id).FirstOrDefaultAsync();
                if (task == null)
                {
                    response.Success = true;
                    response.Message = "Task not found";
                }
                else
                {
                    _context.Tasks.Remove(task);
                    await _context.SaveChangesAsync();

                    //send notification
                    var description = $"Task Deleted - {task.Title} has been deleted";
                    await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);

                    response.Success = true;
                    response.Message = "Task successfully deleted";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error deleting task";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public async Task<ServiceResponse> GetDueTasksByWeek()
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                DateTime today = DateTime.Today;
                DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(6);

                DateTime dueDate;

                var tasks =await _context.Tasks
                    .Where(task => DateTime.TryParseExact(task.DueDate, "dd/MMM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate)
                        && dueDate >= startOfWeek && dueDate <= endOfWeek && task.UserId == returnedUser.Id)
                    .ToListAsync();

                if(tasks.Count > 0)
                {
                    response.Success = true;
                    response.Message = "No task for the week";
                } else
                {
                    response.Success = true;
                    response.Message = "Weekly tasks successfully fetched";
                    response.Data = tasks;
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching weekly tasks";
                response.Errors = new[] { ex.Message }; 
            }
            return response;
        }

        public async Task<ServiceResponse> GetNotifications()
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

               //get notifications
               var notifications = await _context.Notifications.Where(x=> x.UserId == returnedUser.Id).OrderByDescending(x =>
                    DateTime.ParseExact(
                        x.CreatedOn.Replace("Sept", "Sep"), // Replace "Sept" with "Sep" in the date string
                        "dd/MMM/yyyy : HH:mm",
                        CultureInfo.InvariantCulture
                    )
                ).ToListAsync();   

                if(notifications == null)
                {
                    response.Success = true;
                    response.Message = "Notification not found"; 
                }
                else
                {
                    response.Success = true;
                    response.Message = "Notification successfully fetched";
                    response.Data = notifications;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching notifications";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse> GetProjects()
        {
            ServiceResponse response; response = new ();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                var projects = await _context.Projects.Where(x => x.UserId == returnedUser.Id).Include(x => x.Tasks).ToListAsync(); 

                if(projects.Count == 0)
                {
                    response.Success = true;
                    response.Message = "No project found";
                }
                else
                {
                    response.Success = true;
                    response.Message = "Projects successfully fetched";
                    response.Data = projects;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching projects";
                response.Errors = new[] { ex.Message };
            }

            return response; 
        }

        public async Task<ServiceResponse> GetTaskByStatusOrPriority(string query)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                var tasks = await _context.Tasks.Where(x => x.UserId == returnedUser.Id && (x.Status == query || x.Priority == query)).ToListAsync();

                if(tasks.Count == 0)
                {
                    response.Success = true;
                    response.Message = "No tasks found"; 
                }
                else
                {
                    response.Success = true;
                    response.Message = "Tasks successfully fetched";
                    response.Data = tasks;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error fetching tasks";
                response.Errors = new[] { ex.Message };
            }

            return response; 
        }

        public async Task<ServiceResponse> ReadNotification(Guid id)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check notification
                var notification = await _context.Notifications.Where(x => x.Id == id && x.UserId == returnedUser.Id).FirstOrDefaultAsync();
                if(notification == null)
                {
                    response.Success = true;
                    response.Message = "Notification not found";
                }
                else
                {
                    notification.Status = NotificationStatus.Read.ToString();
                    _context.Update(notification);
                    await _context.SaveChangesAsync();

                    response.Success = true;
                    response.Message = "Notification read!!";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error reading noification";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public async Task<ServiceResponse> RunBackgroundNotifications()
        {
            ServiceResponse response = new();
            try
            {
                DateTime currentTime = DateTime.Now;
                DateTime dueDateLimit = currentTime.AddHours(48);

                DateTime dueDate;
                //get tasks due in the 48 hours
                var tasks = await _context.Tasks
                  .Where(task => DateTime.TryParseExact(task.DueDate, "dd/MMM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate)
                  && dueDate >= currentTime && dueDate <= dueDateLimit)
                  .ToListAsync();

                if(tasks.Count == 0)
                {
                    response.Message = "No tasks found";
                    response.Success = true;
                } 
                else
                {
                    foreach (var task in tasks)
                     {
                            var message = $"Task Due in 48Hours - {task.Title}";
                            await _helpers.CreateNewNotification(task.UserId, "Task Due in 48 Hours", message);
                     }

                    response.Success = true;
                    response.Message = "Notifications in queue";
                }
                
            }
            catch (Exception ex)
            {
                response.Errors = new[] { ex.Message };
                response.Success = false;
                response.Message = "Error fetching tasks";
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateTaskPriority(UpdateTask request)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check if tasks exists 
                var task = await _context.Tasks.Where(x => x.Id == request.TaskId).FirstOrDefaultAsync();
                if (task == null)
                {
                    response.Success = true;
                    response.Message = "Task not found";
                }
                else
                {
                    if(task.Priority == request.StatusOrPriority)
                    {
                        response.Success = true;
                        response.Message = "Task already has present priority";
                    }
                    else
                    {
                        task.Priority = request.StatusOrPriority;
                        _context.Update(task);
                        await _context.SaveChangesAsync();

                        //send notification
                        var description = $"Task Updated - {task.Title} has been updated to {task.Priority}";
                        await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);

                        response.Success = true;
                        response.Message = "Task successfully updated";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success= false;
                response.Message = "Error updating Task";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateTaskStatus(UpdateTask request)
        {
            ServiceResponse response = new();
            try
            {
                var httpContext = _accessor.HttpContext;
                var getUser = httpContext?.User.Identity.Name;

                var findUser = _userManager.FindByNameAsync(getUser);
                var returnedUser = findUser.Result;

                //check if tasks exists 
                var task = await _context.Tasks.Where(x => x.Id == request.TaskId).FirstOrDefaultAsync();
                if (task == null)
                {
                    response.Success = true;
                    response.Message = "Task not found";
                }
                else
                {
                    if (task.Status == request.StatusOrPriority)
                    {
                        response.Success = true;
                        response.Message = "Task already has present status";
                    }
                    else
                    {
                        task.Status = request.StatusOrPriority;
                        _context.Update(task);
                        await _context.SaveChangesAsync();

                        //send notification
                        var description = $"Task Updated - {task.Title} has been updated to {task.Priority}";
                        await _helpers.CreateNewNotification(returnedUser.Id, "New Project Created", description);

                        response.Success = true;
                        response.Message = "Task successfully updated";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error updating Task";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }
    }
}
