// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using MaybeMonad;

namespace Initium.Portal.Domain.Helpers
{
    public static class FidoHelpers
    {
        public static Maybe<AssertionOptions> GenerateAssertionOptionsForUser(this IFido2 fido2, IUser user)
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

                return Maybe.From(options);
            }
            catch (Fido2VerificationException)
            {
                return Maybe<AssertionOptions>.Nothing;
            }
        }
    }
}