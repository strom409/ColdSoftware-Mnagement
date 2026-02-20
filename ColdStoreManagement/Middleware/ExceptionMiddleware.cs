using ColdStoreManagement.BLL.Errors;
using System.Net;
using System.Text.Json;

namespace ColdStoreManagement.Middleware
{
    /// <summary>
    /// Custom Exception Middleware
    /// </summary>
    /// <param name="next"></param>
    /// <param name="hostEnvironment"></param>
    public class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment hostEnvironment)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// Middleware invoke function
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong {FunctionName}: {ex.Message}");
                // _logger.LogError(ex, ex.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = APIException.FromException(ex, _hostEnvironment.IsDevelopment());

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(response, options);
                await httpContext.Response.WriteAsync(json);
            }
        }
        /// <summary>
        /// this function used to get running method name
        /// </summary>
        private static string FunctionName
        {
            get
            {
                try
                {
                    var st = new System.Diagnostics.StackTrace();
                    var sf = st.GetFrame(1);

                    // Check for null BEFORE accessing sf.GetMethod()
                    if (sf != null)
                    {
                        var currentMethodName = sf.GetMethod();
                        if (currentMethodName != null) // Also check if currentMethodName is null (unlikely but possible)
                        {
                            if (currentMethodName.Name == "MoveNext")
                                return currentMethodName.ReflectedType?.FullName ?? string.Empty; // Null-conditional operator and null-coalescing
                            else
                                return currentMethodName.Name;
                        }
                    }

                    return string.Empty; // Return empty if sf or currentMethodName is null

                }
                catch (Exception)
                {
                    return string.Empty; // Return empty on error
                }
            }
        }
    }
}
