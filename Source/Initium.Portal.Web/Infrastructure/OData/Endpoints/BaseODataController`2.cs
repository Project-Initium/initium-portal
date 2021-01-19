// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Initium.Portal.Core.Exceptions;
using Initium.Portal.Core.Infrastructure;
using Initium.Portal.Web.Infrastructure.Extensions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;

namespace Initium.Portal.Web.Infrastructure.ODataEndpoints
{
    public abstract class BaseODataController<TReadEntity, TFilterModel> : ODataController
        where TReadEntity : ReadEntity, new()
        where TFilterModel : IODataFilter
    {
        public abstract IActionResult Filtered(ODataQueryOptions<TReadEntity> options, TFilterModel filter);

        public abstract IActionResult FilteredExport(ODataQueryOptions<TReadEntity> options, ExportableFilter<TFilterModel> filter);

        protected abstract ExpressionStarter<TReadEntity> GeneratePredicate(TFilterModel filter);

        protected Stream GenerateCsvStream(IQueryable query, ODataQueryOptions<TReadEntity> options, IDictionary<string, string> mappings)
        {
            var results = options.ApplyTo(query);
            var resultList = new List<TReadEntity>();
            foreach (var item in results)
            {
                if (item is TReadEntity readEntity)
                {
                    resultList.Add(readEntity);
                }
                else if (item.GetType().Name == "SelectSome`1")
                {
                    var dict = ((ISelectExpandWrapper)item).ToDictionary();
                    var model = dict.ToObject<TReadEntity>();
                    resultList.Add(model);
                }
            }

            var type = typeof(TReadEntity);
            var selectedProperties = options.SelectExpand.RawSelect.Split(',');
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            if (options.SelectExpand == null || string.IsNullOrEmpty(options.SelectExpand.RawSelect))
            {
                throw new CustomException("Can only generate CSV when a custom select is used.");
            }

            foreach (var heading in options.SelectExpand.RawSelect.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                csvWriter.WriteField(mappings.ContainsKey(heading) ? mappings[heading] : heading);
            }

            csvWriter.NextRecord();
            foreach (var resultItem in resultList)
            {
                foreach (var selectedProperty in selectedProperties)
                {
                    var prop = type.GetProperty(selectedProperty);
                    if (prop != null)
                    {
                        csvWriter.WriteField(prop.GetValue(resultItem));
                    }
                    else
                    {
                        csvWriter.WriteField(string.Empty);
                    }
                }

                csvWriter.NextRecord();
            }

            streamWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        protected bool AreOptionsValid(ODataQueryOptions<TReadEntity> options)
        {
            try
            {
                options.Validate(new ODataValidationSettings
                {
                    AllowedQueryOptions = AllowedQueryOptions.All,
                    MaxTop = 1000,
                });
            }
            catch (ODataException)
            {
                return false;
            }

            return true;
        }
    }
}