// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure.PageModels
{
    public abstract class NotificationPageModel : PageModel
    {
        private const string Key = "NotificationPageModel";
        private readonly List<PageNotification> _pageNotifications = new List<PageNotification>();

        [ViewData(Key = nameof(PageNotifications))]
        public IReadOnlyList<PageNotification> PageNotifications => this._pageNotifications.AsReadOnly();

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
            if (!(context.Result is RedirectToPageResult) && this.TempData[$"{Key}PageNotifications"] is string serializedPageNotifications)
            {
                if (context.Result == null)
                {
                    var notifications =
                        JsonConvert.DeserializeObject<List<PageNotification>>(serializedPageNotifications);
                    this._pageNotifications.AddRange(notifications);
                }
                else
                {
                    this.TempData.Remove($"{Key}PageNotifications");
                }
            }
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
            if (context.Result is RedirectToPageResult)
            {
                var pageNotifications = JsonConvert.SerializeObject(this._pageNotifications);
                this.TempData[$"{Key}PageNotifications"] = pageNotifications;
            }
        }

        protected void AddPageNotification(string message, string pageNotificationType)
        {
            this._pageNotifications.Add(new PageNotification(message, pageNotificationType));
        }

        public class PageNotification
        {
            public const string Info = "info";
            public const string Error = "error";
            public const string Success = "success";

            public PageNotification()
            {
            }

            public PageNotification(string message, string pageNotificationType)
            {
                this.Message = message;
                this.PageNotificationType = pageNotificationType;
            }

            public string Message { get; set; }

            public string PageNotificationType { get; set; }
        }
    }
}