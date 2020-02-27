// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.RoleAggregate
{
    public class UpdateRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedAndIsUnique_ExpectSuccessfulResultAndRoleUpdated()
        {
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedButIsNotUnique_ExpectFailedResult()
        {
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasNotChanged_ExpectSuccessfulResultAndRoleUpdated()
        {
        }

        [Fact]
        public async Task Handle_GivenRoleDoesNotExist_ExpectFailedResult()
        {
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
        }
    }
}