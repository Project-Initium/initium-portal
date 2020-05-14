// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Initium.Portal.Web.Controllers.Api.AuthApp.Models
{
    public class EnrollAuthAppRequest
    {
        public string Code { get; set; }

        public string SharedKey { get; set; }

        public class Validator : AbstractValidator<EnrollAuthAppRequest>
        {
            public Validator()
            {
                this.RuleFor(x => x.SharedKey)
                    .NotEmpty();
                this.RuleFor(x => x.Code)
                    .NotEmpty();
            }
        }
    }
}