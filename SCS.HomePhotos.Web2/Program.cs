using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using SCS.HomePhotos;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Data.Core;
using SCS.HomePhotos.Model;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Web;
using SCS.HomePhotos.Web.Hubs;
using SCS.HomePhotos.Web.Middleware;
using SCS.HomePhotos.Web.Security;
using SCS.HomePhotos.Workers;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
    });

var Container = builder.Services;
var Configuration = builder.Configuration;

Container.Configure<JwtAuthentication>(Configuration.GetSection("JwtAuthentication"));
Container.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Formatting = Formatting.Indented;
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
    });

Container.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-Token";
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

Container.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
Container.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

Container.AddAuthorization(options =>
{
    options.AddPolicy("Admins", policy => policy.RequireRole(RoleType.Admin.ToString()));
    options.AddPolicy("Contributors", policy => policy.RequireRole(RoleType.Contributor.ToString(), RoleType.Admin.ToString()));
    options.AddPolicy("Readers", policy => policy.RequireRole(RoleType.Reader.ToString(), RoleType.Contributor.ToString(), RoleType.Admin.ToString()));
});

Container.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
    builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

Container.AddSignalR();

Container.AddSwaggerGen(c =>
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

Container.AddSingleton<IStaticConfig>(staticConfig);
Container.AddSingleton<IConfigData>(configData);
Container.AddSingleton<IDynamicConfig>(dynamicConfig);
var configService = new ConfigService(configData, dynamicConfig, staticConfig);
await configService.SetDynamicConfig();
Container.AddSingleton<IConfigService>(configService);

// background tasks            
Container.AddHostedService<QueuedHostedService>();
Container.AddHostedService<TimedIndexHostedService>();
Container.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

// data objects
Container.AddScoped<IPhotoData, PhotoData>();
Container.AddScoped<ITagData, TagData>();
Container.AddScoped<IPhotoTagData, PhotoTagData>();
Container.AddScoped<IUserData, UserData>();
Container.AddScoped<IUserTokenData, UserTokenData>();
Container.AddScoped<ILogData, LogData>();
Container.AddScoped<IFileExclusionData, FileExclusionData>();
Container.AddScoped<IUserSettingsData, UserSettingsData>();

// services
Container.AddScoped<IFileSystemService, FileSystemService>();
Container.AddScoped<IImageTransformer, ImageTransformer>();
Container.AddScoped<IImageService, ImageService>();
Container.AddScoped<IPhotoService, PhotoService>();
Container.AddScoped<IFileUploadService, FileUploadService>();
Container.AddScoped<IAccountService, AccountService>();
Container.AddScoped<ISecurityService, SecurityService>();
Container.AddSingleton<IAdminLogService>(new AdminLogService(new LogData(staticConfig), staticConfig));
Container.AddSingleton<IIndexEvents, IndexEvents>();
Container.AddSingleton<IQueueEvents, QueueEvents>();
Container.AddSingleton<IClientMessageSender, ClientMessageSender>();
Container.AddSingleton<IUploadTracker, UploadTracker>();
Container.AddSingleton<IImageMetadataService, ImageMetadataService>();

builder.Host.UseWindowsService();

var App = builder.Build();

// https://github.com/serilog/serilog-extensions-logging-file
var Services = App.Services;
var loggerFactory = Services.GetService<ILoggerFactory>();
var env = App.Environment;

Services.GetService<IClientMessageSender>(); // need to force it to load

loggerFactory.AddFile(Configuration.GetSection("Logging"));

if (env.IsDevelopment())
{
    App.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    App.UseHsts();
}

App.UseSwagger();
App.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomePhotos API");
    c.RoutePrefix = "swagger";
});

App.UseGloablExceptionMiddleware();

// remove to use http, not https
//app.UseHttpsRedirection();         

App.UsePhotoImageMiddleware();
App.UseStaticFiles();

if (!env.IsDevelopment())
{
    App.UseSpaStaticFiles();
}

App.UseRouting();
App.UseCors("AllowAllOrigins");
App.UseAuthentication();
App.UseAuthorization();

App.UseAvatarImageMiddleware();

App.MapHub<NotifcationHub>("/message-hub");

App.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});

App.MapFallbackToFile("index.html");

App.Run();