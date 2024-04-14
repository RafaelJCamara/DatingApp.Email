using MediatR;

namespace Email.Application.UseCases.EmailValidation.Commands
{
    public sealed record ValidateEmailCommand(string EmailValidationCode) : IRequest<(bool, string?)>;
}
