// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Core.Domain
{
    public sealed class ErrorData
    {
        public ErrorData(ErrorCodes code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public ErrorData(ErrorCodes code)
            : this(code, code.ToString())
        {
        }

        public ErrorCodes Code { get; }

        public string Message { get; }
    }
}