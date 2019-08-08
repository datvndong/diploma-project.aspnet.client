using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Utils.Interfaces {
    public interface IHttpUtil {
        Task<HttpResponseMessage> GetAsync(string apiURI);
        Task<HttpResponseMessage> GetAsync(string token, string apiURI);
        Task<HttpResponseMessage> PostAsync(string apiURI, string body);
        Task<HttpResponseMessage> PostAsync(string token, string apiURI, string body);
        Task<HttpResponseMessage> PutAsync(string token, string apiURI, string body);
        Task<HttpResponseMessage> DeleteAsync(string token, string apiURI);
    }
}
