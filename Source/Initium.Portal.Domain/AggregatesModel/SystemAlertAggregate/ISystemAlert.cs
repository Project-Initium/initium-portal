// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;

namespace Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate
{
    public interface ISystemAlert : IAggregateRoot, IEntity
    {
        string Message { get; }

        SystemAlertType Type { get; }

        DateTime? WhenToShow { get; }

        DateTime? WhenToHide { get; }

        void UpdateDetails(string message, SystemAlertType type, DateTime? whenToShow, DateTime? whenToHide);
    }
}