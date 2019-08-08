using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Implements {
    public class BaseService : IBaseService {
        private readonly IHttpUtil _httpUtil;

        public BaseService(IHttpUtil httpUtil) {
            _httpUtil = httpUtil;
        }

        public async Task<bool> IsValidToken(string token) {
            HttpResponseMessage response = await _httpUtil.GetAsync(token, APIs.CURRENT_USER);
            if (response == null) return false;

            return response.IsSuccessStatusCode;
        }
    }
}