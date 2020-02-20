// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
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
                whenCreated);

            Assert.Equal(id, user.Id);
            Assert.Equal(new string('*', 5), user.EmailAddress);
            Assert.Equal(new string('*', 6), user.PasswordHash);
            Assert.Equal(whenCreated, user.WhenCreated);
            Assert.Empty(user.AuthenticationHistories);
            Assert.Null(user.WhenLastAuthenticated);
            Assert.Null(user.WhenLocked);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
            Assert.True(user.IsLockable);
        }

        [Fact]
        public void ProcessSuccessfulAuthenticationAttempt_GiveValidArguments_ExpectNewEntryInHistoryAndWhenLastAuthenticatedSetAndAttemptsReset()
        {
            var user = new User(
                Guid.NewGuid(),
                new string('*', 5),
                new string('*', 6),
                true,
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
                DateTime.UtcNow);

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
    }
}