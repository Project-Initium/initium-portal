export class DataTablesODataProvider {
    public static providerFunction(url: string): (data: object, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => void {
        return (data: object, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => {
            let params: any = {};
            $.each(data, function (i, value: any) {
                params[i] = value;
            });

            console.log(params);

            var odataQuery: any = {
                $format: 'json'
            };

            $.each(settings.aoColumns, function (i, value) {
                var sFieldName = ((typeof value.mData === 'string') ? value.mData : null);
                if (sFieldName === null || !isNaN(Number(sFieldName))) {
                    return;
                }
                if (odataQuery.$select == null) {
                    odataQuery.$select = sFieldName;
                } else {
                    odataQuery.$select += "," + sFieldName;
                }
            });

            console.log(odataQuery)

            

            odataQuery.$skip = settings._iDisplayStart;
            if (settings._iDisplayLength > -1) {
                odataQuery.$top = settings._iDisplayLength;
            }
        
            odataQuery.$count = true;
                
        
            var asFilters = [];
            var asColumnFilters = []; //used for jquery.dataTables.columnFilter.js
            $.each(settings.aoColumns, function (i, value) {
                var sFieldName = value.sName || value.mData;
                var columnFilter = params["sSearch_" + i]; //fortunately columnFilter's _number matches the index of aoColumns
        
                if ((params.search && params.search.value || columnFilter) && value.bSearchable) {
                    switch (value.sType) {
                        case 'string':
                        case 'html':
                            if (params.search && params.search.value) {
                                asFilters.push("indexof(tolower(" + sFieldName + "), '" + params.search.value.toLowerCase() + "') gt -1");
                            }
        
                            if (columnFilter) {
                                asColumnFilters.push("indexof(tolower(" + sFieldName + "), '" + columnFilter.toLowerCase() + "') gt -1");
                            }
                            break;
        
                        case 'date':
                        case 'numeric':
                            var fnFormatValue =  (value.sType == 'numeric') ? function(val) { return val; } : function(val) { return (new Date(val)).toISOString(); }
                                    
                            if (columnFilter !== null && columnFilter !== "" && columnFilter !== "~") {
                                let asRanges = columnFilter.split("~");
                                if (asRanges[0] !== "") {
                                    asColumnFilters.push("(" + sFieldName + " gt " + fnFormatValue(asRanges[0]) + ")");
                                }
        
                                if (asRanges[1] !== "") {
                                    asColumnFilters.push("(" + sFieldName + " lt " + fnFormatValue(asRanges[1]) + ")");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
        
            if (asFilters.length > 0) {
                odataQuery.$filter = asFilters.join(" or ");
            }
        
            if (asColumnFilters.length > 0) {
                if (odataQuery.$filter !== undefined) {
                    odataQuery.$filter = " ( " + odataQuery.$filter + " ) and ( " + asColumnFilters.join(" and ") + " ) ";
                } else {
                    odataQuery.$filter = asColumnFilters.join(" and ");
                }
            }

            console.log(odataQuery)

            var asOrderBy = [];
            for (var i = 0; i < params.iSortingCols; i++) {
                asOrderBy.push(params["mDataProp_" + params["iSortCol_" + i]] + " " + (params["sSortDir_" + i] || ""));
            }

            if (asOrderBy.length > 0) {
                odataQuery.$orderby = asOrderBy.join();
            }

            console.log(odataQuery)

            $.ajax({
                url: url,
                data: odataQuery,
                success: function(returnedData) {
                    var oDataSource:any = {};
        
                    // Probe data structures for V4, V3, and V2 versions of OData response
                    oDataSource.aaData = returnedData.value || (returnedData.d && returnedData.d.results) || returnedData.d;
                    var iCount = (returnedData["@odata.count"]) ? returnedData["@odata.count"] : ((returnedData["odata.count"]) ? returnedData["odata.count"] : ((returnedData.__count) ? returnedData.__count : (returnedData.d && returnedData.d.__count)));
        
                    if (iCount == null) {
                        if (oDataSource.aaData.length === settings._iDisplayLength) {
                            oDataSource.iTotalRecords = settings._iDisplayStart + settings._iDisplayLength + 1;
                        } else {
                            oDataSource.iTotalRecords = settings._iDisplayStart + oDataSource.aaData.length;
                        }
                    } else {
                        oDataSource.iTotalRecords = iCount;
                    }
        
                    oDataSource.iTotalDisplayRecords = oDataSource.iTotalRecords;
        
                    callback(oDataSource);
                }
              });                
        }
    } 
}