using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SCS.HomePhotos.Core;
using SCS.HomePhotos.Data;
using SCS.HomePhotos.Service;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Web.Middleware;
using SCS.HomePhotos.Web.Security;
using SCS.HomePhotos.Workers;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web
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
            services.Configure<JwtAuthentication>(Configuration.GetSection("JwtAuthentication"));
            services.AddControllersWithViews()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
                });

            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminsOnly", policy => policy.RequireClaim("Admin"));
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // config
            var staticConfig = StaticConfig.Build(Configuration);
            var configData = new ConfigData(staticConfig);
            var dynamicConfig = new DynamicConfig();

            services.AddSingleton<IStaticConfig>(staticConfig);
            services.AddSingleton<IConfigData>(configData);
            services.AddSingleton<IDynamicConfig>(dynamicConfig); // we'll populate this in Configure method
            services.AddSingleton<IConfigService>(new ConfigService(configData, dynamicConfig, staticConfig));

            // background tasks
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<TimedIndexHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // data objects
            services.AddScoped<IPhotoData, PhotoData>();
            services.AddScoped<ITagData, TagData>();
            services.AddScoped<IUserData, UserData>();
            services.AddScoped<IUserTokenData, UserTokenData>();

            // services
            services.AddScoped<IFileSystemService, FileSystemService>();
            services.AddScoped<IImageResizer, ImageResizer>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IAccountService, AccountService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfigService configService, ILoggerFactory loggerFactory)
        {
            // https://github.com/serilog/serilog-extensions-logging-file
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            SetDynamicConfig(configService).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseGloablExceptionMiddleware();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UsePhotoImageMiddleware();
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
        private async Task SetDynamicConfig(IConfigService configService)
        {
            await configService.SetDynamicConfig();
        }
    }
}
