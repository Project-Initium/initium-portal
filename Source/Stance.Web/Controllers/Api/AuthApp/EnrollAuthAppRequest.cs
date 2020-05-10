// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Web.Controllers.Api.AuthApp
{
    public class EnrollAuthAppRequest
    {
        public string Code { get; set; }

        public string SharedKey { get; set; }
    }
}