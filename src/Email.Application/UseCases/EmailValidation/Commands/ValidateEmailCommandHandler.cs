using Email.Application.Services.Interfaces;
using MediatR;

namespace Email.Application.UseCases.EmailValidation.Commands
{
    public sealed class ValidateEmailCommandHandler : IRequestHandler<ValidateEmailCommand, (bool, string?)>
    {
        private readonly IEmailService _emailService;

        public ValidateEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<(bool, string?)> Handle(ValidateEmailCommand request, CancellationToken cancellationToken)
        {
            var emailValidatedWithSuccess = await _emailService.ValidateEmail(request.EmailValidationCode);

            return emailValidatedWithSuccess ? (true, null) : (false, "Problems in validating email.");
        }
    }
}
