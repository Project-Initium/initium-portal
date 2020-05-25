// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Infrastructure.Constants
{
    public static class PageLocations
    {
        public const string AppDashboard = "/App/Index";
        public const string FirstRunInitialUserSetup = "/FirstRun/InitialUserSetup";
        public const string FirstRunSetupCompleted = "/FirstRun/SetupCompleted";
        public const string AuthEmailMfa = "/Auth/ValidateEmailMfaCode";
        public const string AuthAppMfa = "/Auth/ValidateAppMfaCode";

        public const string AuthRequestPassword = "/Auth/RequestPasswordReset";
        public const string AuthRequestAccountVerification = "/Auth/RequestAccountVerification";
        public const string AuthSignOut = "/Auth/SignOut";
        public const string AppProfileDetails = "/App/Profile/Details";

        public const string UserListing = "/App/UserManagement/Users/ListUsers";
        public const string UserCreate = "/App/UserManagement/Users/CreateUser";
        public const string UserEdit = "/App/UserManagement/Users/EditUser";
        public const string UserView = "/App/UserManagement/Users/ViewUser";

        public const string RoleListing = "/App/UserManagement/Roles/ListRoles";
        public const string RoleCreate = "/App/UserManagement/Roles/CreateRole";
        public const string RoleEdit = "/App/UserManagement/Roles/EditRole";
        public const string RoleView = "/App/UserManagement/Roles/ViewRole";

        public const string EnrollAuthenticatorApp = "/App/Profile/AuthenticatorApp";

        public const string SecurityKeys = "/App/Profile/SecurityKeys";

        public const string AuthDeviceMfa = "/Auth/ValidateDeviceMfa";

        public const string ChangePassword = "/App/Profile/Password";

        public const string AuthSignIn = "/Auth/UserAuthentication";

        public const string SystemAlertView = "/App/SystemAlerts/ViewSystemAlert";

        public const string SystemAlertListing = "/App/SystemAlerts/ListSystemAlerts";

        public const string SystemAlertCreate = "/App/SystemAlerts/CreateSystemAlert";

        public const string SystemAlertEdit = "/App/SystemAlerts/EditSystemAlert";
    }
}