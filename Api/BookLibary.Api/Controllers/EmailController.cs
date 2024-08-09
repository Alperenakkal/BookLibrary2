using Microsoft.AspNetCore.Mvc;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices.EmailServices;
using System.Threading.Tasks;

namespace BookLibary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] Email emailModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendEmailAsync(emailModel);
            return Ok("Email sent successfully.");
        }
        [HttpPost("sendverify-email")]
        public async Task<IActionResult> SendVerifyEmail([FromBody] Email emailModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendVerifyEmailAsync(emailModel);
            return Ok("Email sent successfully.");
        }
    }
}