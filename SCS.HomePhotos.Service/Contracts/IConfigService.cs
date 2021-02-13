using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Contracts
{
    public interface IConfigService
    {
        IDynamicConfig DynamicConfig { get; }
        IStaticConfig StaticConfig { get; }

        Task SetDynamicConfig();
        void SaveDynamicConfig();
    }
}