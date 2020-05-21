// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.SystemAlertAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommand : IRequest<Result<CreateNewSystemAlertCommandResult, ErrorData>>
    {
        public CreateNewSystemAlertCommand(string message, SystemAlertType type, DateTime? whenToShow,
            DateTime? whenToHide)
        {
            this.Message = message;
            this.Type = type;
            this.WhenToShow = whenToShow;
            this.WhenToHide = whenToHide;
        }

        public string Message { get; }

        public SystemAlertType Type { get; }

        public DateTime? WhenToShow { get; }

        public DateTime? WhenToHide { get; }
    }
}