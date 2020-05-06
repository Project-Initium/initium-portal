// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Web.Controllers.Api.User
{
    public class UnlockAccountRequest
    {
        public Guid UserId { get; set; }
    }
}