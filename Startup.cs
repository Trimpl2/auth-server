using IdentityServer4;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AuthServer = auth_server.Data.AuthServer;

namespace auth_server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "auth_server", Version = "v1"});
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDbContext<AuthServer.IdentityDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddIdentity<AuthServer.IdentityServerUser, IdentityRole>(options =>
                {
                    options.Password = new PasswordOptions
                    {
                        RequireLowercase = false,
                        RequireDigit = false,
                        RequireUppercase = false,
                        RequireNonAlphanumeric = false
                    };
                    options.SignIn = new SignInOptions
                    {
                        RequireConfirmedEmail = false,
                        RequireConfirmedPhoneNumber = false
                    };
                })
                .AddEntityFrameworkStores<AuthServer.IdentityDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.UserInteraction = new UserInteractionOptions
                    {
                        LogoutUrl = "/connect/logout",
                        LoginUrl = $"{Configuration.GetValue<string>("AuthUIUrl")}/login",
                        LoginReturnUrlParameter = "callback"
                    };
                })
                .AddAspNetIdentity<AuthServer.IdentityServerUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = optionsBuilder =>
                    {
                        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    };
                })
                .AddInMemoryApiScopes(AuthServer.Config.ApiScopes)
                .AddInMemoryClients(AuthServer.Config.Clients)
                .AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "auth_server v1"));
            }

            app.UseIdentityServer();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}