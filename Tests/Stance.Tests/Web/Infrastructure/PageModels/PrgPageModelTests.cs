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
using Stance.Web.Infrastructure.PageModels;
using Xunit;

namespace Stance.Tests.Web.Infrastructure.PageModels
{
    public class PrgPageModelTests
    {
        [Fact]
        public void OnPageHandlerExecuted_ResultIsNotRedirectToPageResult_ExpectNoAttemptsToAccessTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupSet(x => x["PrgPageModelModelState"] = It.IsAny<object>()).Callback(() =>
            {
                modelStateDataCount++;
            });
            tempDataDictionary.SetupSet(x => x["PrgPageModelPageModel"] = It.IsAny<object>()).Callback(() =>
            {
                pageModelDataCount++;
            });

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
                    model.Object)
                { Result = new PageResult() };

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };
            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Equal(0, modelStateDataCount);
            Assert.Equal(0, pageModelDataCount);
        }

        [Fact]
        public void
            OnPageHandlerExecuted_ResultIsRedirectToPageResultAndModelStateIsValidAndPrgStateIsError_ExpectOneAttempt2ToAccessTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupSet(x => x["PrgPageModelModelState"] = It.IsAny<object>()).Callback(() =>
            {
                modelStateDataCount++;
            });
            tempDataDictionary.SetupSet(x => x["PrgPageModelPageModel"] = It.IsAny<object>()).Callback(() =>
            {
                pageModelDataCount++;
            });
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
                    model.Object)
                { Result = new RedirectToPageResult("some-page") };

            var page = new TestablePrgPageModel { TempData = tempDataDictionary.Object, PrgState = PrgState.InError };

            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Equal(0, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
        }

        [Fact]
        public void
            OnPageHandlerExecuted_ResultIsRedirectToPageResultAndModelStateIsValidAndPrgStateIsFailed_ExpectOneAttemptToAccessTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupSet(x => x["PrgPageModelModelState"] = It.IsAny<object>()).Callback(() =>
            {
                modelStateDataCount++;
            });
            tempDataDictionary.SetupSet(x => x["PrgPageModelPageModel"] = It.IsAny<object>()).Callback(() =>
            {
                pageModelDataCount++;
            });
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
                    model.Object)
                { Result = new RedirectToPageResult("some-page") };

            var page = new TestablePrgPageModel { TempData = tempDataDictionary.Object, PrgState = PrgState.Failed };

            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Equal(0, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
        }

        [Fact]
        public void
            OnPageHandlerExecuted_ResultIsRedirectToPageResultAndModelStateIsValidAndPrgStateIsNeitherFailedOrInError_ExpectNoAttemptsToAccessTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupSet(x => x["PrgPageModelModelState"] = It.IsAny<object>()).Callback(() =>
            {
                modelStateDataCount++;
            });
            tempDataDictionary.SetupSet(x => x["PrgPageModelPageModel"] = It.IsAny<object>()).Callback(() =>
            {
                pageModelDataCount++;
            });

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
                    model.Object)
                { Result = new RedirectToPageResult("some-page") };

            var page = new TestablePrgPageModel { TempData = tempDataDictionary.Object };

            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Equal(0, modelStateDataCount);
            Assert.Equal(0, pageModelDataCount);
        }

        [Fact]
        public void
            OnPageHandlerExecuted_ResultIsRedirectToPageResultAndModelStateNotValidAndPrgStateIsFailedOrInError_ExpectTwoAttemptsToAccessTempData()
        {
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupSet(x => x["PrgPageModelModelState"] = It.IsAny<object>()).Callback(() =>
            {
                modelStateDataCount++;
            });
            tempDataDictionary.SetupSet(x => x["PrgPageModelPageModel"] = It.IsAny<object>()).Callback(() =>
            {
                pageModelDataCount++;
            });
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));

            pageContext.ModelState.AddModelError<TestablePrgPageModel.Model>(x => x.Name, "some-error");

            var model = new Mock<PageModel>();

            var pageHandlerExecutedContext = new PageHandlerExecutedContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    model.Object)
                { Result = new RedirectToPageResult("some-page") };

            var page = new TestablePrgPageModel { TempData = tempDataDictionary.Object, PrgState = PrgState.InError };

            page.OnPageHandlerExecuted(pageHandlerExecutedContext);

            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNotRedirectToPageResultAndTempDataContainsModelState_ExpectTwoAttemptToAccessTempDataAndOneAttemptToRemoveTempData()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(
                    "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"some-error\"]}]");
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns(null);

            tempDataDictionary.Setup(x => x.Remove(It.IsAny<string>()));

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = new PageResult() };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal(default, page.PageModel);
            Assert.True(pageContext.ModelState.IsValid);
            tempDataDictionary.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNotRedirectToPageResultAndTempDataContainsPageObject_ExpectTwoAttemptToAccessTempDataAndOneAttemptToRemoveTempData()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(null);
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns("{\"name\": \"some-data\"}");

            tempDataDictionary.Setup(x => x.Remove(It.IsAny<string>()));

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = new PageResult() };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal(default, page.PageModel);
            Assert.True(pageContext.ModelState.IsValid);
            tempDataDictionary.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNotRedirectToPageResultAndTempDataContainsPageObjectAndModelState_ExpectTwoAttemptToAccessTempDataAndTwoAttemptToRemoveTempData()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(
                    "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"some-error\"]}]");
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns("{\"name\": \"some-data\"}");

            tempDataDictionary.Setup(x => x.Remove(It.IsAny<string>()));

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = new PageResult() };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal(default, page.PageModel);
            Assert.True(pageContext.ModelState.IsValid);
            tempDataDictionary.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNullAndTempDataContainsModelState_ExpectTwoAttemptToAccessTempDataAndDeserializedModelStateAndNullPageObject()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(
                    "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"some-error\"]}]");
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns(null);

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = null };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal(default, page.PageModel);
            Assert.False(pageContext.ModelState.IsValid);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNullAndTempDataContainsPageObject_ExpectTwoAttemptToAccessTempDataAndEmptyModelStateAndDeserializedPageObject()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(null);
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns("{\"name\": \"some-data\"}");

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = null };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal("some-data", page.PageModel.Name);
            Assert.True(pageContext.ModelState.IsValid);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNullAndTempDataContainsPageObjectAndModelState_ExpectTwoAttemptToAccessTempDataAndDeserializedModelStateAndDeserializedPageObject()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(
                    "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"some-error\"]}]");
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns("{\"name\": \"some-data\"}");

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = null };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal("some-data", page.PageModel.Name);
            Assert.False(pageContext.ModelState.IsValid);
        }

        [Fact]
        public void
            OnPageHandlerExecuting_GivenResultIsNullAndTempDataIsEmpty_ExpectTwoAttemptToAccessTempDataAndEmptyModelStateAndNullPageObject()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; })
                .Returns(null);
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; })
                .Returns(null);

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = null };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(1, modelStateDataCount);
            Assert.Equal(1, pageModelDataCount);
            Assert.Equal(default, page.PageModel);
            Assert.True(pageContext.ModelState.IsValid);
        }

        [Fact]
        public void OnPageHandlerExecuting_GivenResultIsRedirectToPageResult_ExpectNoAccessToTempData()
        {
            var pageContext = new PageContext(new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new PageActionDescriptor(),
                new ModelStateDictionary()));
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            var modelStateDataCount = 0;
            var pageModelDataCount = 0;

            tempDataDictionary.SetupGet(x => x["PrgPageModelModelState"]).Callback(() => { modelStateDataCount++; });
            tempDataDictionary.SetupGet(x => x["PrgPageModelPageModel"]).Callback(() => { pageModelDataCount++; });

            var page = new TestablePrgPageModel
            {
                TempData = tempDataDictionary.Object,
            };

            var pageHandlerExecutingContext = new PageHandlerExecutingContext(
                    pageContext,
                    Array.Empty<IFilterMetadata>(),
                    new HandlerMethodDescriptor(),
                    new Dictionary<string, object>(),
                    page)
                { Result = new RedirectToPageResult("some-page") };

            page.OnPageHandlerExecuting(pageHandlerExecutingContext);
            Assert.Equal(0, modelStateDataCount);
            Assert.Equal(0, pageModelDataCount);
        }

        private class TestablePrgPageModel : PrgPageModel<TestablePrgPageModel.Model>
        {
            public class Model
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Used as part of unit testing")]
                public string Name { get; set; }
            }
        }
    }
}