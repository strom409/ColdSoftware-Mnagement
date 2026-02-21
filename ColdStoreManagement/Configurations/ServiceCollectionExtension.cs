using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Implementation;
using ColdStoreManagement.DAL.Services.Interface;

namespace ColdStoreManagement.Configurations
{
    /// <summary>
    /// Db and Service collection
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// config services and Repositories 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("SqlDbContext");
            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                throw new InvalidOperationException("The connection string 'SqlDbContext' was not found in the configuration.");
            }
            services.AddScoped(sp => new SQLHelperCore(sqlConnectionString));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBankingService, BankingService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IChamberService, ChamberService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICrateService, CrateService>();
            services.AddScoped<IGrowerService, GrowerService>();
            services.AddScoped<ITransactionsInService, TransactionsInService>();
            services.AddScoped<IVehicleInfoService, VehicleInfoService>();

            return services;
        }
    }
}
