// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Stance.Web.Infrastructure.Extensions;

namespace Stance.Web.Infrastructure.PageModels
{
    public abstract class PrgPageModel<TModel> : NotificationPageModel
    {
        private const string Key = "PrgPageModel";

        [BindProperty]
        public TModel PageModel { get; set; }

        [TempData]
        public PrgState PrgState { get; set; }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
            if (!(context.Result is RedirectToPageResult))
            {
                if (this.TempData[$"{Key}ModelState"] is string serializedModelState)
                {
                    if (context.Result == null)
                    {
                        var modelState = serializedModelState.DeserializeModelState();
                        context.ModelState.Merge(modelState);
                    }
                    else
                    {
                        this.TempData.Remove($"{Key}ModelState");
                    }
                }

                if (this.TempData[$"{Key}PageModel"] is string serializePageModel)
                {
                    if (context.Result == null)
                    {
                        this.PageModel = JsonConvert.DeserializeObject<TModel>(serializePageModel);
                    }
                    else
                    {
                        this.TempData.Remove($"{Key}PageModel");
                    }
                }
            }
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);
            if (context.Result is RedirectToPageResult)
            {
                if (!context.ModelState.IsValid)
                {
                    if (context.ModelState != null)
                    {
                        var modelState = context.ModelState.ToSerializedString();
                        this.TempData[$"{Key}ModelState"] = modelState;
                    }

                    if (this.PageModel != null)
                    {
                        var pageModel = JsonConvert.SerializeObject(this.PageModel);
                        this.TempData[$"{Key}PageModel"] = pageModel;
                    }
                }
            }
        }
    }
}