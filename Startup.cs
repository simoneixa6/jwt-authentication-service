using jwt_authentication_service.Helpers;
using jwt_authentication_service.Models;
using jwt_authentication_service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace jwt_authentication_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<UserDatabaseSettings>(
               Configuration.GetSection(nameof(UserDatabaseSettings)));

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddSingleton<IUserDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<UserDatabaseSettings>>().Value);

            services.AddSingleton<UserService>();

            services.AddMvc().AddNewtonsoftJson();

            services.AddControllers();

            services.AddScoped<IUserService, UserService>();

            services.AddLogging(builder =>
               builder
                   .AddDebug()
                   .AddConsole()
                   .AddConfiguration(Configuration.GetSection("Logging"))
                   .SetMinimumLevel(LogLevel.Information)
           );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Ajout du middleware qui vérifie les tokens jwt des requêtes
            app.UseMiddleware<JWTMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
