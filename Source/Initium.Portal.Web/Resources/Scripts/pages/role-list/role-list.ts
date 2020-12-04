import 'datatables.net';
import 'datatables.net-bs4';
import { BaseList } from '../base-list';
import { FilterViewModel } from './filter-view-model';

export class RoleList extends BaseList<FilterViewModel> {
    constructor() {
        super();
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        this.baseInit('#roles', new FilterViewModel());
    }

    protected readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'Name',
                type: 'string',
                title: 'Name',
            },
            {
                data: 'ResourceCount',
                type: 'number',
                title: 'Resource Count'
            },
            {
                data: 'UserCount',
                type: 'number',
                title: 'User Count'
            },
            {
                data: 'Id',
                type: 'uuid',
                visible: false
            }
        ],
        dom: 'rt<"table-information"lpi>',
        language: {
            processing: '<div class="processing-inner">Processing...</div>'
        }
    };

}
// tslint:disable-next-line:no-unused-expression
new RoleList();
