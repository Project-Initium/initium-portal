// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure.PageModels
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
                        var modelState = serializedModelState.ToModelState();
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
                    var modelState = context.ModelState.ToSerializedString();
                    this.TempData[$"{Key}ModelState"] = modelState;

                    var pageModel = JsonConvert.SerializeObject(this.PageModel);
                    this.TempData[$"{Key}PageModel"] = pageModel;
                }
                else if (this.PrgState == PrgState.Failed || this.PrgState == PrgState.InError)
                {
                    var pageModel = JsonConvert.SerializeObject(this.PageModel);
                    this.TempData[$"{Key}PageModel"] = pageModel;
                }
            }
        }

        public override PageResult Page()
        {
            this.PrgState = PrgState.Default;
            if (this.TempData.ContainsKey($"{Key}ModelState"))
            {
                this.TempData.Remove($"{Key}ModelState");
            }

            if (this.TempData.ContainsKey($"{Key}PageModel"))
            {
                this.TempData.Remove($"{Key}PageModel");
            }

            return base.Page();
        }
    }
}