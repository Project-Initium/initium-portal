// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Core.Constants;
using Stance.Core.Contracts.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public interface IUser : IAggregateRoot, IEntity
    {
        string EmailAddress { get; }

        string PasswordHash { get; }

        DateTime WhenCreated { get; }

        DateTime? WhenLastAuthenticated { get; }

        bool IsLockable { get; }

        DateTime? WhenLocked { get; }

        DateTime? WhenVerified { get; }

        DateTime? WhenDisabled { get; }

        int AttemptsSinceLastAuthentication { get; }

        bool IsAdmin { get; }

        IReadOnlyList<AuthenticationHistory> AuthenticationHistories { get; }

        IReadOnlyList<SecurityTokenMapping> SecurityTokenMappings { get; }

        IReadOnlyCollection<UserRole> UserRoles { get; }

        IReadOnlyCollection<AuthenticatorApp> AuthenticatorApps { get; }

        IReadOnlyCollection<AuthenticatorDevice> AuthenticatorDevices { get; }

        IReadOnlyCollection<PasswordHistory> PasswordHistories { get; }

        Guid SecurityStamp { get; }

        Profile Profile { get; }

        bool IsVerified { get; }

        bool IsDisabled { get; }

        void ProcessSuccessfulAuthenticationAttempt(DateTime whenAttempted);

        void ProcessPartialSuccessfulAuthenticationAttempt(DateTime whenAttempted, AuthenticationHistoryType stage);

        void ProcessUnsuccessfulAuthenticationAttempt(DateTime whenAttempted, bool applyLock);

        string GenerateNewPasswordResetToken(DateTime whenRequest, TimeSpan duration);

        string GenerateNewAccountConfirmationToken(DateTime whenRequested, TimeSpan duration);

        void ChangePassword(string passwordHash, DateTime whenChanged);

        void VerifyAccount(DateTime whenVerified);

        void CompleteTokenLifecycle(Guid token, DateTime whenHappened);

        void UpdateProfile(string firstName, string lastName);

        void UpdateSystemAccessDetails(string emailAddress, bool isLockable);

        void SetRoles(IReadOnlyList<Guid> roles);

        void SetAdminStatus(bool isAdmin);

        AuthenticatorApp EnrollAuthenticatorApp(Guid id, string key, DateTime whenEnrolled);

        void RevokeAuthenticatorApp(DateTime whenRevoked);

        AuthenticatorDevice EnrollAuthenticatorDevice(Guid id, DateTime whenEnrolled, byte[] publicKey, byte[] credentialId, Guid aaguid, int counter, string name, string credType);

        void RevokeAuthenticatorDevice(Guid id, DateTime whenRevoked);

        void UnlockAccount();

        void DisableAccount(DateTime whenDisabled);

        void EnableAccount();
    }
}