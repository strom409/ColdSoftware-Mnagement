using ColdStoreManagement.BLL.Data;
using ColdStoreManagement.Configurations;
using ColdStoreManagement.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.

// Configure logging: Add serilog services to the container and read config from appsettings
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.File(
            path: "Logs/coldstore-.log",  // Path and pattern for log files (rolling over with date or numbering)
            rollingInterval: RollingInterval.Day,  // Roll over daily
            retainedFileCountLimit: 10,  // Keep logs for 10 days
            fileSizeLimitBytes: 15 * 1024 * 1024, // Optional: limit log file size (e.g., 15MB per file)
            shared: true  // Allow multiple processes to write to the same log file
        );
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {        
        builder.AllowAnyOrigin() // Allows any origin
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        ////builder.WithOrigins("http://localhost:5173", "http://localhost:44399")
        //.AllowAnyHeader()
        //.AllowAnyMethod()
        //.AllowCredentials();
    });
});

// Configure JSON Options
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Use camel case property names
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Ignore null values (uncomment if needed)
        // opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        // Convert enums to string
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        // Handle reference loop issues
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger for API Documentation
builder.Services.AddSwaggerDocumentation();

// Register services. Add services to the container.
builder.Services.RegisterApplicationServices(Configuration);
// Register IHttpContextAccessor
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Config Authentication. Add Authentication and JWT Bearer Configuration
builder.Services.ConfigAuthentication(Configuration);

// Configure Authorization Policies
builder.Services.ConfigAuthorizationPolicies();

// RateLimiting middleware
builder.Services.ConfigRateLimiter();

// Add Health Checks
builder.Services.AddHealthChecks();


#region This method gets called by the runtime and used to configure the HTTP request pipeline.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error"); // Add an error handler for production
    app.UseHsts();
}
// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cold-Store Management System API v1");
    c.RoutePrefix = "swagger"; // or "" for root
});

//// VERY IMPORTANT if behind proxy / load balancer
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
//    RequireHeaderSymmetry = false,
//    ForwardLimit = null
//});

// Configure Serilog for logging
app.UseSerilogRequestLogging();

//app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use CORS policy
app.UseCors("CorsPolicy");

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

//config exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure RateLimiting middleware
app.UseRateLimiter();

// Map Controllers
app.MapControllers();

// HealthCheck Middleware, Add Health Checks Endpoint
app.MapHealthChecks("/api/health");

// Run the application
app.Run();

#endregion