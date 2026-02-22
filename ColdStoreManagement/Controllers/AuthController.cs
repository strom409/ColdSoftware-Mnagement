using ColdStoreManagement.BLL.Models.Auth;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.Controllers
{
    /// <summary>
    /// This endpoint provides User Auth related endpoints.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="authService"></param>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseController
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        ///  this endpoint used for user authuntication 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            try
            {
                // Validation is handled automatically by [ApiController] attribute 
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(loginModel);

                bool isValid = Validator.TryValidateObject(loginModel, context, validationResults, true);
                if (!isValid)
                {
                    return Unauthorized(new { Message = "Validation failed! Invalid user data returned from system." });
                }

                // Call Business Logic Layer
                var result = await _authService.CheckUser(loginModel);
                if (result == null)
                {
                    return Unauthorized(new { Message = "Invalid user data returned from system." });
                }

                // 3. Check failure flag from Database/Logic
                if (result.RetFlag.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { Message = result.RetMessage ?? "Login failed." });
                }

                // 4. Success – Generate a JWT Token
                // Instead of ProtectedLocalStore, we send a token back to the client
                var token = _authService.GenerateJwtToken(result);

                return Ok(new
                {
                    Token = token,
                    Username = loginModel.GlobalUserName,
                    UnitId = result.GlobalUnitId,
                    UserGroup = result.GlobalUserGroup,
                    UnitName = loginModel.GlobalUnitName,
                    ExpiresIn = 3600 // 1 hour
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// this endpoint used to change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        
        [HttpPost("change-password")]
        [Authorize(policy: "RequireAuthenticatedUser")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.UpdateUserPasswordAsync(model.UserName, model.OldPassword, model.NewPassword);
                if (result != null && result.RetFlag?.Trim() == "FALSE")
                {
                    return BadRequest("Failed to update Password");
                }

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }


        ///// <summary>
        ///// This endpoint used to request to Send Password Reset Link for reset password
        ///// and this send email to user account to reset
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("forget-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> RequestPasswordReset([FromBody] ResetRequestDto model)
        //{
        //    try
        //    {
        //        var result = await authService.GenerateResetTokenAsync(model.Email);
        //        if (result.Success) return Ok(result);
        //        else return BadRequest(result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// This Endpoint used to submit reset password with reset token
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("reset-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        //{
        //    try
        //    {
        //        var result = await authService.ResetPasswordWithTokenAsync(model);
        //        if (result.Success) return Ok(result);
        //        else return BadRequest(result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// This endpoint used to verify user account by OTP
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("verify-email-confirmation")]
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
        //{
        //    try
        //    {
        //        var result = await authService.VerifyOtp(model);
        //        if (result.Success) return Ok(result);
        //        else return BadRequest(result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        ///// <summary>
        ///// this endpoint used to send verification email
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[HttpPost("resend-verification-email")]
        //public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailDto model)
        //{
        //    try
        //    {
        //        var result = await authService.ResendVerificationEmail(model);
        //        if (result.Success) return Ok(result);
        //        else return BadRequest(result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //        /// <summary>
        //        /// Private function to get IP address
        //        /// </summary>
        //        /// <param name="httpContext"></param>
        //        /// <returns></returns>
        //        private static string GetIpAddress(HttpContext httpContext)
        //        {
        //            var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        //            if (string.IsNullOrEmpty(ip))
        //            {
        //                ip = httpContext.Connection.RemoteIpAddress?.ToString();
        //            }
        //#pragma warning disable CS8603 // Possible null reference return.
        //            return ip;
        //#pragma warning restore CS8603 // Possible null reference return.
        //        }

    }
}
