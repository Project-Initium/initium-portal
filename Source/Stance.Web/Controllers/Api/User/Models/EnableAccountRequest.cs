// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace Stance.Web.Controllers.Api.User.Models
{
    public class EnableAccountRequest
    {
        public Guid UserId { get; set; }

        public class Validator : AbstractValidator<EnableAccountRequest>
        {
            public Validator()
            {
                this.RuleFor(x => x.UserId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}