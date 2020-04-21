// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Stance.Core.Constants;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public sealed class User : Entity, IUser
    {
        private readonly List<AuthenticationHistory> _authenticationHistories;
        private readonly List<SecurityTokenMapping> _securityTokenMappings;
        private readonly List<UserRole> _userRoles;
        private readonly List<AuthenticatorApp> _authenticatorApps;
        private readonly List<AuthenticatorDevice> _authenticatorDevices;

        public User(Guid id, string emailAddress, string passwordHash, bool isLockable, DateTime whenCreated, string firstName, string lastName, IReadOnlyList<Guid> roles, bool isAdmin)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.PasswordHash = passwordHash;
            this.WhenCreated = whenCreated;
            this.IsLockable = isLockable;
            this.SecurityStamp = Guid.NewGuid();
            this.IsAdmin = isAdmin;

            this.Profile = new Profile(this.Id, firstName, lastName);

            this._authenticationHistories = new List<AuthenticationHistory>();
            this._securityTokenMappings = new List<SecurityTokenMapping>();
            this._userRoles = roles.Select(x => new UserRole(x)).ToList();
            this._authenticatorApps = new List<AuthenticatorApp>();
            this._authenticatorDevices = new List<AuthenticatorDevice>();
        }

        private User()
        {
            this._authenticationHistories = new List<AuthenticationHistory>();
            this._securityTokenMappings = new List<SecurityTokenMapping>();
            this._userRoles = new List<UserRole>();
            this._authenticatorApps = new List<AuthenticatorApp>();
            this._authenticatorDevices = new List<AuthenticatorDevice>();
        }

        public string EmailAddress { get; private set; }

        public string PasswordHash { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public DateTime? WhenLastAuthenticated { get; private set; }

        public DateTime? WhenVerified { get; private set; }

        public bool IsLockable { get; private set; }

        public DateTime? WhenLocked { get; private set; }

        public Guid SecurityStamp { get; private set; }

        public Profile Profile { get; private set; }

        public int AttemptsSinceLastAuthentication { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool IsVerified => this.WhenVerified.HasValue;

        public IReadOnlyList<AuthenticationHistory> AuthenticationHistories =>
            this._authenticationHistories.AsReadOnly();

        public IReadOnlyList<SecurityTokenMapping> SecurityTokenMappings =>
            this._securityTokenMappings.AsReadOnly();

        public IReadOnlyCollection<UserRole> UserRoles => this._userRoles.AsReadOnly();

        public IReadOnlyCollection<AuthenticatorApp> AuthenticatorApps => this._authenticatorApps.AsReadOnly();

        public IReadOnlyCollection<AuthenticatorDevice> AuthenticatorDevices => this._authenticatorDevices.AsReadOnly();

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

        public string GenerateNewPasswordResetToken(DateTime whenRequest, TimeSpan duration)
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

            return token.Token;
        }

        public string GenerateNewAccountConfirmationToken(DateTime whenRequested, TimeSpan duration)
        {
            var token =
                this.SecurityTokenMappings.FirstOrDefault(m =>
                    m.WhenUsed == null && m.WhenExpires >= whenRequested &&
                    m.Purpose == SecurityTokenMapping.SecurityTokenPurpose.AccountConfirmation);
            if (token == null)
            {
                token = new SecurityTokenMapping(Guid.NewGuid(), SecurityTokenMapping.SecurityTokenPurpose.AccountConfirmation, whenRequested,
                    whenRequested.Add(duration));
                this._securityTokenMappings.Add(token);
            }

            return token.Token;
        }

        public void ChangePassword(string passwordHash)
        {
            this.PasswordHash = passwordHash;
            this.SecurityStamp = Guid.NewGuid();
        }

        public void VerifyAccount(DateTime whenVerified)
        {
            this.WhenVerified = whenVerified;
        }

        public void CompleteTokenLifecycle(Guid token, DateTime whenHappened)
        {
            var securityTokenMapping = this._securityTokenMappings.First(x => x.Id == token);
            securityTokenMapping.MarkUsed(whenHappened);
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            this.Profile.UpdateProfile(firstName, lastName);
        }

        public void UpdateSystemAccessDetails(string emailAddress, bool isLockable)
        {
            this.EmailAddress = emailAddress;
            this.IsLockable = isLockable;
        }

        public void SetRoles(IReadOnlyList<Guid> roles)
        {
            var distinctRoles = roles.Distinct().ToList();
            var currentRoles = this._userRoles.Select(x => x.Id).ToList();
            var toAdd = distinctRoles.Except(currentRoles);
            var toRemove = currentRoles.Except(distinctRoles);

            foreach (var item in toRemove)
            {
                this._userRoles.Remove(this._userRoles.Single(x => x.Id == item));
            }

            this._userRoles.AddRange(toAdd.Select(x => new UserRole(x)));
        }

        public void SetAdminStatus(bool isAdmin)
        {
            this.IsAdmin = isAdmin;
        }

        public AuthenticatorApp EnrollAuthenticatorApp(Guid id, string key, DateTime whenEnrolled)
        {
            var authenticatorApp = new AuthenticatorApp(id, key, whenEnrolled);
            this._authenticatorApps.Add(authenticatorApp);
            return authenticatorApp;
        }

        public void RevokeAuthenticatorApp(DateTime whenRevoked)
        {
            this._authenticatorApps.Single(x => x.WhenRevoked == null).RevokeApp(whenRevoked);
        }

        public AuthenticatorDevice EnrollAuthenticatorDevice(Guid id, DateTime whenEnrolled, byte[] publicKey,
            byte[] credentialId, Guid aaguid, int counter, string name, string credType)
        {
            var authenticatorDevice = new AuthenticatorDevice(id, whenEnrolled, publicKey, credentialId, aaguid,
                counter, name, credType);
            this._authenticatorDevices.Add(authenticatorDevice);
            return authenticatorDevice;
        }

        public void RevokeAuthenticatorDevice(Guid id, DateTime whenRevoked)
        {
            this._authenticatorDevices.Single(x => x.Id == id).RevokeDevice(whenRevoked);
        }

        public void UnlockAccount()
        {
            this.WhenLocked = null;
        }
    }
}