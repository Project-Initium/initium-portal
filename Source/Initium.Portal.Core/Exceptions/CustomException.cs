// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace Initium.Portal.Core.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message)
            : base(message)
        {
        }

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}