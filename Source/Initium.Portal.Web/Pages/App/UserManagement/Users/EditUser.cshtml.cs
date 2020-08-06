// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Initium.Portal.Web.Pages.App.UserManagement.Users
{
    [ResourceBasedAuthorize("user-edit")]
    public class EditUser : PrgPageModel<EditUser.Model>
    {
        private readonly IMediator _mediator;
        private readonly IUserQueryService _userQueryService;
        private readonly IRoleQueryService _roleQueryService;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public EditUser(IUserQueryService userQueryService, IMediator mediator, IRoleQueryService roleQueryService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._roleQueryService = roleQueryService ?? throw new ArgumentNullException(nameof(roleQueryService));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string LockedStatus { get; private set; }

        public string AuthenticationStatus { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public string Name { get; private set; }

        public List<SelectListItem> AvailableRoles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.NotFound();
            }

            if (maybe.Value.UserId == this.Id)
            {
                return this.NotFound();
            }

            var userMaybe = await this._userQueryService.GetDetailsOfUserById(this.Id);
            if (userMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var user = userMaybe.Value;
            this.PageModel ??= new Model
            {
                EmailAddress = user.EmailAddress,
                IsLockable = user.IsLockable,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.UserId,
                IsAdmin = user.IsAdmin,
                Roles = user.Resources.ToList(),
            };

            this.Name = $"{user.FirstName} {user.LastName}";
            this.WhenCreated = user.WhenCreated;

            this.AuthenticationStatus = user.WhenLastAuthenticated.HasValue
                ? user.WhenLastAuthenticated.ToString()
                : "Never Authenticated";

            if (user.IsLockable)
            {
                this.LockedStatus = user.WhenLocked.HasValue ? user.WhenLocked.ToString() : "Not Locked";
            }
            else
            {
                this.LockedStatus = "User is not lockable";
            }

            this.AvailableRoles = new List<SelectListItem>();
            var roles = await this._roleQueryService.GetSimpleRoles();
            if (roles.HasValue)
            {
                this.AvailableRoles.AddRange(roles.Value.Select(x => new SelectListItem(x.Name, x.Id.ToString())));
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateUserCoreDetailsCommand(
                this.PageModel.UserId,
                this.PageModel.EmailAddress,
                this.PageModel.FirstName,
                this.PageModel.LastName,
                this.PageModel.IsLockable,
                this.PageModel.IsAdmin,
                this.PageModel.Roles));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The user was updated successfully", PageNotification.Success);
                return this.RedirectToPage(PageLocations.UserView, new { id = this.PageModel.UserId });
            }

            this.AddPageNotification("There was an issue updating the user.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Model()
            {
                this.Roles = new List<Guid>();
            }

            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            [Display(Name = "Is Lockable")]
            public bool IsLockable { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public Guid UserId { get; set; }

            [Display(Name = "Is Admin")]
            public bool IsAdmin { get; set; }

            public List<Guid> Roles { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .EmailAddress();
                this.RuleFor(x => x.FirstName)
                    .NotEmpty();
                this.RuleFor(x => x.LastName)
                    .NotEmpty();
                this.RuleFor(x => x.UserId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}