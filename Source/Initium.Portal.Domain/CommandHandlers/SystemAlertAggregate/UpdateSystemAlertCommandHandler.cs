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
    public class UpdateSystemAlertCommandHandler : IRequestHandler<UpdateSystemAlertCommand, ResultWithError<ErrorData>>
    {
        private readonly ISystemAlertRepository _systemAlertRepository;
        private readonly ILogger<UpdateSystemAlertCommandHandler> _logger;

        public UpdateSystemAlertCommandHandler(ISystemAlertRepository systemAlertRepository, ILogger<UpdateSystemAlertCommandHandler> logger)
        {
            this._systemAlertRepository = systemAlertRepository;
            this._logger = logger;
        }

        public async Task<ResultWithError<ErrorData>> Handle(UpdateSystemAlertCommand request, CancellationToken cancellationToken)
        {
            var result = await this.Process(request, cancellationToken);
            var dbResult = await this._systemAlertRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return ResultWithError.Fail(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private async Task<ResultWithError<ErrorData>> Process(
            UpdateSystemAlertCommand request, CancellationToken cancellationToken)
        {
            var alertMaybe = await this._systemAlertRepository.Find(request.SystemAlertId, cancellationToken);
            if (alertMaybe.HasNoValue)
            {
                this._logger.LogDebug("Entity not found.");
                return ResultWithError.Fail<ErrorData>(new ErrorData(ErrorCodes.SystemAlertNotFound));
            }

            var alert = alertMaybe.Value;
            alert.UpdateDetails(request.Name, request.Message, request.Type, request.WhenToShow, request.WhenToHide);
            this._systemAlertRepository.Update(alert);
            return ResultWithError.Ok<ErrorData>();
        }
    }
}