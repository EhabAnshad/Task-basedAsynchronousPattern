using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IServerClient<TOrder>
    {

        Task<TOrder> GetAsync(string id, string path);

        Task<TOrder> PostAsync(TOrder model, string path = "");

    }
}
 