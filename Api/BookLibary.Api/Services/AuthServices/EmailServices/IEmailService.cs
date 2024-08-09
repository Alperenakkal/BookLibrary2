using BookLibary.Api.Models;
using Org.BouncyCastle.Asn1.Pkcs;

namespace BookLibary.Api.Services.AuthServices.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email Email);
        Task SendVerifyEmailAsync(Email Email);

    }
}
