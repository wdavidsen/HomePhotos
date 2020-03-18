using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SCS.HomePhotos.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)                
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://0.0.0.0:44375", "http://0.0.0.0:8080");
                });
    }
}
