// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public abstract class BaseAuthenticationProcessCommandResult
    {
        private readonly string _emailAddress;
        private readonly string _firstName;
        private readonly string _lastName;

        protected BaseAuthenticationProcessCommandResult(Guid userId, string emailAddress, string firstName, string lastName)
            : this(userId, AuthenticationState.Completed)
        {
            this._emailAddress = emailAddress;
            this._firstName = firstName;
            this._lastName = lastName;
        }

        protected BaseAuthenticationProcessCommandResult(
            Guid userId, AuthenticationState authenticationStatus)
        {
            this.UserId = userId;
            this.AuthenticationStatus = authenticationStatus;
        }

        public enum AuthenticationState
        {
            Completed,
            AwaitingMfaEmailCode,
        }

        public Guid UserId { get; }

        public string EmailAddress
        {
            get
            {
                if (this.AuthenticationStatus != AuthenticationState.Completed)
                {
                    throw new InvalidOperationException();
                }

                return this._emailAddress;
            }
        }

        public string FirstName
        {
            get
            {
                if (this.AuthenticationStatus != AuthenticationState.Completed)
                {
                    throw new InvalidOperationException();
                }

                return this._firstName;
            }
        }

        public string LastName
        {
            get
            {
                if (this.AuthenticationStatus != AuthenticationState.Completed)
                {
                    throw new InvalidOperationException();
                }

                return this._lastName;
            }
        }

        public AuthenticationState AuthenticationStatus { get; }
    }
}