// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Constants;
using Stance.Domain.AggregatesModel.UserAggregate;
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
    }
}