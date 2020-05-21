// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Domain.CommandResults.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandResult
    {
        public CreateNewSystemAlertCommandResult(Guid systemAlertId)
        {
            this.SystemAlertId = systemAlertId;
        }

        public Guid SystemAlertId { get; }
    }
}