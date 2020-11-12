// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Pages.App.Tenants
{
    public class EditTenant : PrgPageModel<EditTenant.Model>
    {
        private readonly ITenantQueryService _tenantQueryService;
        private readonly IMediator _mediator;

        public EditTenant(ITenantQueryService tenantQueryService, IMediator mediator)
        {
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var tenantMaybe = await this._tenantQueryService.GetTenantMetadataById(this.Id);
            if (tenantMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var tenant = tenantMaybe.Value;

            this.PageModel ??= new Model
            {
                Identifier = tenant.Identifier,
                Name = tenant.Name,
                TenantId = tenant.Id,
            };

            this.Name = tenant.Name;

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
                this.PageModel.Name));

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