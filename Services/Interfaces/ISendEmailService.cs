using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ISendEmailService {
        Task<string> SendEmail(string token, string assign, string nameForm);
    }
}
