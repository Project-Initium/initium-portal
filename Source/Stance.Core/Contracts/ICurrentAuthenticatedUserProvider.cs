// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MaybeMonad;

namespace Stance.Core.Contracts
{
    public interface ICurrentAuthenticatedUserProvider
    {
        Maybe<AuthenticatedUser> CurrentAuthenticatedUser { get; }
    }
}