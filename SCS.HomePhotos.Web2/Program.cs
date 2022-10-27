using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SCS.HomePhotos.Web.Security;
using SCS.HomePhotos.Model;
using Microsoft.OpenApi.Models;
using SCS.HomePhotos;
using SCS.HomePhotos.Data.Core;
using SCS.HomePhotos.Data.Contracts;
using SCS.HomePhotos.Service.Core;
using SCS.HomePhotos.Service.Contracts;
using SCS.HomePhotos.Workers;
using SCS.HomePhotos.Service.Workers;
using SCS.HomePhotos.Web;
using SCS.HomePhotos.Web.Hubs;
using Microsoft.Extensions.Logging;
using SCS.HomePhotos.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllersWithViews();
var services = builder.Services;
var Configuration = builder.Configuration;

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
await configService.SetDynamicConfig();
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

var app = builder.Build();

// https://github.com/serilog/serilog-extensions-logging-file
var services2 = app.Services;
var loggerFactory = services2.GetService<ILoggerFactory>();
var env = app.Environment;

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

//app.MapFallbackToFile("index.html");

app.Run();