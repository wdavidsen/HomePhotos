namespace SCS.HomePhotos
{
    public interface IStaticConfig
    {
        string DatabasePath { get; set; }

        int TokenExpirationMinutes { get; set; }

        int RefreshTokenExpirationDays { get; set; }

        int MaxFailedLogins { get; set; }

        int ImageIndexParallelism { get; set; }

        int MaxImageFileSizeBytes { get; set; }

        int MaxAllowedIndexDirectoryFailures { get; set; }

        int ImageResizeQuality { get; set; }

        int LogRetentionDays { get; set; }

        PasswordRequirements PasswordRequirements { get; set; }
    }
}