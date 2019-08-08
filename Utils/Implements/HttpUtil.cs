using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Utils.Interfaces;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Utils.Implements {
    public sealed class HttpUtil : IHttpUtil {
        private readonly HttpClient _httpClient;

        public HttpUtil(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        private void AddToken(string token) {
            _httpClient.DefaultRequestHeaders.Remove(Keywords.TOKEN_KEY);
            _httpClient.DefaultRequestHeaders.Add(Keywords.TOKEN_KEY, token);
        }

        public async Task<HttpResponseMessage> GetAsync(string apiURI) {
            try {
                return await _httpClient.GetAsync(apiURI);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string token, string apiURI) {
            try {
                AddToken(token);
                return await _httpClient.GetAsync(apiURI);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string apiURI, string body) {
            try {
                StringContent content = new StringContent(body, Encoding.UTF8, Keywords.MEDIA_TYPE_JSON);
                return await _httpClient.PostAsync(apiURI, content);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string token, string apiURI, string body) {
            try {
                AddToken(token);
                StringContent content = new StringContent(body, Encoding.UTF8, Keywords.MEDIA_TYPE_JSON);
                return await _httpClient.PostAsync(apiURI, content);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string token, string apiURI, string body) {
            try {
                AddToken(token);
                StringContent content = new StringContent(body, Encoding.UTF8, Keywords.MEDIA_TYPE_JSON);
                return await _httpClient.PutAsync(apiURI, content);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string token, string apiURI) {
            try {
                AddToken(token);
                return await _httpClient.DeleteAsync(apiURI);
            } catch (HttpRequestException) {
                return null;
            }
        }
    }
}