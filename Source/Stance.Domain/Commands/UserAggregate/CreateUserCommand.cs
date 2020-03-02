﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public class CreateUserCommand : IRequest<ResultWithError<ErrorData>>
    {
        private readonly List<Guid> _roles;

        public CreateUserCommand(string emailAddress, string firstName, string lastName, bool isLockable, bool isAdmin, List<Guid> roles)
        {
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IsLockable = isLockable;
            this.IsAdmin = isAdmin;
            this._roles = roles;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public bool IsLockable { get; }

        public bool IsAdmin { get; }

        public IReadOnlyList<Guid> Roles => this._roles.AsReadOnly();
    }
}