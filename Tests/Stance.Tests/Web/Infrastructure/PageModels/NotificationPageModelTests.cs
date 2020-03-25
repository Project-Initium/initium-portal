// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json;
using Stance.Web.Infrastructure.PageModels;
using Xunit;

namespace Stance.Tests.Web.Infrastructure.PageModels
{
    public class NotificationPageModelTests
    {
        [Fact]
        public void AddPageNotification_GivenValidArguments_AddsNotification()
        {
            var page = new TestableNotificationPageModel();
            page.AddNotification();

            Assert.Single(page.PageNotifications);
            Assert.Contains(page.PageNotifications, x => x.Title == "some-title" && x.Message == "some-message");
        }

        [Fact]
        public void OnPageHandlerExecuted_GivenResultIsNotRedirectToPageResult_ExpectNoItemInTempData()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));

            var model = new Mock<PageModel>();

            var pageHandlerExecutedContext = new PageHandlerExecutedContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                model.Object) { Result = new PageResult() };

            var page = new TestableNotificationPageModel();
            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Null(page.TempData);
        }

        [Fact]
        public void OnPageHandlerExecuted_IfResultIsRedirectToPageResult_ExpectItemInTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            tempDataDictionary.Setup(x => x[It.IsAny<string>()])
                .Callback((string key) =>
            {
                Assert.Equal("NotificationPageModelPageNotifications", key);
            });

            var page = new TestableNotificationPageModel { TempData = tempDataDictionary.Object };

            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));

            var pageHandlerExecutedContext = new PageHandlerExecutedContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                page) { Result = new RedirectToPageResult("page-name") };

            page.AddNotification();
            page.OnPageHandlerExecuted(pageHandlerExecutedContext);
        }

        [Fact]
        public void OnPageHandlerExecuting_GivenResultIsRedirectToPageResult_ExpectNoPageItems()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            tempDataDictionary.Setup(x => x[It.IsAny<string>()])
                .Returns(() => JsonConvert.SerializeObject(new NotificationPageModel.PageNotification
                {
                    Title = "some-title",
                    Message = "some-message",
                }));

            var page = new TestableNotificationPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                new Dictionary<string, object>(),
                page) { Result = new RedirectToPageResult("some-page") };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Empty(page.PageNotifications);
        }

        [Fact]
        public void OnPageHandlerExecuting_GivenResultIsNotRedirectToPageResultNothingInTempData_ExpectNoPageItems()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            tempDataDictionary.Setup(x => x[It.IsAny<string>()])
                .Returns(() => null);

            var page = new TestableNotificationPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                new Dictionary<string, object>(),
                page) { Result = new PageResult() };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Empty(page.PageNotifications);
        }

        [Fact]
        public void OnPageHandlerExecuting_GivenResultIsNull_ExpectPageItems()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            tempDataDictionary.Setup(x => x[It.IsAny<string>()])
                .Returns(() => JsonConvert.SerializeObject(new List<NotificationPageModel.PageNotification>
                {
                    new NotificationPageModel.PageNotification
                    {
                        Title = "some-title",
                        Message = "some-message",
                    },
                }));

            var page = new TestableNotificationPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                new Dictionary<string, object>(),
                page) { Result = null };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Single(page.PageNotifications);
        }

        [Fact]
        public void OnPageHandlerExecuting_GivenResultIsNotNull_ExpectPageItems()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            tempDataDictionary.Setup(x => x[It.IsAny<string>()])
                .Returns(() => JsonConvert.SerializeObject(new List<NotificationPageModel.PageNotification>
                {
                    new NotificationPageModel.PageNotification
                    {
                        Title = "some-title",
                        Message = "some-message",
                    },
                }));

            tempDataDictionary.Setup(x => x.Remove(It.IsAny<string>()));

            var page = new TestableNotificationPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                pageContext,
                Array.Empty<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                new Dictionary<string, object>(),
                page) { Result = new PageResult() };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            tempDataDictionary.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        private class TestableNotificationPageModel : NotificationPageModel
        {
            public void AddNotification()
            {
                this.AddPageNotification("some-title", "some-message", PageNotification.Success);
            }
        }
    }
}