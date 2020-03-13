using System.Threading.Tasks;

namespace SCS.HomePhotos.Service
{
    public interface IConfigService
    {
        IDynamicConfig DynamicConfig { get; }
        IStaticConfig StaticConfig { get; }

        Task SetDynamicConfig();
    }
}