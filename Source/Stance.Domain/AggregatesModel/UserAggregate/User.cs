// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Stance.Core.Constants;
using Stance.Core.Domain;
using Stance.Domain.Events;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public sealed class User : Entity, IUser
    {
        private readonly List<AuthenticationHistory> _authenticationHistories;
        private readonly List<SecurityTokenMapping> _securityTokenMappings;

        public User(Guid id, string emailAddress, string passwordHash, bool isLockable, DateTime whenCreated)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.PasswordHash = passwordHash;
            this.WhenCreated = whenCreated;
            this.IsLockable = isLockable;
            this.SecurityStamp = Guid.NewGuid();

            this._authenticationHistories = new List<AuthenticationHistory>();
            this._securityTokenMappings = new List<SecurityTokenMapping>();
        }

        private User()
        {
            this._authenticationHistories = new List<AuthenticationHistory>();
            this._securityTokenMappings = new List<SecurityTokenMapping>();
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

        public IReadOnlyList<SecurityTokenMapping> SecurityTokenMappings =>
            this._securityTokenMappings.AsReadOnly();

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

        public void GenerateNewPasswordResetToken(DateTime whenRequest, TimeSpan duration)
        {
            var token =
                this._securityTokenMappings.FirstOrDefault(m =>
                    m.WhenUsed == null && m.WhenExpires >= whenRequest &&
                    m.Purpose == SecurityTokenMapping.SecurityTokenPurpose.PasswordReset);
            if (token == null)
            {
                token = new SecurityTokenMapping(Guid.NewGuid(), SecurityTokenMapping.SecurityTokenPurpose.PasswordReset, whenRequest,
                    whenRequest.Add(duration));
                this._securityTokenMappings.Add(token);
            }

            this.AddDomainEvent(new PasswordResetTokenGeneratedEvent(this.Id, this.EmailAddress, token.Token));
        }

        public void ChangePassword(string passwordHash)
        {
            this.PasswordHash = passwordHash;
        }

        public void CompleteTokenLifecycle(Guid token, DateTime whenHappened)
        {
            var securityTokenMapping = this._securityTokenMappings.First(x => x.Id == token);
            securityTokenMapping.MarkUsed(whenHappened);
        }
    }
}