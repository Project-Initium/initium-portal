// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.SystemAlertAggregate
{
    public class DeleteSystemAlertCommand : IRequest<ResultWithError<ErrorData>>
    {
        public DeleteSystemAlertCommand(Guid systemAlertId)
        {
            this.SystemAlertId = systemAlertId;
        }

        public Guid SystemAlertId { get; }
    }
}