using CentralizedDataSystem.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CentralizedDataSystem.Utils {
    public sealed class HttpUtils {
        private static readonly HttpUtils instance = new HttpUtils();
        private HttpClient httpClient;

        static HttpUtils() { }

        private HttpUtils() {
            httpClient = new HttpClient();
        }

        public static HttpUtils Instance {
            get {
                return instance;
            }
        }

        public void SetToken(string token) {
            httpClient.DefaultRequestHeaders.Add(Keywords.TOKEN_KEY, token);
        }

        public void RemoveToken() {
            httpClient.DefaultRequestHeaders.Remove(Keywords.TOKEN_KEY);
        }

        public async Task<HttpResponseMessage> GetAsync(string apiURI) {
            try {
                return await httpClient.GetAsync(apiURI);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string apiURI, string body) {
            try {
                var content = new StringContent(body, Encoding.UTF8, Keywords.MEDIA_TYPE_JSON);
                return await httpClient.PostAsync(apiURI, content);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string apiURI, string body) {
            try {
                var content = new StringContent(body, Encoding.UTF8, Keywords.MEDIA_TYPE_JSON);
                return await httpClient.PutAsync(apiURI, content);
            } catch (HttpRequestException) {
                return null;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string apiURI) {
            try {
                return await httpClient.DeleteAsync(apiURI);
            } catch (HttpRequestException) {
                return null;
            }
        }
    }
}