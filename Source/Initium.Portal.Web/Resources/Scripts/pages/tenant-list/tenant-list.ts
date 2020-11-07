import 'datatables.net';
import 'datatables.net-bs4';
import { BaseList } from '../base-list';
import { FilterViewModel } from './filter-view-model';

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
        dom: 'rt<"table-information"lpi>',
        language: {
            processing: '<div class="processing-inner">Processing...</div>'
        }
    };

    constructor() {
        super();
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        this.baseInit('#tenants', new FilterViewModel());
    }
}
// tslint:disable-next-line:no-unused-expression
new TenantList();
