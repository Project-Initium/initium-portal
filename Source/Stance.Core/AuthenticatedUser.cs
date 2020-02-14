using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Core
{
    public sealed class AuthenticatedUser
    {
        public AuthenticatedUser(Guid userId)
        {
            this.UserId = userId;
        }
        public Guid UserId { get; }
    }
}
