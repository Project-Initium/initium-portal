// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.ApiEndpoints
{
    public class BasicEndpointResponse
    {
        public BasicEndpointResponse(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }
}