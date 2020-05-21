// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Controllers.Api
{
    public class BasicApiResponse
    {
        public BasicApiResponse(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }
}