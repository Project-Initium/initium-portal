// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.SystemAlertAggregate
{
    public class DeleteSystemAlertCommandHandler : IRequestHandler<DeleteSystemAlertCommand, ResultWithError<ErrorData>>
    {
        private readonly ISystemAlertRepository _systemAlertRepository;
        private readonly ILogger<DeleteSystemAlertCommandHandler> _logger;

        public DeleteSystemAlertCommandHandler(ISystemAlertRepository systemAlertRepository, ILogger<DeleteSystemAlertCommandHandler> logger)
        {
            this._systemAlertRepository = systemAlertRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(DeleteSystemAlertCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._systemAlertRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(
            DeleteSystemAlertCommand request, CancellationToken cancellationToken)
        {
            var alertMaybe = await this._systemAlertRepository.Find(request.SystemAlertId, cancellationToken);
            if (alertMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail(new ErrorData(ErrorCodes.SystemAlertNotFound));
            }

            var alert = alertMaybe.Value;
            this._systemAlertRepository.Delete(alert);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}