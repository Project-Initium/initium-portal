// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Management.Components.ProfilePanel
{
    public class ProfilePanelViewModel
    {
        public ProfilePanelViewModel()
            : this(string.Empty, string.Empty)
        {
        }

        public ProfilePanelViewModel(string emailAddress, string name)
        {
            this.EmailAddress = emailAddress;
            this.Name = name;
        }

        public string EmailAddress { get; }

        public string Name { get; }
    }
}