export interface IODataRequest {
    $select?: string;
    $skip?: number;
    $top?: number;
    $count?: boolean;
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
    p?: string
}
export interface ICustomQuery {
    actionUrl: string;
    requestData: any
    method?: string;
}

export interface ISettings  {
    route: string;
    externalFilter?: (filterQuery: IODataRequest) => ICustomQuery;
    externalState?: (stateData: IStateData, simpleStateData: ISimpleStateData) => {stateData: IStateData, simpleStateData: ISimpleStateData};
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

    private generateSelection(settings: DataTables.SettingsLegacy, excludeNonVisible: boolean = false) :string {
        let select = '';
        settings.aoColumns.forEach(value => {
            if (excludeNonVisible && !value.bVisible) {
                return;
            }
            const fieldName = CustomizedDataTable.getColumnFieldName(value);
            if (!fieldName) {
                return;
            }
            if (!select) {
                select = fieldName;
            } else {
                select += ',' + fieldName;
            }
        });
        
        return select;
    }
    
    private generateFilters(settings: DataTables.SettingsLegacy, searchValue: string): string {
        let filters: string[] = [];
        let globalFilter : string = undefined;
        let isGlobalFilterNumber: boolean = false;
        let globalFilterNumber: number = undefined;
        
        globalFilter = searchValue.trim();
        globalFilterNumber = Number(globalFilter);
        isGlobalFilterNumber = !isNaN(globalFilterNumber)
        

        settings.aoColumns.forEach((value) => {
            const fieldName:string = CustomizedDataTable.getColumnFieldName(value);
            const columnType:string = value.sType || (<any>value).type || 'unknown';

            if ((!globalFilter) || !value.bSearchable || !fieldName) {
                return;
            }

            switch (columnType) {
                case 'string':
                    if (globalFilter) {
                        filters.push(`indexof(tolower(${fieldName}), '${globalFilter.toLowerCase()}') gt -1`);
                    }
                    break;
                case "number":
                    if (isGlobalFilterNumber) {
                        filters.push(`(${fieldName} eq ${globalFilterNumber})`);
                    }
                    break;
                default:
            }
        });

        if (filters.length > 0) {
            return filters.join(' or ');
        }
        
        return undefined;
    }

     
    public generateExport(exportUrl: string, externalFilter?: (filterQuery: IODataRequest) => ICustomQuery)  {
        const request: IODataRequest = {};
        let settings:DataTables.SettingsLegacy = (this.tableApi.columns().data() as any).context[0]
        request.$select = this.generateSelection(settings, true);
        
        debugger;
        let searchValue = this.tableApi.search()
        if (searchValue) {
            const filter =  this.generateFilters(settings, searchValue);
            if(filter) {
                request.$filter = filter;
            }
        }

        let orderBy: string[] = [];
        this.tableApi.order().forEach((value) =>{
            orderBy.push(`${CustomizedDataTable.getColumnFieldName(settings.aoColumns[value[0]])} ${value[1]}`);
        })

        if (orderBy.length > 0) {
            request.$orderby = orderBy.join();
        }

        let fetchRequest: RequestInit = {
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            method: 'GET',
        }
        let getUrl = new URL(exportUrl, document.location.origin);
        if (externalFilter) {
            const customQuery = externalFilter(request);
            if (customQuery.method) {
                fetchRequest.method = customQuery.method;
            }
            getUrl = new URL(customQuery.actionUrl, document.location.origin);
            fetchRequest.body = JSON.stringify(customQuery.requestData);
        }


        Object.keys(request).forEach(key => getUrl.searchParams.append(key, request[key]))
        fetch(getUrl.toString(), fetchRequest)
            .then((response) => {
                return response.blob()
            })
            .then((value: Blob) => {
                const url = window.URL.createObjectURL(value);
                const a = document.createElement('a');
                a.href = url;
                a.download =  "report.csv";
                document.body.appendChild(a);
                a.click();
                a.remove();
            })
    }
    
    private providerFunction(url: string, externalFilter?: ((filterQuery: IODataRequest) => ICustomQuery)): (data: object, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => void {
        return (data: any, callback: ((data: any) => void), settings: DataTables.SettingsLegacy) => {
             const request: IODataRequest = {
                $count: false
            };

            request.$select = this.generateSelection(settings);

            request.$skip = settings._iDisplayStart;
            if (settings._iDisplayLength > -1) {
                request.$top = settings._iDisplayLength;
            }
            request.$count = true;

            if (data.search.value) {
                const filter = this.generateFilters(settings, data.search.value);
                if(filter) {
                    request.$filter = filter;
                }
            }

            let orderBy: string[] = [];
            data.order.forEach((value) =>{
                orderBy.push(`${CustomizedDataTable.getColumnFieldName(settings.aoColumns[value.column])} ${value.dir}`);
            })            

            if (orderBy.length > 0) {
                request.$orderby = orderBy.join();
            }

            let fetchRequest: RequestInit = {
                mode: 'same-origin',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json'
                },
                redirect: 'follow',
                referrerPolicy: 'no-referrer',
                method: 'GET',
            }
            let getUrl = new URL(url, document.location.origin);
            if (externalFilter) {
                const customQuery = externalFilter(request);
                if (customQuery.method) {
                    fetchRequest.method = customQuery.method;
                }
                getUrl = new URL(customQuery.actionUrl, document.location.origin);
                fetchRequest.body = JSON.stringify(customQuery.requestData);
            } 
            
            
            Object.keys(request).forEach(key => getUrl.searchParams.append(key, request[key]))
            fetch(getUrl.toString(), fetchRequest)
                .then((response) => {
                    return response.json()
                }).then((data: any) => {
                const dataSource: any = {
                    draw: parseInt(data.draw) // Cast for security reason
                };

                dataSource.data = data.value;
                const recordCount = data['@odata.count'];

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

    private stateSaveCallback(externalState?: (stateData: IStateData, simpleStateData: ISimpleStateData) => {stateData: IStateData, simpleStateData: ISimpleStateData}): (settings: DataTables.SettingsLegacy, data: IStateData) => void {
        const contextThis = this;
        return (settings: DataTables.SettingsLegacy, data: IStateData) => {
            if (contextThis.popped) {
                contextThis.popped = false;
                return;
            }

            let dataToSave: ISimpleStateData = {
                l: data.length,
                c: data.order[0][0],
                d: data.order[0][1],                
                s: data.start
            }
            if (data.search.search && data.search.search !== 'null') {
                dataToSave.p = data.search.search;
            }
            if (externalState) {
                const returned = externalState(data, dataToSave);
                data = returned.stateData;
                dataToSave = returned.simpleStateData;
            }
            const queryString = Object.keys(dataToSave).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(dataToSave[key])
            }).join('&');
            history.pushState(data, document.title, `?${queryString}`)
            // XHR HERE TO PUSH STATE TO SESSION FOR BACK FUNCTIONS
        }
    }

    private stateLoadCallback(externalHydration?: (params: URLSearchParams) => void): (settings: DataTables.SettingsLegacy) => IStateData {
        return (): IStateData => {

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
            return {
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
            };
        };
    }

    private listenForHistoryChange(event: PopStateEvent) {
        const state:IStateData = event.state;

        if(this.settings.externalStateManager) {
            this.settings.externalStateManager(state);
        }

        this.popped = true;
        this.searchField.val(state.search.search)

        this.tableApi.page.len(state.length)
            .page(state.length / state.start)
            .order(state.order)
            .search(state.search.search)
            .draw()

    }
}

