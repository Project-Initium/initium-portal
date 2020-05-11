// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Stance.Web.Controllers.Api.AuthApp.Models
{
    public class RevokeAuthAppRequest
    {
        public string Password { get; set; }

        public class RevokeAuthAppRequestValidator : AbstractValidator<RevokeAuthAppRequest>
        {
            public RevokeAuthAppRequestValidator()
            {
                this.RuleFor(x => x.Password)
                    .NotEmpty();
            }
        }
    }
}