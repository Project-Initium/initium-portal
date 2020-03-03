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

        public const string UserListing = "/App/UserManagement/Users/ListUsers";
        public const string UserCreate = "/App/UserManagement/Users/CreateUser";
        public const string UserEdit = "/App/UserManagement/Users/EditUser";
        public const string UserView = "/App/UserManagement/Users/ViewUser";

        public const string RoleListing = "/App/UserManagement/Roles/ListRoles";
        public const string RoleCreate = "/App/UserManagement/Roles/CreateRole";
        public const string RoleEdit = "/App/UserManagement/Roles/EditRole";
        public const string RoleView = "/App/UserManagement/Roles/ViewRole";
    }
}