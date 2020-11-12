// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentValidation;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Settings;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Web.Management.Pages.App.Tenants
{
    public class CreateTenant : PrgPageModel<CreateTenant.Model>
    {
        private readonly IMediator _mediator;
        private readonly IMultiTenantStore<TenantInfo> _multiTenantStore;
        private readonly MultiTenantSettings _multiTenantSettings;

        public CreateTenant(IMediator mediator, IMultiTenantStore<TenantInfo> multiTenantStore, IOptions<MultiTenantSettings> multiTenantSettings)
        {
            if (multiTenantSettings == null)
            {
                throw new ArgumentNullException(nameof(multiTenantSettings));
            }

            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._multiTenantStore = multiTenantStore ?? throw new ArgumentNullException(nameof(multiTenantStore));
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var tenantId = Guid.NewGuid();
            string connStr;

            if (this._multiTenantSettings.MultiTenantType == MultiTenantType.TableSplit)
            {
                connStr = this._multiTenantSettings.DefaultTenantConnectionString;
            }
            else
            {
                var builder = new SqlConnectionStringBuilder(this._multiTenantSettings.DefaultTenantConnectionString)
                {
                    InitialCatalog = tenantId.ToString(),
                };
                connStr = builder.ConnectionString;
            }

            var result =
                await this._mediator.Send(new CreateTenantCommand(tenantId, this.PageModel.Identifier, this.PageModel.Name,
                    connStr));

            if (result.IsSuccess)
            {
                await this._multiTenantStore.TryAddAsync(new TenantInfo
                {
                    Id = tenantId.ToString(),
                    Identifier = this.PageModel.Identifier,
                    Name = this.PageModel.Name,
                    ConnectionString = connStr,
                });
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
            public string Identifier { get; set; }

            public string Name { get; set; }
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