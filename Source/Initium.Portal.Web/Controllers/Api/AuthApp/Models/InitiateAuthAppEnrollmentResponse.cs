// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Controllers.Api.AuthApp.Models
{
    public class InitiateAuthAppEnrollmentResponse
    {
        public InitiateAuthAppEnrollmentResponse()
        {
            this.IsSuccess = false;
        }

        public InitiateAuthAppEnrollmentResponse(string sharedKey, string formattedSharedKey, string authenticatorUri)
        {
            this.IsSuccess = true;
            this.SharedKey = sharedKey;
            this.FormattedSharedKey = formattedSharedKey;
            this.AuthenticatorUri = authenticatorUri;
        }

        public bool IsSuccess { get; }

        public string SharedKey { get; }

        public string FormattedSharedKey { get; }

        public string AuthenticatorUri { get; }
    }
}