// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Queries.TransferObjects
{
    internal sealed class PresenceCheckDto<TPresence>
    {
        public TPresence PresenceIdentifier { get; set; }
    }
}