// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ResultMonad;
using Serilog;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Infrastructure.Extensions;

namespace Stance.Infrastructure.Behaviors
{
    public class AuditBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _auditLogger;
        private readonly ICurrentAuthenticatedUserProvider _authenticatedUserProvider;

        public AuditBehavior(ILogger auditLogger, ICurrentAuthenticatedUserProvider authenticatedUserProvider)
        {
            this._auditLogger = auditLogger;
            this._authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            var currentUser = this._authenticatedUserProvider.CurrentAuthenticatedUser;
            Type resultType;
            if (response.GetType().UnderlyingSystemType.Name == "Result`2")
            {
                resultType = typeof(Result<,>).MakeGenericType(
                    response.GetType().GetGenericArguments().First(), typeof(ErrorData));
            }
            else
            {
                resultType = typeof(ResultWithError<>).MakeGenericType(typeof(ErrorData));
            }

            var propToCheck = resultType.GetProperty("IsSuccess");
            var propValue = propToCheck.GetValue(response);

            if (propValue != null)
            {
                var commandSuccess = (bool)propValue;
                if (commandSuccess)
                {
                    this._auditLogger.Information(
                        "The action {CommandName} was executed successfully. It's data was {@CommandData}. The user was {User}",
                        request.GetGenericTypeName(), request,
                        currentUser.HasValue ? currentUser.Value.UserId.ToString() : "Unknown");
                }
                else
                {
                    this._auditLogger.Error(
                        "The action {CommandName} failed to execute. It's data was {@CommandData}. The user was {User}",
                        request.GetGenericTypeName(), request,
                        currentUser.HasValue ? currentUser.Value.UserId.ToString() : "Unknown");
                }
            }

            return response;
        }
    }
}