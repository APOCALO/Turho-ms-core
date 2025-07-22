using System.Reflection;
using Web.Api.Middlewares;

namespace Web.Api
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services) 
        {
            services.AddControllers();

            // Add Swagger
            services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            // Agregamos endpoint de salud /health
            services.AddHealthChecks();

            services.AddTransient<GlobalExceptionHandlingMiddleware>();

            return services;
        }
    }
}
