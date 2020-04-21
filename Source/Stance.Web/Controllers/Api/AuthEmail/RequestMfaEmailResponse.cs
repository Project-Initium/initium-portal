// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Web.Controllers.Api.AuthEmail
{
    public class RequestMfaEmailResponse
    {
        public RequestMfaEmailResponse(bool success)
        {
            this.Success = success;
        }

        public bool Success { get; }
    }
}