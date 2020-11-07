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
    search: string
}
export interface ISimpleStateData {
    s: number;
    l: number;
    c: number;
    d: string;
    p?: string
}
export interface ICustomQuery {
    requestData: any
    searchParam: string;
}

export interface ISettings  {
    route: string;
    externalFilter: (filterQuery: IODataRequest) => ICustomQuery;
    externalState: (stateData: IStateData, simpleStateData: ISimpleStateData) => {stateData: IStateData, simpleStateData: ISimpleStateData};
    externalHydration: (params: URLSearchParams) => void;
    externalStateManager: (state:IStateData) => void;
}

export class CustomizedDataTableProvider {
    public readonly tableApi: DataTables.Api;
    private popped: boolean = false;
    public table: HTMLTableElement;

    constructor(tableElement: JQuery<HTMLElement>, private settings: ISettings, opts: DataTables.Settings) {
        const contextThis = this;
        opts.ajax = this.providerFunction(this.settings.route, this.settings.externalFilter);
        opts.stateSaveCallback = this.stateSaveCallback(this.settings.externalState);
        opts.stateLoadCallback = this.stateLoadCallback(this.settings.externalHydration);
        opts.stateSave = true;
        opts.processing = true;
        opts.serverSide = true;
        this.tableApi = tableElement.DataTable(opts);
        this.table = tableElement[0] as HTMLTableElement;
        window.addEventListener('popstate', (event) => contextThis.listenForHistoryChange(event));
    }

    private generateSelection(settings: DataTables.SettingsLegacy, excludeNonVisible: boolean = false) :string {
        let select = '';
        settings.aoColumns.forEach(value => {
            if (excludeNonVisible && !value.bVisible) {
                return;
            }
            const fieldName = CustomizedDataTableProvider.getColumnFieldName(value);
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

    private generateSelectionMapping(settings: DataTables.SettingsLegacy, excludeNonVisible: boolean = false) {
        const select = {
            
        };
        settings.aoColumns.forEach(value => {
            if (excludeNonVisible && !value.bVisible) {
                return;
            }
            const fieldName = CustomizedDataTableProvider.getColumnTypeInformation(value);
            if (!fieldName) {
                return;
            }
            
            select[fieldName.fieldName] = fieldName.fieldTitle;
        });

        return select;
    }

    private generateFilters(settings: DataTables.SettingsLegacy, searchValue: string): string {
        const filters: string[] = [];
        let globalFilter: string;
        let isGlobalFilterNumber: boolean = false;
        let globalFilterNumber: number;

        globalFilter = searchValue.trim();
        globalFilterNumber = Number(globalFilter);
        isGlobalFilterNumber = !isNaN(globalFilterNumber);

        settings.aoColumns.forEach((value) => {
            const fieldName:string = CustomizedDataTableProvider.getColumnFieldName(value);
            const columnType:string = value.sType || (value as any).type || 'unknown';

            if ((!globalFilter) || !value.bSearchable || !fieldName) {
                return;
            }

            switch (columnType) {
                case 'string':
                    if (globalFilter) {
                        filters.push(`indexof(tolower(${fieldName}), '${globalFilter.toLowerCase()}') gt -1`);
                    }
                    break;
                case 'number':
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
        const settings:DataTables.SettingsLegacy = (this.tableApi.columns().data() as any).context[0];
        request.$select = this.generateSelection(settings, true);

        const customQuery = externalFilter(request);
        const searchValue = customQuery.searchParam;
        if (searchValue) {
            const filter =  this.generateFilters(settings, searchValue);
            if(filter) {
                request.$filter = filter;
            }
        }

        const orderBy: string[] = [];
        this.tableApi.order().forEach((value) =>{
            orderBy.push(`${CustomizedDataTableProvider.getColumnFieldName(settings.aoColumns[value[0]])} ${value[1]}`);
        });


        if (orderBy.length > 0) {
            request.$orderby = orderBy.join();
        }

        const fetchRequest: RequestInit = {
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            method: 'POST',
        };

        const getUrl = new URL(exportUrl, document.location.origin);
        fetchRequest.body = JSON.stringify( {
            filter: customQuery.requestData,
            mappings: this.generateSelectionMapping(settings, true)
        });

        Object.keys(request).forEach(key => getUrl.searchParams.append(key, request[key]));
        fetch(getUrl.toString(), fetchRequest)
            .then((response) => {
                return response.blob()
            })
            .then((value: Blob) => {
                const url = window.URL.createObjectURL(value);
                const a = document.createElement('a');
                a.href = url;
                a.download =  'report.csv';
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

            const customQuery = externalFilter(request);

            request.$select = this.generateSelection(settings);

            request.$skip = settings._iDisplayStart;
            if (settings._iDisplayLength > -1) {
                request.$top = settings._iDisplayLength;
            }
            request.$count = true;

            if (customQuery.searchParam) {
                const filter = this.generateFilters(settings, customQuery.searchParam);
                if(filter) {
                    request.$filter = filter;
                }
            }

            const orderBy: string[] = [];
            data.order.forEach((value) =>{
                orderBy.push(`${CustomizedDataTableProvider.getColumnFieldName(settings.aoColumns[value.column])} ${value.dir}`);
            });

            if (orderBy.length > 0) {
                request.$orderby = orderBy.join();
            }

            const fetchRequest: RequestInit = {
                mode: 'same-origin',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json'
                },
                redirect: 'follow',
                referrerPolicy: 'no-referrer',
                method: 'Post',
            };
            const getUrl = new URL(url, document.location.origin);
                fetchRequest.body = JSON.stringify(customQuery.requestData);



            Object.keys(request).forEach(key => getUrl.searchParams.append(key, request[key]));
            fetch(getUrl.toString(), fetchRequest)
                .then((response) => {
                    return response.json()
                }).then((data: any) => {
                const dataSource: any = {
                    draw: parseInt(data.draw, 10) // Cast for security reason
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

    private static getColumnFieldName(column: DataTables.ColumnLegacy): string {
        return column.mData;
    }

    private static getColumnTypeInformation(column: DataTables.ColumnLegacy): { fieldName: string, fieldTitle: string } {
        return {
            fieldName: column.mData,
            fieldTitle: column.sTitle
        };
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
            };

            if (externalState) {
                const returned = externalState(data, dataToSave);
                data = returned.stateData;
                dataToSave = returned.simpleStateData;
            }

            if (data.search) {
                dataToSave.p = data.search;
            }

            const queryString = Object.keys(dataToSave).map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(dataToSave[key])
            }).join('&');
            history.pushState(data, document.title, `?${queryString}`)
            // XHR HERE TO PUSH STATE TO SESSION FOR BACK FUNCTIONS
        }
    }

    private stateLoadCallback(externalHydration?: (params: URLSearchParams) => void): (settings: DataTables.SettingsLegacy) => IStateData {
        return (): any => {

            const params = new URLSearchParams(document.location.search);

            const l = Number.parseInt(params.get('l'), 10);
            const c = Number.parseInt(params.get('c'), 10);
            const s = Number.parseInt(params.get('s'), 10);
            const data: ISimpleStateData = {
                l: l ? l : undefined,
                c: c ? c : 0,
                d: c ? params.get('d') : 'desc',
                p: params.get('p'),
                s: s ? s : undefined,
            };

            if (externalHydration) {
                externalHydration(params);
            }
            return {
                time: new Date().getTime(),
                start: data.s,
                length: data.l,
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

        this.tableApi.page.len(state.length)
            .page(state.length / state.start)
            .order(state.order)
            .draw()

    }
}
