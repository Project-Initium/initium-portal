export interface IODataRequest {
    $select?: string;
    $skip?: number;
    $top?: number;
    $count: boolean;
    $filter?: string;
    $orderby?: string;
}

export interface IStateData {
    time: number;
    start: number;
    length: number;
    order: any[][];
    search: {
        search: string;
        smart: boolean;
    }
}
export interface ISimpleStateData {
    s: number;
    l: number;
    c: number;
    d: string;
    p: string
}

export interface ISettings  {
    route: string;
    externalFilter?: (filterQuery: IODataRequest) => Promise<any>;
    externalState?: (stateData: IStateData, simpleStateData: ISimpleStateData) => {data: IStateData, simpleStateData: ISimpleStateData};
    externalHydration?: (params: URLSearchParams) => void;
    externalStateManager?: (state:IStateData) => void;
}

export class CustomizedDataTable {
    public readonly tableApi: DataTables.Api;
    private popped: boolean = false;
    private searchField: JQuery<HTMLElement>;

    constructor(tableElement: JQuery<HTMLElement>, private settings: ISettings, opts: DataTables.Settings) {
        const contextThis = this;
        opts.ajax = this.providerFunction(this.settings.route, this.settings.externalFilter);
        opts.stateSaveCallback = this.stateSaveCallback(this.settings.externalState);
        opts.stateLoadCallback = this.stateLoadCallback(this.settings.externalHydration);
        opts.stateSave = true;
        opts.processing = true;
        opts.serverSide = true;
        this.tableApi = tableElement.DataTable(opts);
        window.addEventListener('popstate', (event) => contextThis.listenForHistoryChange(event));
        this.searchField = tableElement.find('.dataTables_filter input[type=search]')
    }

