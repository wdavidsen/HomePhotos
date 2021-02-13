using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Data.Core;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Web.Hubs;
using SCS.HomePhotos.Web.Middleware;
using SCS.HomePhotos.Web.Security;
using SCS.HomePhotos.Workers;
using System.Threading.Tasks;
// using SCS.HomePhotos.Web.Filters;

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

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-Token";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admins", policy => policy.RequireRole(RoleType.Admin.ToString()));
                options.AddPolicy("Contributers", policy => policy.RequireRole(RoleType.Contributer.ToString(), RoleType.Admin.ToString()));
                options.AddPolicy("Readers", policy => policy.RequireRole(RoleType.Reader.ToString(), RoleType.Contributer.ToString(), RoleType.Admin.ToString()));
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

            services.AddSignalR();

            // config
            var staticConfig = StaticConfig.Build(Configuration);
            var configData = new ConfigData(staticConfig);
            var dynamicConfig = new DynamicConfig();

            services.AddSingleton<IStaticConfig>(staticConfig);
            services.AddSingleton<IConfigData>(configData);
            services.AddSingleton<IDynamicConfig>(dynamicConfig);
            var configService = new ConfigService(configData, dynamicConfig, staticConfig);
            SetDynamicConfig(configService).Wait();
            services.AddSingleton<IConfigService>(configService);

            // background tasks            
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<TimedIndexHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // data objects
            services.AddScoped<IPhotoData, PhotoData>();
            services.AddScoped<ITagData, TagData>();
            services.AddScoped<IUserData, UserData>();
            services.AddScoped<IUserTokenData, UserTokenData>();
            services.AddScoped<ILogData, LogData>();

            // services
            services.AddScoped<IFileSystemService, FileSystemService>();
            services.AddScoped<IImageTransformer, ImageTransformer>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddSingleton<IAdminLogService>(new AdminLogService(new LogData(staticConfig), staticConfig));
            services.AddSingleton<IIndexEvents, IndexEvents>();
            services.AddSingleton<IQueueEvents, QueueEvents>();
            services.AddSingleton<IClientMessageSender, ClientMessageSender>();
            services.AddSingleton<IUploadTracker, UploadTracker>();
            services.AddSingleton<IImageMetadataService, ImageMetadataService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.ApplicationServices.GetService<IClientMessageSender>();

            // https://github.com/serilog/serilog-extensions-logging-file
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

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

            // remove to use http, not https
            //app.UseHttpsRedirection();         

            app.UsePhotoImageMiddleware();
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAvatarImageMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotifcationHub>("/message-hub");
            });
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
