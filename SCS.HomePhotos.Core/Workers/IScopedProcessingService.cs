using System.Threading;
using System.Threading.Tasks;

namespace SCS.HomePhotos.Service.Workers
{
    public interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
