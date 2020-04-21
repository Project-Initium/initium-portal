// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace Stance.Core.Exceptions
{
    [Serializable]
    public class StanceException : Exception
    {
        public StanceException()
        {
        }

        public StanceException(string message)
            : base(message)
        {
        }

        public StanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected StanceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}