// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class ValidateEmailMfaCode : PrgPageModel<ValidateEmailMfaCode.Model>
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            throw new NotImplementedException();
        }

        public class Model
        {
            public string Code { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Code).NotEmpty();
            }
        }
    }
}