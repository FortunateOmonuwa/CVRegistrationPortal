global using Utilities.Responses;
global using Utilities.Services.MailService.DTO;

namespace Utilities.Services.MailService
{
    public interface IMailService
    {
        Task<ResponseDetail<string>> SendMail(MailRequest mail);
    }
}
