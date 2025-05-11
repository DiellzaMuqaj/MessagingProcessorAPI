using FluentValidation;
using MessagingProcessor.DTO;
using MessagingProcessor.Models;

namespace MessagingProcessor.Validators
{
    public class MessageRequestValidator : AbstractValidator<MessageRequest>
    {
        public MessageRequestValidator()
        {
            RuleFor(x => x.Type).IsInEnum();

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");

            When(x => x.Type == MessageType.SMS, () =>
            {
                RuleFor(x => x.Recipient)
                    .Matches(@"^\+\d+$")
                    .WithMessage("SMS recipient must contain only '+' and digits.");
            });

            When(x => x.Type == MessageType.Email, () =>
            {
                RuleFor(x => x.Recipient)
                    .EmailAddress()
                    .WithMessage("Invalid email address format.");
            });

            When(x => x.Type == MessageType.PushNotification, () =>
            {
                RuleFor(x => x.Recipient)
                    .MinimumLength(10)
                    .WithMessage("Push token is too short.");
            });
        }
    }
}
