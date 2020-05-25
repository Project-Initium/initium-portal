import 'datatables.net'
import 'datatables.net-bs4'
import {ICustomQuery, ISimpleStateData, IStateData} from "../services/customized-data-table";
import {BaseFilterViewModel, BaseList} from "./base-list";
import * as ko from "knockout";
import * as moment from "moment";

interface ISystemAlertSimpleStateData extends ISimpleStateData {
}

interface ISystemAlertStateData extends IStateData {
}

class FilterViewModel extends BaseFilterViewModel {
    public searchTerm = ko.observable<string>('');
    private customQuery: ICustomQuery;


    constructor() {
        super()
    }

    createInternalRequest() {
        this.customQuery = {
            requestData: null,
            searchParam: this.searchTerm()
        }
    }

    generateStateData(stateData: ISystemAlertStateData, simpleStateData: ISystemAlertSimpleStateData):
        { stateData: IStateData; simpleStateData: ISimpleStateData } {
        stateData.search = this.searchTerm();
        return {stateData: stateData, simpleStateData: simpleStateData}
    }

    getFilter(): ICustomQuery {
        return {
            searchParam: '',
            requestData: {
            }
        }
    }

    hydrateFromParams(params: URLSearchParams) {
    }

    hydrateFromState(userStateData: IStateData) {
        this.searchTerm(userStateData.search);
    }

    reset() {
        return;
    }
}

export class SystemAlertList extends BaseList<FilterViewModel>{
    protected readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'Name',
                type: 'string',
                title: 'Name'
            },
            {
                data: 'Type',
                type: 'enum',
                title: 'Type'
            },
            {
                data: 'IsActive',
                type: 'boolean',
                render: function (data, type, full, meta) {
                    
                    if (data === true) {
                        return '<i class="fa fa-check" aria-hidden="true"></i>';
                    }
                    return '<i class="fa fa-times" aria-hidden="true"></i>';
                },
                title: 'Is Active'
            },
            {
                data: 'WhenToShow',
                type: 'date',
                render: function (data, type, full, meta) {
                    const m = moment(data);
                    if(m.isValid)
                    {
                        const parsed = m.format('DD/MM/YYYY HH:mm');
                        if (parsed !== 'Invalid date')
                        {
                            return parsed;
                        }
                    }
                    return '';
                },
                title: 'When Show'
            },
            {
                data: 'WhenToHide',
                type: 'date',
                render: function (data, type, full, meta) {
                    const m = moment(data);
                    if(m.isValid)
                    {
                        const parsed = m.format('DD/MM/YYYY HH:mm');
                        if (parsed !== 'Invalid date')
                        {
                            return parsed;
                        }
                    }
                    return '';
                },
                title: 'When Hide'
            },
            {
                data: 'Id',
                type: 'uuid',
                visible: false
            },
        ],
        dom: 'rt<"table-information"lpi>'
    };

    constructor() {
        super();
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        this.baseInit('#system-alerts', new FilterViewModel());
    }
}
new SystemAlertList();
