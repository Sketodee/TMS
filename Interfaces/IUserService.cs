using TMS.Request;
using TMS.Responses;

namespace TMS.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse> SignUp(SignUpRequest request);
        Task<ServiceResponse> Login(LoginRequest request);
        Task<ServiceResponse> ForgotPassword(OtpRequest request);
        Task<ServiceResponse> ResetPassword(PasswordResetRequest request);
    }
}
