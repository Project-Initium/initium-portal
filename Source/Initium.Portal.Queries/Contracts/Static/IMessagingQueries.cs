// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Static.Models.Messaging;
using MaybeMonad;

namespace Initium.Portal.Queries.Contracts.Static
{
    public interface IMessagingQueries
    {
        Task<Maybe<IReadOnlyList<ActiveSystemAlert>>> GetActiveSystemAlerts();

        Task<Maybe<DetailedSystemAlert>> GetDetailedSystemAlertById(Guid id);
    }
}