using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TMS.Data;
using TMS.HelperFunctions;
using TMS.Interfaces;
using TMS.Models;
using TMS.Request;
using TMS.Responses;
using static System.Net.WebRequestMethods;

namespace TMS.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly Helpers _helpers;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly SignInManager<AppUser> _signInManager;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, 
            ILogger<UserService> logger, IConfiguration configuration, DataContext context, Helpers helpers,
            IPasswordValidator<AppUser> passwordValidator,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _helpers = helpers;
            _passwordValidator = passwordValidator;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponse> ForgotPassword(OtpRequest request)
        {
            ServiceResponse response = new();
            try
            {
                //find the app user using identity
                AppUser appUser = await _userManager.FindByEmailAsync(request.Email);

                if (appUser == null)
                {
                    response.Message = "User not found";
                    response.Success = false;
                }
                else
                {
                    //generate an otp for the app user
                    var tokenProvider = await _context.TokenProviders.Where(x => x.UserId == appUser.Id).FirstOrDefaultAsync();
                    if (tokenProvider != null)
                    {
                        tokenProvider.Created = DateTime.Now;
                        tokenProvider.Expired = DateTime.Now.AddMinutes(1);
                        tokenProvider.Token = _helpers.GetRandomID();
                        _context.TokenProviders.Update(tokenProvider);
                        await _context.SaveChangesAsync();


                        //send token to user 
                        var message = $"Here is your one time OTP : {tokenProvider.Token}";
                        //await _helpers.SendEmailAsync("TestEmailTemplate", appUser.Name, appUser.Email, message, "Password Reset OTP");


                        Email email = new Email
                        {
                            template = "Test",
                            userName = appUser.Name,
                            userEmail = appUser.Email,
                            message = message,
                            subject = "Password Reset OTP",
                            type = EmailType.passwordResetEmail.ToString(),
                        };

                        _context.Emails.Add(email);
                        await _context.SaveChangesAsync();

                        _logger.LogWarning($"{appUser.Email} request forgotpassword OTP");
                        response.Message = "Otp sent to Email";
                        response.Success = true;
                    }
                    else
                    {
                        var otp = _helpers.GetRandomID();
                        TokenProvider newToken = new TokenProvider
                        {
                            UserId = appUser.Id,
                            Token = otp
                        };

                        _context.TokenProviders.Add(newToken);
                        await _context.SaveChangesAsync();

                        //send token to user 
                        var message = $"Here is your one time OTP : {otp}";
                        //await _helpers.SendEmailAsync("TestEmailTemplate", appUser.Name, appUser.Email, message, "Password Reset OTP");

                        Email email = new Email
                        {
                            template = "Test",
                            userName = appUser.Name,
                            userEmail = appUser.Email,
                            message = message,
                            subject = "Password Reset OTP",
                            type = EmailType.passwordResetEmail.ToString(),
                        };

                        _context.Emails.Add(email);
                        await _context.SaveChangesAsync();

                        _logger.LogWarning($"{appUser.Email} request forgotpassword OTP");
                        response.Message = "Otp sent to Email";
                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse> Login(LoginRequest request)
        {
            ServiceResponse response = new();
            try
            {
                AppUser appUser = await _userManager.FindByEmailAsync(request.Email);
                var lockedOutStatus = await _userManager.IsLockedOutAsync(appUser);

                if (appUser == null)
                {
                    response.Message = "Invalid credentials ";
                    response.Success = false;
                }
                else if (lockedOutStatus)
                {
                    response.Message = "Your account have been locked out due to multiple sign in attempts.";
                    response.Success = false;
                }
                else if (appUser != null && await _userManager.CheckPasswordAsync(appUser, request.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(appUser);

                    //reset the failed count on successful login 
                    await _userManager.ResetAccessFailedCountAsync(appUser);

                    var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, appUser.UserName),
                                new Claim(ClaimTypes.Email, appUser.Email),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    var returnedToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var userDetails = new LoginResponse
                    {
                        roles = userRoles,
                        email = appUser.Email,
                        Name = appUser.Name,
                        id = appUser.Id,
                        token = returnedToken,
                    };

                    _logger.LogWarning($"{appUser.Email} logged in successfully");
                    response.Data = userDetails;
                    response.Success = true;

                }
                else
                {
                    //increase failed counts
                    await _userManager.AccessFailedAsync(appUser);
                    response.Message = "invalid credentials";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Invalid Credentials";
                response.Success = false;
            }

            return response;
        }

        public async Task<ServiceResponse> ResetPassword(PasswordResetRequest request)
        {
            ServiceResponse response = new();
            try
            {
                AppUser appUser = await _userManager.FindByEmailAsync(request.Email);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    //use "email", "forgot password" and "token" to validate the token generated from Forgetpassword service 

                    var tokenProvider = await _context.TokenProviders.Where(x => x.UserId == appUser.Id).FirstOrDefaultAsync();
                    TimeSpan span = DateTime.Now.Subtract(tokenProvider.Created);

                    var validPass = await _passwordValidator.ValidateAsync(_userManager, appUser, request.Password);
                    if (tokenProvider.Token == request.Token && validPass.Succeeded && span.Minutes <= 4 && span.Seconds <= 60) //change minutes to 0 after FE test
                    {
                        //generate a token to change user password, 
                        var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                        var passwordResetResult = await _userManager.ResetPasswordAsync(appUser, token, request.Password);

                        if (passwordResetResult.Succeeded)
                        {
                            //send mail to user to confirm that password has been reset 
                            var message = "Your password reset is successful. You can proceed to login with your new password";
                            //await _helpers.SendEmailAsync("Test", appUser.Name, appUser.Email, message, "Password Reset Confirmation");


                            Email email = new Email
                            {
                                template = "Test",
                                userName = appUser.Name,
                                userEmail = appUser.Email,
                                message = message,
                                subject = "Password Reset Confirmation",
                                type = EmailType.passwordResetEmail.ToString(),
                            };

                            _context.Emails.Add(email);
                            await _context.SaveChangesAsync();

                            //reset loginfailed count
                            await _userManager.ResetAccessFailedCountAsync(appUser);
                            await _userManager.SetLockoutEndDateAsync(appUser, DateTimeOffset.MinValue);


                            //change token state in db because its been used 
                            var newToken = await _context.TokenProviders.Where(x => x.Token == tokenProvider.Token).FirstOrDefaultAsync();
                            newToken.Created = DateTime.Now;
                            newToken.Expired = DateTime.Now.AddSeconds(60);
                            newToken.Token = _helpers.GetRandomID();
                            _context.TokenProviders.Update(newToken);
                            await _context.SaveChangesAsync();

                            _logger.LogWarning($"{appUser.Email} successfully changed their password");
                            response.Message = "Password successfully changed";
                            response.Success = true;
                        }
                    }
                    else if (!validPass.Succeeded)
                    {
                        List<string> errors = new List<string>();

                        foreach (var error in validPass.Errors)
                        {
                            errors.Add(error.Description);
                        }

                        response.Message = "Can't Reset Password";
                        response.Success = false;
                        response.Errors = errors;
                    }
                    else if (tokenProvider.Token != request.Token)
                    {
                        response.Message = "Invalid token";
                        response.Success = false;
                    }
                    else if (span.Seconds > 60 || span.Minutes > 4) //change minutes to 0 after FE test
                    {
                        response.Message = "Token expired";
                        response.Success = false;
                    }
                    else
                    {
                        response.Message = "Password Reset failed";
                        response.Success = false;
                    }

                }
                else if (appUser == null)
                {
                    response.Message = "Credentials not found";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        public async Task<ServiceResponse> SignUp(SignUpRequest request)
        {
            ServiceResponse response = new();

            try
            {
                var newUser = new AppUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    Name = request.Name,
                };

                //create new user(system admin)
                IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);

                if (result.Succeeded)
                {
                    //check if User role already exists 
                    if(await _roleManager.FindByNameAsync(AuthorizeRoles.User.ToString()) == null) {
                        //Create new user role
                        await _roleManager.CreateAsync(new IdentityRole(AuthorizeRoles.User.ToString()));
                    }
                    //add new user to User role 
                    await _userManager.AddToRoleAsync(newUser, AuthorizeRoles.User.ToString());

                    //email service
                    var message = "Welcome to TMS";
                    Email email = new Email
                    {
                        template = "Test",
                        userName = newUser.Name,
                        userEmail = newUser.Email,
                        message = message,
                        subject = "TMS",
                        type = EmailType.newUserEmail.ToString(),
                    };

                    _context.Emails.Add(email);
                    await _context.SaveChangesAsync();

                    _logger.LogWarning("New User created succesfully");
                    response.Success = true;
                    response.Message = "User created successfully";
                }
                else if (!result.Succeeded)
                {
                    response.Message = "Error creating user ";
                    response.Success = false;
                    response.Errors = result.Errors.Select(x => x.Description);
                }

                else
                {
                    response.Success = false;
                    response.Message = "Can't create user";
                }

            }
            catch (Exception ex)
            {
                response.Message = "Can't create user";
                response.Errors = new[] { ex.Message };
                response.Success = false;
            }

            return response;
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(12),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
