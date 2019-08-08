namespace CentralizedDataSystem.Services.Interfaces {
    public interface ISendEmailService {
        void SendEmail(string email, string nameForm, string content);
    }
}
