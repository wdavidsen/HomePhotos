﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.OpenApi.Models;

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

using System;
using System.IO;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Web
{
    /// <summary>
    /// The startup initialization class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the root configuration.
        /// </summary>
        /// <value>
        /// The root configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtAuthentication>(Configuration.GetSection("JwtAuthentication"));
            services.AddControllers()
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

            // replaced with SpaRoot, SpaProxyServerUrl and SpaProxyLaunchCommand in project file.
            // In production, the Angular files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomePhotos API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SCS.HomePhotos.Web.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SCS.HomePhotos.Model.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SCS.HomePhotos.Service.xml"));
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field; for example: Bearer [token here]",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomePhotos API");
                c.RoutePrefix = "swagger";
            });

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

            // replaced with SpaRoot, SpaProxyServerUrl and SpaProxyLaunchCommand in project file.
            //app.UseSpa(spa =>
            //{
            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,
            //    // see https://go.microsoft.com/fwlink/?linkid=864501

            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});

            var webApp = app as WebApplication;
            webApp.MapFallbackToFile("index.html");
        }
        private async Task SetDynamicConfig(IConfigService configService)
        {
            await configService.SetDynamicConfig();
        }
    }
}