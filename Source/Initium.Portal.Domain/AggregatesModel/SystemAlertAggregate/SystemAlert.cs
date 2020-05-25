// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate
{
    public sealed class SystemAlert : Entity, ISystemAlert
    {
        public SystemAlert(Guid id, string name, string message, SystemAlertType type,
            DateTime? whenToShow = null,
            DateTime? whenToHide = null)
        {
            this.Id = id;
            this.Name = name;
            this.Message = message;
            this.Type = type;
            this.WhenToShow = whenToShow;
            this.WhenToHide = whenToHide;
        }

        private SystemAlert()
        {
        }

        public string Name { get; private set; }

        public string Message { get; private set; }

        public SystemAlertType Type { get; private set; }

        public DateTime? WhenToShow { get; private set; }

        public DateTime? WhenToHide { get; private set; }

        public void UpdateDetails(string name, string message, SystemAlertType type, DateTime? whenToShow,
            DateTime? whenToHide)
        {
            this.Name = name;
            this.Message = message;
            this.Type = type;
            this.WhenToShow = whenToShow;
            this.WhenToHide = whenToHide;
        }
    }
}