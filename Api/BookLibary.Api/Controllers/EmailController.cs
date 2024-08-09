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
        private readonly EmailService _emailService;

        public ContactController(EmailService emailService)
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
    }
}