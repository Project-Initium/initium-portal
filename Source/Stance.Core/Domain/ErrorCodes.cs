// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Core.Domain
{
    public enum ErrorCodes
    {
        SavingChanges = 1,
        SystemIsAlreadySetup = 2,
        UserNotFound = 3,
        AuthenticationFailed = 4,
        MfaCodeNotValid = 5,
        UserAlreadyExists = 6,
    }
}