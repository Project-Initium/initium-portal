// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.CommandResults.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandHandler : IRequestHandler<CreateNewSystemAlertCommand,
        Result<CreateNewSystemAlertCommandResult, ErrorData>>
    {
        private readonly ISystemAlertRepository _systemAlertRepository;
        private readonly ILogger<CreateNewSystemAlertCommandHandler> _logger;

        public CreateNewSystemAlertCommandHandler(ISystemAlertRepository systemAlertRepository, ILogger<CreateNewSystemAlertCommandHandler> logger)
        {
            this._systemAlertRepository = systemAlertRepository;
            this._logger = logger;
        }

        public async Task<Result<CreateNewSystemAlertCommandResult, ErrorData>> Handle(
            CreateNewSystemAlertCommand request, CancellationToken cancellationToken)
        {
            var result = this.Process(request);
            var dbResult = await this._systemAlertRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<CreateNewSystemAlertCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private Result<CreateNewSystemAlertCommandResult, ErrorData> Process(
            CreateNewSystemAlertCommand request)
        {
            var alert = this._systemAlertRepository.Add(new SystemAlert(Guid.NewGuid(), request.Name, request.Message, request.Type,
                request.WhenToShow, request.WhenToHide));

            return Result.Ok<CreateNewSystemAlertCommandResult, ErrorData>(
                new CreateNewSystemAlertCommandResult(alert.Id));
        }
    }
}