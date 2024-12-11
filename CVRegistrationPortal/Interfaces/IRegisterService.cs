global using CVRegistrationPortal.DTOs;
using Utilities.Responses;

namespace CVRegistrationPortal.Interfaces
{
    public interface IRegisterService
    {
        Task<ResponseDetail<User>> Register(UserCreateDTO model);
        Task<ResponseDetail<string>> VerifyAccount(string token);
        void SendVerificationMail(string email, string body, string extra);
        Task<ResponseDetail<string>> ResendSendVerificationToken(string email);
    }
}
