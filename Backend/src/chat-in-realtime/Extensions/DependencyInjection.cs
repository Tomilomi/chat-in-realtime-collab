using chat_in_realtime.Handlers;

namespace chat_in_realtime.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
             {
                 options.AddDefaultPolicy(policy =>
                 {
                     policy.SetIsOriginAllowed(_ => true)
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
                 });
             });

            //controllers y swagger
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // SignalR
            services.AddSignalR();

            // Manejo global de excepciones
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}