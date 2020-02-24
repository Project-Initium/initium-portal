// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Web.Infrastructure.Constants
{
    public static class PageLocations
    {
        public const string AppDashboard = "/App/Index";
        public const string FirstRunInitialUserSetup = "/FirstRun/InitialUserSetup";
        public const string FirstRunSetupCompleted = "/FirstRun/SetupCompleted";
        public const string AuthEmailMfa = "/Auth/ValidateEmailMfaCode";

        public const string AuthRequestPassword = "/Auth/RequestPassword";
        public const string AuthSignOut = "/Auth/SignOut";
        public const string AppProfileDetails = "/App/Profile/Details";
    }
}