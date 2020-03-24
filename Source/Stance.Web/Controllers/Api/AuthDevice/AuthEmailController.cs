using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fido2NetLib;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Web.Controllers.Api.AuthDevice
{
    public class AuthEmailController : Controller
    {
        private readonly IMediator _mediator;

        public AuthEmailController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize(AuthenticationSchemes = "login-partial")]
        [HttpPost("api/auth-email/request-mfa-email")]
        public async Task<IActionResult> RequestMfaEmail()
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand());

            return this.Json(new
            {
                Success = result.IsSuccess,
            });
        }
    }
}
