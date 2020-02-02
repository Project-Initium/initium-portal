// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Contracts.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public interface IUser : IAggregateRoot, IEntity
    {
        string EmailAddress { get; }

        string PasswordHash { get; }

        DateTime WhenCreated { get; }
    }
}