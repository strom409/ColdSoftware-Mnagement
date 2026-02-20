using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace ColdStoreManagement.Configurations
{
    /// <summary>
    /// Authentication Services Extensions
    /// </summary>
    public static class AuthenticationServiceExtension
    {
        /// <summary>
        /// Authentication Configs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                // options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWTConfigs:ValidIssuer"],
                    ValidAudience = configuration["JWTConfigs:ValidAudience"],
#pragma warning disable CS8604 // Possible null reference argument.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTConfigs:Secret"])),
#pragma warning restore CS8604 // Possible null reference argument.

                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new { status = false, message = "User is not authenticated." });
                        return context.Response.WriteAsync(result);
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new { status = false, message = "User is not authenticated." });
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        //context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        // context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new { status = false, message = "User is not authorized to access this resource." });
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            return services;
        }
        /// <summary>
        /// Authorization Policies
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                // Policy that allows all roles
                .AddPolicy("SuperAdminPolicy", policy =>
                    policy.RequireRole("SuperAdmin"))
                // Policy that allows admin role only
                .AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"))

                // Policy that allows only authenticate users
                .AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());

            return services;
        }


    }
}
