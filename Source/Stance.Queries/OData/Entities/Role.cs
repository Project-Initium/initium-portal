using System;

namespace Stance.Queries.OData.Entities
{
    public sealed class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ResourceCount { get; set; }
    }
}