    private providerFunction(url: string, externalFilter?: ((filterQuery: IODataRequest) => Promise<any>)): (data: object, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => void {
        return (data: any, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => {
            const request: IODataRequest = {
                $count: false
            };

            settings.aoColumns.forEach(value => {
                var fieldName = CustomizedDataTable.getColumnFieldName(value);
                if (!fieldName) {
                    return;
                }
                if (!request.$select) {
                    request.$select = fieldName;
                } else {
                    request.$select += ',' + fieldName;
                }
            });

            request.$skip = settings._iDisplayStart;
            if (settings._iDisplayLength > -1) {
                request.$top = settings._iDisplayLength;
            }
            request.$count = true;

            var filters = [];
            var columnFilters = [];
            var globalFilter = data.search.value;

            settings.aoColumns.forEach((value, i) => {
                var fieldName = CustomizedDataTable.getColumnFieldName(value);
                var columnFilter = data.columns[i].search.value;
                var columnType = (value as any).type || 'string';

                if ((!globalFilter && !columnFilter) || !value.bSearchable || !fieldName) {
                    return;
                }

                switch (columnType) {
                    case "string":
                    case "html":
                        if (globalFilter && globalFilter.trim()) {
                            filters.push("indexof(tolower(" + fieldName + "), '" + globalFilter.toLowerCase() + "') gt -1");
                        }

                        if (columnFilter && columnFilter.trim()) {
                            columnFilters.push("indexof(tolower(" + fieldName + "), '" + columnFilter.toLowerCase() + "') gt -1");
                        }
                        break;

                    case "date":
                    case "num":
                    case "numeric":
                    case "number":
                        var parseValue: any = function (val) {
                            var f = Number.parseFloat(val);
                            if (isNaN(f)) return null;
                            return f;
                        }
                        if (columnType === "date") {
                            parseValue = function (val) {
                                var d:Date = new Date(val);
                                if (!d) return null;
                                return d.toISOString();
                            }
                        }

                        var processRange = function (val) {
                            var result = "";
                            var separator = "";
                            var range = val.split("~");
                            var formattedValue;

                            if (range.length > 1) {
                                formattedValue = parseValue(range[0]);
                                if (formattedValue) {
                                    result = fieldName + " ge " + formattedValue;
                                    separator = " and ";
                                }
                                formattedValue = parseValue(range[1]);
                                if (formattedValue) {
                                    result += separator + fieldName + " le " + formattedValue;
                                }
                            } else {
                                formattedValue = parseValue(val);
                                if (formattedValue) {
                                    result = fieldName + " eq " + formattedValue;
                                }
                            }

                            if (result) {
                                result = "(" + result + ")";
                            }

                            return result;
                        }

                        // Numeric and date filters are supported also in form lower~upper
                        if (columnFilter && columnFilter !== "~") {
                            var colFilter = processRange(columnFilter);
                            if (colFilter) { columnFilters.push(colFilter); }
                        }

                        if (globalFilter && globalFilter !== "~") {
                            var globFilter = processRange(globalFilter);
                            if (globFilter) { filters.push(globFilter); }
                        }

                        break;
                    default:
                }
            });

            if (filters.length > 0) {
                request.$filter = filters.join(' or ');
            }

            if (columnFilters.length > 0) {
                if (request.$filter) {
                    request.$filter = ' ( ' + request.$filter + ' ) and ( ' + columnFilters.join(' and ') + ' ) ';
                } else {
                    request.$filter = columnFilters.join(' and ');
                }
            }

            var orderBy = [];
            $.each(data.order, function (i, value) {
                orderBy.push(CustomizedDataTable.getColumnFieldName(settings.aoColumns[value.column]) + ' ' + value.dir);
            });

            if (orderBy.length > 0) {
                request.$orderby = orderBy.join();
            }

            if (settings.oInit.oDataAbort) {
                if (settings.jqXHR && settings.jqXHR.readystate != 4) {
                    settings.jqXHR.abort();
                }
            }

            let p: Promise<any>;
            if (externalFilter) {
                p = externalFilter(request);
            } else {
                var getUrl = new URL(url, document.location.origin);
                Object.keys(request).forEach(key => getUrl.searchParams.append(key, request[key]))
                p = fetch(getUrl.toString(), {
                    method: 'GET',
                    mode: 'same-origin',
                    cache: 'no-cache',
                    credentials: 'same-origin',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    redirect: 'follow',
                    referrerPolicy: 'no-referrer',
                }).then((response) => {
                    return response.json()
                });
            }
            p.then((data: any) => {
                var dataSource: any = {
                    draw: parseInt(data.draw) // Cast for security reason
                };

                dataSource.data = data.value;
                var recordCount = data['@odata.count'];

                if (recordCount != null) {
                    dataSource.recordsFiltered = recordCount;
                } else {
                    if (dataSource.data.length === settings._iDisplayLength) {
                        dataSource.recordsFiltered = settings._iDisplayStart + settings._iDisplayLength + 1;
                    } else {
                        dataSource.recordsFiltered = settings._iDisplayStart + dataSource.data.length;
                    }
                }
                dataSource.recordsTotal = dataSource.recordsFiltered;

                callback(dataSource);
            })
        }
    }

    private static getColumnFieldName(column: any) {
        return column.name || column.data;
    }

    private stateSaveCallback(externalState?: (stateData: IStateData, simpleStateData: ISimpleStateData) => {data: IStateData, simpleStateData: ISimpleStateData}): (settings: DataTables.SettingsLegacy, data: IStateData) => void {
        const contextThis = this;
        return (settings: DataTables.SettingsLegacy, data: IStateData) => {
            if (contextThis.popped) {
                contextThis.popped = false;
                return;
            }

            var dataToSave: ISimpleStateData = {
                l: data.length,
                c: data.order[0][0],
                d: data.order[0][1],
                p: data.search.search,
                s: data.start
            }
            if (externalState) {
                const returned = externalState(data, dataToSave);
                data = returned.data;
                dataToSave = returned.simpleStateData;
            }
            var queryString = Object.keys(dataToSave).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(dataToSave[key])
            }).join('&');
            history.pushState(data, document.title, `?${queryString}`)
            // XHR HERE TO PUSH STATE TO SESSION FOR BACK FUNCTIONS
        }
    }

    private stateLoadCallback(externalHydration?: (params: URLSearchParams) => void): (settings: DataTables.SettingsLegacy) => IStateData {
        return (settings: DataTables.SettingsLegacy): IStateData => {

            const params = new URLSearchParams(document.location.search)
            
            const l = Number.parseInt(params.get('l'));
            const c = Number.parseInt(params.get('c'));
            const s = Number.parseInt(params.get('s'));
            let data: ISimpleStateData = {
                l: l ? l : undefined,
                c: c ? c : 0,
                d: c ? params.get('d') : 'desc',
                p: params.get('p'),
                s: s ? s : undefined,
            }

            if (externalHydration) {
                externalHydration(params);
            }

            let dataToUse: IStateData = {
                time: new Date().getTime(),
                start: data.s,
                length: data.l,
                search: {
                    search: data.p,
                    smart: true
                },
                order: [
                    [data.c, data.d]
                ]
            }

            return dataToUse;
        };
    }

    private listenForHistoryChange(event: PopStateEvent) {
        var state:IStateData = event.state;

        if(this.settings.externalStateManager) {
            this.settings.externalStateManager(state);
        };

        this.popped = true;
        this.searchField.val(state.search.search)

        this.tableApi.page.len(state.length)
            .page(state.length / state.start)
            .order(state.order)
            .search(state.search.search)
            .draw()

    }
}

