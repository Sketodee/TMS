using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.Interfaces;
using TMS.Request;
using TMS.Responses;

namespace TMS.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Sign up new user
        /// </summary>
        [HttpPost("signup")]
        public async Task<ActionResult<ServiceResponse>> SignUp (SignUpRequest request)
        {
            var response = await _userService.SignUp(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse>> Login(LoginRequest request)
        {
            var response = await _userService.Login(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        [HttpPost("forgotpassword")]
        public async Task<ActionResult<ServiceResponse>> ForgotPassword (OtpRequest request)
        {
            var response = await _userService.ForgotPassword(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        [HttpPost("resetpassword")]
        public async Task<ActionResult<ServiceResponse>> ResetPassword(PasswordResetRequest request)
        {
            var response = await _userService.ResetPassword(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
