using Email.Application.UseCases.EmailValidation.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Email.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("validate/{emailValidationCode}")]
        public async Task<IActionResult> ValidateEmail(string emailValidationCode)
        {
            (bool emailValidatedWithSuccess, string? error) = await _mediator.Send(new ValidateEmailCommand(emailValidationCode));

            return emailValidatedWithSuccess ? NoContent() : BadRequest(error);
        }
    }
}
