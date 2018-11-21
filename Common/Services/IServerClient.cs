using System.Threading;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Services
{
    public interface IServerClient<T>
    {

        Task<T> GetAsync(CancellationToken token, string id, string path);

        Task<bool> PostAsync(CancellationToken token, string jsonBody = "", string path = "");

        Task<bool> PostAsync(CancellationToken token, T model, string path = "");

        Task<bool> PutAsync(CancellationToken token, T model, string path = "");

        Task<bool> PutAsync(CancellationToken token, string jsonBody = "", string path = "");

    }
}
 