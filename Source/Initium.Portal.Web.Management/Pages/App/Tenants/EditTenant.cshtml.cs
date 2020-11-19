// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.Tenant;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace Initium.Portal.Web.Management.Pages.App.Tenants
{
    public class EditTenant : PrgPageModel<EditTenant.Model>
    {
        private readonly ITenantQueryService _tenantQueryService;
        private readonly IMediator _mediator;
        private readonly IFeatureManager _featureManager;

        public EditTenant(ITenantQueryService tenantQueryService, IMediator mediator, IFeatureManager featureManager)
        {
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public TenantMetadata Tenant { get; private set; }

        public List<SystemFeatures> Features { get; private set; } = new List<SystemFeatures>();

        public async Task<IActionResult> OnGetAsync()
        {
            var tenantMaybe = await this._tenantQueryService.GetTenantMetadataById(this.Id);
            if (tenantMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.Tenant = tenantMaybe.Value;

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

            this.PageModel ??= new Model
            {
                Identifier = this.Tenant.Identifier,
                Name = this.Tenant.Name,
                TenantId = this.Tenant.Id,
                SystemFeatures = this.Tenant.SystemFeatures.ToList(),
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateTenantCommand(
                this.PageModel.TenantId,
                this.PageModel.Identifier,
                this.PageModel.Name,
                this.PageModel.SystemFeatures));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The tenant was updated successfully", PageNotification.Success);
                return this.RedirectToPage(PageLocations.TenantView, new { id = this.PageModel.TenantId });
            }

            this.AddPageNotification("There was an issue updating the tenant.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Guid TenantId { get; set; }

            public string Identifier { get; set; }

            public string Name { get; set; }

            public List<SystemFeatures> SystemFeatures { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.TenantId)
                    .NotEqual(Guid.Empty);

                this.RuleFor(x => x.Name)
                    .NotEmpty();

                this.RuleFor(x => x.Identifier)
                    .NotEmpty();
            }
        }
    }
}