// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Initium.Portal.Web.ApiEndpoints.AuthDevice
{
    public class AssertionOptionsPost : BaseEndpoint
        .WithoutRequest
        .WithResponse<BasicEndpointResponse>
    {
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public AssertionOptionsPost(ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            this._tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "login-partial")]
        [Route("api/auth-device/assertion-options", Name = "AssertionOptionsPostEndpoint")]
        public override ActionResult<BasicEndpointResponse> Handle()
        {
            var tempData = this._tempDataDictionaryFactory.GetTempData(this.HttpContext);

            var options = tempData["fido2.assertionOptions"] as string;

            tempData["fido2.assertionOptions"] = options;

            return this.Content(options, "application/json");
        }
    }
}