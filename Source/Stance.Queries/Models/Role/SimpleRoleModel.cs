// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Queries.Models.Role
{
    public class SimpleRoleModel
    {
        public SimpleRoleModel(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}