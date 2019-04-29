using System.Threading.Tasks;

namespace HostedService
{
    public interface IStrategy
    {
        Task ExecuteAsync();
    }
}