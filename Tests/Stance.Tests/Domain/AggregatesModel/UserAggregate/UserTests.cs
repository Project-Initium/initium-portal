// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                true,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid> { TestVariables.RoleId },
                true);

            Assert.Equal(TestVariables.UserId, user.Id);
            Assert.Equal("email-address", user.EmailAddress);
            Assert.Equal("password-hash", user.PasswordHash);
            Assert.Equal(TestVariables.Now, user.WhenCreated);
            Assert.Empty(user.AuthenticationHistories);
            Assert.Null(user.WhenLastAuthenticated);
            Assert.Null(user.WhenLocked);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
            Assert.True(user.IsLockable);
            Assert.True(user.IsAdmin);
            Assert.Equal("first-name", user.Profile.FirstName);
            Assert.Equal("last-name", user.Profile.LastName);
            Assert.Single(user.UserRoles);
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var user = (User)Activator.CreateInstance(typeof(User), true);
            Assert.NotNull(user);

            foreach (var prop in user.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(user, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void ProcessSuccessfulAuthenticationAttempt_GiveValidArguments_ExpectNewEntryInHistoryAndWhenLastAuthenticatedSetAndAttemptsReset()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                true,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
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
                TestVariables.UserId,
                "email-address",
                "password-hash",
                true,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
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
                TestVariables.UserId,
                "email-address",
                "password-hash",
                true,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
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
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.ProcessUnsuccessfulAuthenticationAttempt(TestVariables.Now.AddMinutes(30), true);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(1, user.AttemptsSinceLastAuthentication);
            Assert.Null(user.WhenLocked);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.Failure &&
                           history.WhenHappened == TestVariables.Now.AddMinutes(30));
        }

        [Fact]
        public void ProcessPartialSuccessfulAuthenticationAttempt_GivenValidArguments_ExpectItemAddedToAuthenticationHistory()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.ProcessPartialSuccessfulAuthenticationAttempt(
                TestVariables.Now.AddMinutes(30), AuthenticationHistoryType.EmailMfaRequested);

            Assert.Null(user.WhenLastAuthenticated);
            Assert.Equal(0, user.AttemptsSinceLastAuthentication);
            Assert.Contains(
                user.AuthenticationHistories,
                history => history.AuthenticationHistoryType == AuthenticationHistoryType.EmailMfaRequested &&
                           history.WhenHappened == TestVariables.Now.AddMinutes(30));
        }

        [Fact]
        public void ChangePassword_GivenValidArguments_ExpectPasswordChangedAndSecurityStampChanged()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            var securityStamp = user.SecurityStamp;
            user.ChangePassword("new-password-hash", TestVariables.Now);

            Assert.Equal("new-password-hash", user.PasswordHash);
            Assert.NotEqual(securityStamp, user.SecurityStamp);
            Assert.Contains(
                user.PasswordHistories,
                history => history.Hash == "new-password-hash" && history.WhenUsed == TestVariables.Now);
        }

        [Fact]
        public void CompleteTokenLifecycle_GivenValidArguments_ExpectTokenToBeMarkedAsUsed()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            var token = user.GenerateNewPasswordResetToken(TestVariables.Now.AddMinutes(30), TimeSpan.FromHours(1));

            user.CompleteTokenLifecycle(new Guid(Convert.FromBase64String(token)), TestVariables.Now.AddMinutes(30));

            Assert.Contains(user.SecurityTokenMappings, x => x.WhenUsed == TestVariables.Now.AddMinutes(30));
        }

        [Fact]
        public void GenerateNewPasswordResetToken_GivenTokenDoesNotExist_ExpectNewTokenCreated()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.GenerateNewPasswordResetToken(TestVariables.Now.AddMinutes(30), TimeSpan.FromHours(1));

            Assert.Contains(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.PasswordReset &&
                     x.WhenCreated == TestVariables.Now.AddMinutes(30) && x.WhenExpires == TestVariables.Now.AddMinutes(30).Add(TimeSpan.FromHours(1)));
        }

        [Fact]
        public void GenerateNewPasswordResetToken_GivenTokenDoesExist_ExpectNoNewTokenCreated()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.GenerateNewPasswordResetToken(TestVariables.Now, TimeSpan.FromHours(1));

            user.GenerateNewPasswordResetToken(TestVariables.Now.AddMinutes(30), TimeSpan.FromHours(1));

            Assert.DoesNotContain(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.PasswordReset &&
                     x.WhenCreated == TestVariables.Now.AddMinutes(30));
        }

        [Fact]
        public void UpdateProfile_GivenValidArguments_ExpectProfileToBeUpdated()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                string.Empty,
                string.Empty,
                new List<Guid>(),
                true);

            user.UpdateProfile("first-name", "last-name");

            Assert.Equal("first-name", user.Profile.FirstName);
            Assert.Equal("last-name", user.Profile.LastName);
        }

        [Fact]
        public void UpdateSystemAccessDetails_GivenValidArguments_ExpectPropertiesToBeUpdated()
        {
            var user = new User(
                TestVariables.UserId,
                string.Empty,
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.UpdateSystemAccessDetails("email-address", true);
            Assert.Equal("email-address", user.EmailAddress);
            Assert.True(user.IsLockable);
        }

        [Fact]
        public void SetAdminStatus_GivenValidArgument_ExpectAdminStatusChange()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
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
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
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
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            var authenticatorApp = user.EnrollAuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now);
            Assert.Single(user.AuthenticatorApps);
            Assert.Contains(authenticatorApp, user.AuthenticatorApps);
        }

        [Fact]
        public void RevokeAuthenticatorApp_GivenThereIsAnAppEnrolled_ExpectAppToBeRevoked()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.EnrollAuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now);
            user.RevokeAuthenticatorApp(TestVariables.Now.AddMinutes(30));
            Assert.Single(user.AuthenticatorApps);
            Assert.DoesNotContain(user.AuthenticatorApps, x => !x.WhenRevoked.HasValue);
        }

        [Fact]
        public void GenerateNewAccountConfirmationToken_GivenTokenDoesNotExist_ExpectNewTokenCreated()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.GenerateNewAccountConfirmationToken(TestVariables.Now.AddMinutes(30), TimeSpan.FromHours(1));

            Assert.Contains(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.AccountConfirmation &&
                     x.WhenCreated == TestVariables.Now.AddMinutes(30) && x.WhenExpires == TestVariables.Now.AddMinutes(30).Add(TimeSpan.FromHours(1)));
        }

        [Fact]
        public void GenerateNewAccountConfirmationToken_GivenTokenDoesExist_ExpectNoNewTokenCreated()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.GenerateNewAccountConfirmationToken(TestVariables.Now.AddMinutes(30), TimeSpan.FromHours(1));

            user.GenerateNewAccountConfirmationToken(TestVariables.Now.AddMinutes(45), TimeSpan.FromHours(1));

            Assert.DoesNotContain(
                user.SecurityTokenMappings,
                x => x.Purpose == SecurityTokenMapping.SecurityTokenPurpose.AccountConfirmation &&
                     x.WhenCreated == TestVariables.Now.AddMinutes(45));
        }

        [Fact]
        public void VerifyAccount_GivenValidArguments_ExpectAccountToBeVerified()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            Assert.Null(user.WhenVerified);
            Assert.False(user.IsVerified);

            user.VerifyAccount(TestVariables.Now.AddMinutes(30));

            Assert.Equal(TestVariables.Now.AddMinutes(30), user.WhenVerified);
            Assert.True(user.IsVerified);
        }

        [Fact]
        public void EnrollAuthenticatorDevice_GivenValidArguments_ExpectDeviceToBeAdded()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            var device = user.EnrollAuthenticatorDevice(TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey, TestVariables.AuthenticatorDeviceCredentialId,
                TestVariables.AuthenticatorDeviceAaguid, 1, "name", "cred-type");

            Assert.Contains(device, user.AuthenticatorDevices);
        }

        [Fact]
        public void RevokeAuthenticatorDevice_GivenValidArguments_ExpectDeviceToBeRevoked()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            var device = user.EnrollAuthenticatorDevice(TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey, TestVariables.AuthenticatorDeviceCredentialId,
                TestVariables.AuthenticatorDeviceAaguid, 1, "name", "cred-type");

            user.RevokeAuthenticatorDevice(device.Id, TestVariables.Now.AddMinutes(30));

            Assert.Equal(TestVariables.Now.AddMinutes(30), device.WhenRevoked);
        }

        [Fact]
        public void UnlockAccount_GivenValidArguments_ExpectAccountToBeUnlock()
        {
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                true,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>(),
                true);

            user.ProcessUnsuccessfulAuthenticationAttempt(TestVariables.Now, true);
            user.UnlockAccount();

            Assert.Null(user.WhenLocked);
        }
    }
}