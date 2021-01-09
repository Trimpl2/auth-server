using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace auth_server.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAccountApiAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("auth-server")
                .AddIdentityServerAuthentication("auth-server", options =>
                {
                    options.Authority = "https://localhost:5001";
                });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("auth-server-policy", builder =>
                {
                    builder.AddAuthenticationSchemes("auth-server");
                    builder.RequireScope("account");
                });
            });

            return services;
        }
    }
}