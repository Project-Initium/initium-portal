// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;

namespace Initium.Portal.Queries.Models.Messaging
{
    public class ActiveSystemAlert
    {
        public ActiveSystemAlert(string message, SystemAlertType type)
        {
            this.Message = message;
            this.Type = type;
        }

        public string Message { get; }

        public SystemAlertType Type { get; }
    }
}