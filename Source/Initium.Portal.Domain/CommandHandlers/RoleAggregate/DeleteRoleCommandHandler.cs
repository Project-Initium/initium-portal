// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.RoleAggregate
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ResultWithError<ErrorData>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository, ILogger<DeleteRoleCommandHandler> logger)
        {
            this._roleRepository = roleRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(
            DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            if (dbResult.Error is InUsePersistenceError)
            {
                this._logger.LogDebug("Failed presence check");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleInUse));
            }

            this._logger.LogDebug("Failed saving changes");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(
            DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleMaybe = await this._roleRepository.Find(request.RoleId, cancellationToken);
            if (roleMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.RoleNotFound));
            }

            var role = roleMaybe.Value;

            this._roleRepository.Delete(role);

            return ResultWithError.Ok<ErrorData>();
        }
    }
}