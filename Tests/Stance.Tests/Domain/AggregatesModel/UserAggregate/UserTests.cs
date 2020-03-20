// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Stance.Core.Constants;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public sealed class UserTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var whenCreated = DateTime.UtcNow;
            var user = new User(
                id,
                new string('*', 5),
                new string('*', 6),
                true,
                whenCreated,
                new string('*', 7),
                new string('*', 8),
                new List<Guid> { Guid.NewGuid() }.AsReadOnly(),
                true);

            Assert.Equal(id, user.Id);
            Assert.Equal(new string('*', 5), user.EmailAddress);
            Assert.Equal(new string('*', 6), user.PasswordHash);
            Assert.Equal(whenCreated, user.WhenCreated);
            Assert.Empty(user.AuthenticationHistories);
            Assert.Null(user.WhenLastAuthenticated);
            Assert.Null(user.WhenLocked);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
            Assert.True(user.IsLockable);
            Assert.True(user.IsAdmin);
            Assert.Equal(new string('*', 7), user.Profile.FirstName);
            Assert.Equal(new string('*', 8), user.Profile.LastName);
            Assert.Single(user.UserRoles);
        }

        [Fact]
        public void ProcessSuccessfulAuthenticationAttempt_GiveValidArguments_ExpectNewEntryInHistoryAndWhenLastAuthenticatedSetAndAttemptsReset()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                true,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenAttempted = DateTime.UtcNow;
            user.ProcessUnsuccessfulAuthenticationAttempt(whenAttempted, false);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(1, user.AttemptsSinceLastAuthentication);

            user.ProcessSuccessfulAuthenticationAttempt(whenAttempted);

            Assert.Equal(whenAttempted, user.WhenLastAuthenticated);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.Success &&
                           history.WhenHappened == whenAttempted);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
        }

        [Fact]
        public void ProcessUnsuccessfulAuthenticationAttempt_GivenNoLockedIsApplied_ExpectNewEntryInHistoryAndUnsuccessfulCountIncreased()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                true,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenAttempted = DateTime.UtcNow;
            user.ProcessUnsuccessfulAuthenticationAttempt(whenAttempted, false);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(1, user.AttemptsSinceLastAuthentication);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.Failure &&
                           history.WhenHappened == whenAttempted);
        }

        [Fact]
        public void ProcessUnsuccessfulAuthenticationAttempt_GivenLockedIsAppliedAndAccountIsLockable_ExpectNewEntryInHistoryAndUnsuccessfulCountIncreasedAndAccountLocked()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                true,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenAttempted = DateTime.UtcNow;
            user.ProcessUnsuccessfulAuthenticationAttempt(whenAttempted, true);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(1, user.AttemptsSinceLastAuthentication);
            Assert.Equal(whenAttempted, user.WhenLocked);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.Failure &&
                           history.WhenHappened == whenAttempted);
        }

        [Fact]
        public void ProcessUnsuccessfulAuthenticationAttempt_GivenLockedIsAppliedAndAccountIsNotLockable_ExpectNewEntryInHistoryAndUnsuccessfulCountIncreased()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenAttempted = DateTime.UtcNow;
            user.ProcessUnsuccessfulAuthenticationAttempt(whenAttempted, true);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(1, user.AttemptsSinceLastAuthentication);
            Assert.Null(user.WhenLocked);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.Failure &&
                           history.WhenHappened == whenAttempted);
        }

        [Fact]
        public void ProcessPartialSuccessfulAuthenticationAttempt_GivenValidArguments_ExpectItemAddedToAuthenticationHistory()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenAttempted = DateTime.UtcNow;

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                whenAttempted, AuthenticationHistoryType.EmailMfaRequested);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.EmailMfaRequested &&
                           history.WhenHappened == whenAttempted);
        }

        [Fact]
        public void ChangePassword_GivenValidArguments_ExpectPasswordChanged()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.ChangePassword(new string('*', 7));

            Assert.Equal(new string('*', 7), user.PasswordHash);
        }

        [Fact]
        public void CompleteTokenLifecycle_GivenValidArguments_ExpectTokenToBeMarkedAsUsed()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenRequested = DateTime.UtcNow;
            user.GenerateNewPasswordResetToken(whenRequested, TimeSpan.FromHours(1));
            var token = (user.DomainEvents.First() as PasswordResetTokenGeneratedEvent)?.Token;

            user.CompleteTokenLifecycle(new Guid(Convert.FromBase64String(token)), whenRequested);

            Assert.Contains(user.SecurityTokenMappings, x => x.WhenUsed == whenRequested);
        }

        [Fact]
        public void GenerateNewPasswordResetToken_GivenTokenDoesNotExist_ExpectNewTokenCreatedAndEventRaised()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var whenRequested = DateTime.UtcNow;
            user.GenerateNewPasswordResetToken(whenRequested, TimeSpan.FromHours(1));

            Assert.Contains(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.PasswordReset &&
                     x.WhenCreated == whenRequested && x.WhenExpires == whenRequested.Add(TimeSpan.FromHours(1)));

            Assert.Single(user.DomainEvents);
            Assert.IsType<PasswordResetTokenGeneratedEvent>(user.DomainEvents.Last());
        }

        [Fact]
        public void GenerateNewPasswordResetToken_GivenTokenDoesExist_ExpectNoNewTokenCreatedAndEventRaised()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.GenerateNewPasswordResetToken(DateTime.UtcNow, TimeSpan.FromHours(1));

            var whenRequested = DateTime.UtcNow;
            user.GenerateNewPasswordResetToken(whenRequested, TimeSpan.FromHours(1));

            Assert.DoesNotContain(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.PasswordReset &&
                     x.WhenCreated == whenRequested);

            Assert.Equal(2, user.DomainEvents.Count);
            Assert.IsType<PasswordResetTokenGeneratedEvent>(user.DomainEvents.Last());
        }

        [Fact]
        public void UpdateProfile_GivenValidArguments_ExpectProfileToBeUpdated()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.UpdateProfile(new string('*', 9), new string('*', 10));

            Assert.Equal(new string('*', 9), user.Profile.FirstName);
            Assert.Equal(new string('*', 10), user.Profile.LastName);
        }

        [Fact]
        public void UpdateSystemAccessDetails_GivenValidArguments_ExpectPropertiesToBeUpdated()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.UpdateSystemAccessDetails(new string('+', 5), true);
            Assert.Equal(new string('+', 5), user.EmailAddress);
            Assert.True(user.IsLockable);
        }

        [Fact]
        public void SetAdminStatus_GivenValidArgument_ExpectAdminStatusChange()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.SetAdminStatus(false);

            Assert.False(user.IsAdmin);
        }

        [Fact]
        public void SetRoles_GiveValidArguments_ExpectRolesToBeUpdated()
        {
            var permRole = Guid.NewGuid();
            var tempRole = Guid.NewGuid();
            var newRole = Guid.NewGuid();
            var dupeRole = permRole;

            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid> { permRole, tempRole }.AsReadOnly(),
                true);

            user.SetRoles(new List<Guid>
            {
                permRole,
                newRole,
                dupeRole,
            });

            Assert.Single(user.UserRoles, x => x.Id == permRole);
            Assert.Single(user.UserRoles, x => x.Id == newRole);
            Assert.DoesNotContain(user.UserRoles, role => role.Id == tempRole);
        }

        [Fact]
        public void EnrollAuthenticatorApp_GivenValidArguments_ExpectAuthenticatorAppAdded()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            var authenticatorApp = user.EnrollAuthenticatorApp(Guid.NewGuid(), new string('*', 5), DateTime.UtcNow);
            Assert.Single(user.AuthenticatorApps);
            Assert.Contains(authenticatorApp, user.AuthenticatorApps);
        }

        [Fact]
        public void RevokeAuthenticatorApp_GivenThereIsAnAppEnrolled_ExpectAppToBeRevoked()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                false,
                DateTime.UtcNow,
                new string('*', 7),
                new string('*', 8),
                new List<Guid>().AsReadOnly(),
                true);

            user.EnrollAuthenticatorApp(Guid.NewGuid(), new string('*', 5), DateTime.UtcNow);
            var whenRevoked = DateTime.UtcNow;
            user.RevokeAuthenticatorApp(whenRevoked);
            Assert.Single(user.AuthenticatorApps);
            Assert.DoesNotContain(user.AuthenticatorApps, x => !x.WhenRevoked.HasValue);
        }
    }
}