// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Exceptions;
using MediatR;

namespace Initium.Portal.Infrastructure.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] _validators;

        public ValidatorBehavior(
            IValidator<TRequest>[] validators)
        {
            this._validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var failures = this._validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                throw new StanceException(
                    $"Command Validation Errors for type {typeof(TRequest).Name}",
                    new ValidationException("Validation exception", failures));
            }

            return await next();
        }
    }
}