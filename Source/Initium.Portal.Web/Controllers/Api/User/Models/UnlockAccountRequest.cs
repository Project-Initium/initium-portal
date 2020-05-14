// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace Initium.Portal.Web.Controllers.Api.User.Models
{
    public class UnlockAccountRequest
    {
        public Guid UserId { get; set; }

        public class Validator : AbstractValidator<UnlockAccountRequest>
        {
            public Validator()
            {
                this.RuleFor(x => x.UserId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}