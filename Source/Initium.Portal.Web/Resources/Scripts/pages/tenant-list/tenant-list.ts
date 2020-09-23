import 'datatables.net'
import 'datatables.net-bs4'
import {BaseList} from '../base-list';
import moment from 'moment';
import {FilterViewModel} from './filter-view-model';


export class TenantList extends BaseList<FilterViewModel>{
    protected readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'Identifier',
                type: 'string',
                title: 'Identifier'
            },
            {
                data: 'Name',
                type: 'string',
                title: 'Name'
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
        this.baseInit('#tenants', new FilterViewModel());
    }
}
const p = new TenantList();
