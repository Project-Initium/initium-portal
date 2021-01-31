using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.OData.EntityBuilders
{
    public class UserODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            var user = builder.EntitySet<UserReadEntity>("Users");
            // user.EntityType.Name = "User";
            //
            // user.EntityType.HasKey(entity => entity.Id);
            // user.EntityType.Property(entity => entity.EmailAddress);

            var function = user.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserReadEntity>("Users");
            //function.Namespace = "User";
            //
            function = user.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            // function.Namespace = "User";
        }
    }

    // public class RoleODateEntityBuilder : IODataEntityBuilder
    // {
    //     public void Configure(ODataConventionModelBuilder builder)
    //     {
    //         var role = builder.EntitySet<RoleReadEntity>("Role");
    //         var function = role.EntityType.Collection.Function("Filtered");
    //         function.ReturnsCollectionFromEntitySet<RoleReadEntity>("Role");
    //         function.Namespace = "Role";
    //
    //         function = role.EntityType.Collection.Function("FilteredExport");
    //         function.Returns<FileResult>();
    //         function.Namespace = "Role";
    //     }
    // }
    //
    // public class UserNotificationODateEntityBuilder : IODataEntityBuilder
    // {
    //     public void Configure(ODataConventionModelBuilder builder)
    //     {
    //         var userNotification = builder.EntitySet<UserNotification>("UserNotification");
    //         userNotification.EntityType.HasKey(uN =>
    //             new
    //             {
    //                 uN.NotificationId, uN.UserId,
    //             });
    //         var function = userNotification.EntityType.Collection.Function("Filtered");
    //         function.ReturnsCollectionFromEntitySet<UserNotification>("UserNotification");
    //         function.Namespace = "UserNotification";
    //
    //         function = userNotification.EntityType.Collection.Function("FilteredExport");
    //         function.Returns<FileResult>();
    //         function.Namespace = "UserNotification";
    //     }
    // }
    //
    // public class SystemAlertODateEntityBuilder : IODataEntityBuilder
    // {
    //     public void Configure(ODataConventionModelBuilder builder)
    //     {
    //         var systemAlert = builder.EntitySet<SystemAlertReadEntity>("SystemAlert");
    //         var function = systemAlert.EntityType.Collection.Function("Filtered");
    //         function.ReturnsCollectionFromEntitySet<SystemAlertReadEntity>("SystemAlert");
    //         function.Namespace = "SystemAlert";
    //
    //         function = systemAlert.EntityType.Collection.Function("FilteredExport");
    //         function.Returns<FileResult>();
    //         function.Namespace = "SystemAlert";
    //     }
    // }
    

}