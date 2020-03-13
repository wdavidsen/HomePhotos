using System.Threading.Tasks;
using SCS.HomePhotos.Model;

namespace SCS.HomePhotos.Data
{
    public interface IConfigData : IDataBase
    {
        Task<Config> GetConfiguration();
        Task<Config> SaveConfiguration(Config config);
    }
}