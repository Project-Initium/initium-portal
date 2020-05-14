// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Controllers.Api.User.Models
{
    public class UnlockAccountResponse
    {
        public UnlockAccountResponse(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }
}