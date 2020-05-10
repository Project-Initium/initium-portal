// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Stance.Web.Controllers.Api.AuthApp
{
    public class EnrollAuthAppRequestValidator : AbstractValidator<EnrollAuthAppRequest>
    {
        public EnrollAuthAppRequestValidator()
        {
            this.RuleFor(x => x.SharedKey)
                .NotEmpty();
            this.RuleFor(x => x.Code)
                .NotEmpty();
        }
    }
}