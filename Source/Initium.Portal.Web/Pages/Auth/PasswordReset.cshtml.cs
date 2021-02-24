// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Caching;
using Initium.Portal.Core.Settings;
using Initium.Portal.Core.Validation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.EventHandlers.Models;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Initium.Portal.Web.Pages.Auth
{
    public class PasswordReset : PrgPageModel<PasswordReset.Model>
    {
        private readonly IMediator _mediator;
        private readonly IDataSerializer _dataSerializer;
        private readonly IClock _clock;

        public PasswordReset(IMediator mediator, IDataSerializer dataSerializer, IClock clock)
        {
            this._mediator = mediator;
            this._dataSerializer = dataSerializer;
            this._clock = clock;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [TempData]
        public bool TokenError { get; set; }

        public void OnGet()
        {
            var maybe = this._dataSerializer.DeserializeFromBase64<SecurityToken>(this.Token);
            if (maybe.HasNoValue || DateTimeOffset.FromUnixTimeSeconds(maybe.Value.WhenExpires) < this._clock.GetCurrentInstant().ToDateTimeOffset())
            {
                this.TokenError = true;
            }
            else
            {
                this.PageModel ??= new Model
                {
                    Token = maybe.Value.Token,
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage(new { this.PageModel.Token });
            }

            var result =
                await this._mediator.Send(new PasswordResetCommand(this.PageModel.Token, this.PageModel.Password));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                return this.RedirectToPage();
            }

            this.TokenError = true;
            this.PrgState = PrgState.Failed;

            return this.RedirectToPage(new { this.PageModel.Token });
        }

        public class Model
        {
            public Guid Token { get; set; }

            public string Password { get; set; }

            [Display(Name = "Password Confirmation")]
            public string PasswordConfirmation { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator(IOptions<SecuritySettings> securitySettings)
            {
                this.RuleFor(x => x.Token).NotEmpty();
                this.RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Please enter your new password.")
                    .SetValidator(new PasswordValidator(securitySettings.Value));
                this.RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password).WithMessage("Please confirm your new password.");
            }
        }
    }
}