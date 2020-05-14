// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Controllers.Api.AuthEmail.Models
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