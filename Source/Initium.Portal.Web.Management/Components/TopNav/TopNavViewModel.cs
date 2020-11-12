// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Management.Components.TopNav
{
    public class TopNavViewModel
    {
        public TopNavViewModel()
            : this(string.Empty, string.Empty, default)
        {
        }

        public TopNavViewModel(string emailAddress, string name, bool hasUnreadNotifications)
        {
            this.EmailAddress = emailAddress;
            this.Name = name;
            this.HasUnreadNotifications = hasUnreadNotifications;
        }

        public string EmailAddress { get; }

        public string Name { get; }

        public bool HasUnreadNotifications { get; }
    }
}