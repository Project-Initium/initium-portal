// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Core.Constants;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public sealed class User : Entity, IUser
    {
        private readonly List<AuthenticationHistory> _authenticationHistories;

        public User(Guid id, string emailAddress, string passwordHash, bool isLockable, DateTime whenCreated)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.PasswordHash = passwordHash;
            this.WhenCreated = whenCreated;
            this.IsLockable = isLockable;
            this.SecurityStamp = Guid.NewGuid();

            this._authenticationHistories = new List<AuthenticationHistory>();
        }

        private User()
        {
            this._authenticationHistories = new List<AuthenticationHistory>();
        }

        public string EmailAddress { get; private set; }

        public string PasswordHash { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public DateTime? WhenLastAuthenticated { get; private set; }

        public bool IsLockable { get; private set; }

        public DateTime? WhenLocked { get; private set; }

        public Guid SecurityStamp { get; private set; }

        public int AttemptsSinceLastAuthentication { get; private set; }

        public IReadOnlyList<AuthenticationHistory> AuthenticationHistories =>
            this._authenticationHistories.AsReadOnly();

        public void ProcessSuccessfulAuthenticationAttempt(DateTime whenAttempted)
        {
            this._authenticationHistories.Add(new AuthenticationHistory(Guid.NewGuid(), whenAttempted, AuthenticationHistoryType.Success));
            this.WhenLastAuthenticated = whenAttempted;
            this.WhenLocked = null;
            this.AttemptsSinceLastAuthentication = 0;
        }

        public void ProcessPartialSuccessfulAuthenticationAttempt(DateTime whenAttempted, AuthenticationHistoryType stage)
        {
            this._authenticationHistories.Add(new AuthenticationHistory(Guid.NewGuid(), whenAttempted, stage));
        }

        public void ProcessUnsuccessfulAuthenticationAttempt(DateTime whenAttempted, bool applyLock)
        {
            this._authenticationHistories.Add(new AuthenticationHistory(Guid.NewGuid(), whenAttempted, AuthenticationHistoryType.Failure));
            this.AttemptsSinceLastAuthentication++;
            if (applyLock && this.IsLockable)
            {
                this.WhenLocked = whenAttempted;
            }
        }
    }
}