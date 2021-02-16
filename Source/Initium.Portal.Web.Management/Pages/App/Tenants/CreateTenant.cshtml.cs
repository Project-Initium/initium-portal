// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Settings;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace Initium.Portal.Web.Management.Pages.App.Tenants
{
    public class CreateTenant : PrgPageModel<CreateTenant.Model>
    {
        private readonly IMediator _mediator;
        private readonly MultiTenantSettings _multiTenantSettings;
        private readonly IFeatureManager _featureManager;

        public CreateTenant(IMediator mediator, IOptions<MultiTenantSettings> multiTenantSettings, IFeatureManager featureManager)
        {
            this._mediator = mediator;
            this._featureManager = featureManager;
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public List<SystemFeatures> Features { get; private set; } = new List<SystemFeatures>();

        public async Task OnGetAsync()
        {
            var features = this._featureManager.GetFeatureNamesAsync();
            await foreach (var feature in features)
            {
                if (!await this._featureManager.IsEnabledAsync(feature))
                {
                    continue;
                }

                var enumValue = Enum.Parse<SystemFeatures>(feature);
                this.Features.Add(enumValue);
            }

            this.PageModel ??= new Model();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var tenantId = Guid.NewGuid();
            var connStr = this._multiTenantSettings.DefaultTenantConnectionString;

            var result =
                await this._mediator.Send(new CreateTenantCommand(tenantId, this.PageModel.Identifier, this.PageModel.Name,
                    connStr, this.PageModel.SystemFeatures));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The system alert was created successfully", PageNotification.Success);
                return this.RedirectToPage(PageLocations.TenantView, new { id = tenantId });
            }

            this.AddPageNotification("There was an issue creating the system alert.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Model()
            {
                this.SystemFeatures = new List<SystemFeatures>();
            }

            public string Identifier { get; set; }

            public string Name { get; set; }

            public List<SystemFeatures> SystemFeatures { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();

                this.RuleFor(x => x.Identifier)
                    .NotEmpty();
            }
        }
    }
}