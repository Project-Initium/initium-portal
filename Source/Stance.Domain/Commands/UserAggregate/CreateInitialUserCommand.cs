// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public sealed class CreateInitialUserCommand : IRequest<ResultWithError<ErrorData>>
    {
        public CreateInitialUserCommand(string emailAddress, string password, string firstName, string lastName)
        {
            this.EmailAddress = emailAddress;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string EmailAddress { get; }

        public string Password { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }
}