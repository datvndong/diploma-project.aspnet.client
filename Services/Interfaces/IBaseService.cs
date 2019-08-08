using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IBaseService {
        Task<bool> IsValidToken(string token);
    }
}
