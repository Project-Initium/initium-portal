using MediatR;

namespace Stance.Domain.Events
{
    public class PasswordChangedEvent : INotification
    {
        public PasswordChangedEvent(string emailAddress, string firstName, string lastName)
        {
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }
}