﻿// Copyright (c) DeviousCreation. All rights reserved.
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

        int AttemptsSinceLastAuthentication { get; }

        bool IsAdmin { get; }

        IReadOnlyList<AuthenticationHistory> AuthenticationHistories { get; }

        IReadOnlyList<SecurityTokenMapping> SecurityTokenMappings { get; }

        IReadOnlyCollection<UserRole> UserRoles { get; }

        Guid SecurityStamp { get; }

        Profile Profile { get; }

        void ProcessSuccessfulAuthenticationAttempt(DateTime whenAttempted);

        void ProcessPartialSuccessfulAuthenticationAttempt(DateTime whenAttempted, AuthenticationHistoryType stage);

        void ProcessUnsuccessfulAuthenticationAttempt(DateTime whenAttempted, bool applyLock);

        void GenerateNewPasswordResetToken(DateTime whenRequest, TimeSpan duration);

        void ChangePassword(string passwordHash);

        void CompleteTokenLifecycle(Guid token, DateTime whenHappened);

        void UpdateProfile(string firstName, string lastName);

        void UpdateSystemAccessDetails(string emailAddress, bool isLockable);

        void SetRoles(IReadOnlyList<Guid> roles);

        void SetAdminStatus(bool isAdmin);
    }
}