// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Runtime.CompilerServices;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using ResultMonad;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Domain.Helpers
{
    public static class FidoHelper
    {
        public static Result<AssertionOptions, Fido2VerificationException> GenerateAssertionOptionsForUser(this IFido2 fido2, IUser user)
        {
            var existingCredentials =
                user.AuthenticatorDevices.Select(x => new PublicKeyCredentialDescriptor(x.CredentialId));
            try
            {
                var exts = new AuthenticationExtensionsClientInputs
                {
                    SimpleTransactionAuthorization = "FIDO",
                    GenericTransactionAuthorization = new TxAuthGenericArg
                    {
                        ContentType = "text/plain",
                        Content = new byte[] { 0x46, 0x49, 0x44, 0x4F },
                    },
                    UserVerificationIndex = true,
                    Location = true,
                    UserVerificationMethod = true,
                };

                var uv = UserVerificationRequirement.Discouraged;
                var options = fido2.GetAssertionOptions(
                    existingCredentials,
                    uv,
                    exts);

                return Result.Ok<AssertionOptions, Fido2VerificationException>(options);
            }
            catch (Fido2VerificationException exception)
            {
                return Result.Fail<AssertionOptions, Fido2VerificationException>(exception);
            }
        }
    }
}