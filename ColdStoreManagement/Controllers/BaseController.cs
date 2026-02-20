using ColdStoreManagement.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ColdStoreManagement.Controllers
{
    /// <summary>
    /// Its base Controller shared for all
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// method to get values from JWT token.
        /// </summary>
        /// <returns></returns>
        protected JWTUserDto DecodeJWT()
        {
            var userModel = new JWTUserDto();
            try
            {
                if (Request != null)
                {
                    var stream = Request.Headers.FirstOrDefault(x => x.Key == "Authorization").Value;
                    if (!string.IsNullOrEmpty(stream))
                    {
                        var token = stream.ToString().Split(" ").LastOrDefault();
                        var handler = new JwtSecurityTokenHandler();
                        JwtSecurityToken parsedToken = handler.ReadJwtToken(token);


                        foreach (Claim claim in parsedToken.Claims)
                        {
                            switch (claim.Type)
                            {
                                case ClaimTypes.NameIdentifier:
                                    userModel.UserId = int.Parse(claim.Value);
                                    break;
                                case ClaimTypes.Email:
                                    userModel.Email = claim.Value;
                                    break;
                                case ClaimTypes.Role:
                                    userModel.Role = claim.Value;
                                    break;
                                case ClaimTypes.Sid:
                                    userModel.UnitName = claim.Value;
                                    break;
                                case JwtRegisteredClaimNames.Jti:
                                    userModel.Jti = claim.Value;
                                    break;
                                    // Add more cases for other claims if needed
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return userModel;
        }
        protected int CurrentUserId => GetClaimValue<int>(ClaimTypes.NameIdentifier);

        protected string UserRole => GetClaimValue<string>(ClaimTypes.Role);

        protected string UserEmail => GetClaimValue<string>(ClaimTypes.Email);


        private T GetClaimValue<T>(string claimType)
        {
            // Check if the User or Identity exists
            var claim = User?.FindFirst(claimType);

            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                throw new UnauthorizedAccessException("Required claim is missing or the token is invalid.");
            }
            // Convert the claim value to the specified type T
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(claim.Value)!;
        }

        ///// <summary>
        ///// Get user id of current user
        ///// </summary>
        ///// <returns></returns>
        //protected int GetClaimUserId()
        //{
        //    try
        //    {
        //        if (Request != null)
        //        {
        //            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new Exception("Authorization header is missing or token is invalid.");
        //            return int.Parse(userIdClaim.Value); // Assuming userId is an integer 
        //        }
        //        throw new Exception("Authorization header is missing or token is invalid.");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        ///// <summary>
        ///// used to get user role
        ///// </summary>
        ///// <returns></returns>
        //protected string GetUserRole()
        //{
        //    try
        //    {
        //        if (Request != null)
        //        {
        //            var userRoleClaim = User.FindFirst(ClaimTypes.Role) ?? throw new Exception("Authorization header is missing or token is invalid.");
        //            return userRoleClaim.Value; // Assuming userId is an integer 
        //        }
        //        throw new Exception("Authorization header is missing or token is invalid.");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        ///// <summary>
        ///// this function used to get running method name
        ///// </summary>
        //protected string FunctionName
        //{
        //    get
        //    {
        //        try
        //        {
        //            var st = new System.Diagnostics.StackTrace();
        //            var sf = st.GetFrame(1);

        //            // Check for null BEFORE accessing sf.GetMethod()
        //            if (sf != null)
        //            {
        //                var currentMethodName = sf.GetMethod();
        //                if (currentMethodName != null) // Also check if currentMethodName is null (unlikely but possible)
        //                {
        //                    if (currentMethodName.Name == "MoveNext")
        //                        return currentMethodName.ReflectedType?.FullName ?? string.Empty; // Null-conditional operator and null-coalescing
        //                    else
        //                        return currentMethodName.Name;
        //                }
        //            }

        //            return string.Empty; // Return empty if sf or currentMethodName is null

        //        }
        //        catch (Exception)
        //        {
        //            return string.Empty; // Return empty on error
        //        }
        //    }
        //}

    }
}
