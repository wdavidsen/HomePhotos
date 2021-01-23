using Microsoft.Extensions.Configuration;

namespace SCS.HomePhotos
{
    public class StaticConfig : IStaticConfig
    {
        public string DatabasePath { get; set; }

        public int TokenExpirationMinutes { get; set; }

        public int RefreshTokenExpirationDays { get; set; }

        public int MaxFailedLogins { get; set; }

        public int ImageIndexParallelism { get; set; }

        public int MaxImageFileSizeBytes { get; set; }

        public int MaxAllowedIndexDirectoryFailures { get; set; }

        public int ImageResizeQuality { get; set; }

        public int LogRetentionDays { get; set; }

        public PasswordRequirements PasswordRequirements { get; set; }

        public string ImagePasscode { get; set; }

        public int PhotoExpirationDays { get; set; }

        public static StaticConfig Build(IConfiguration config)
        {
            var instance = new StaticConfig();
            config.GetSection("HomePhotos").Bind(instance);

            return instance;
        }
    }
}
