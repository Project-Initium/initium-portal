// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Initium.Portal.Core.Exceptions
{
    [SuppressMessage("ReSharper", "S3925", Justification = "Full serialization for this exception is not needed")]
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message)
        {
        }

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}