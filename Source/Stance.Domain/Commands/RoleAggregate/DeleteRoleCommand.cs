﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.RoleAggregate
{
    public class DeleteRoleCommand : IRequest<ResultWithError<ErrorData>>
    {
        public DeleteRoleCommand(Guid roleId)
        {
            this.RoleId = roleId;
        }

        public Guid RoleId { get; }
    }
}