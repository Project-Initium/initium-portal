﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Caching;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.EventHandlers.Models;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Domain.EventHandlers.IntegrationEventHandlers
{
    public class ProcessAccountConfirmationToken : INotificationHandler<AccountConfirmationTokenGeneratedIntegrationEvent>
    {
        private readonly ILogger<ProcessAccountConfirmationToken> _logger;
        private readonly FeatureBasedTenantInfo _tenantInfo;
        private readonly IDataSerializer _dataSerializer;

        public ProcessAccountConfirmationToken(ILogger<ProcessAccountConfirmationToken> logger, FeatureBasedTenantInfo tenantInfo, IDataSerializer dataSerializer)
        {
            this._logger = logger;
            this._tenantInfo = tenantInfo;
            this._dataSerializer = dataSerializer;
        }

        public Task Handle(AccountConfirmationTokenGeneratedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var result = this._dataSerializer.SerializeToBase64(new SecurityToken(
                notification.Token,
                new DateTimeOffset(notification.WhenExpires).ToUnixTimeSeconds()));
            if (result.IsSuccess)
            {
                this._logger.LogInformation($"Account Confirmation Token for Tenant {this._tenantInfo.Name} with email {notification.EmailAddress} is {result.Value}");
            }

            return Task.FromResult(0);
        }
    }
}