import 'datatables.net';
import 'datatables.net-bs4';
import 'gijgo';
import moment from 'moment';
import { BaseList } from '../base-list';
import { FilterViewModel } from './filter-view-model';

export class UsersList extends BaseList<FilterViewModel>{
    protected readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'EmailAddress',
                type: 'string',
                title: 'Email Address'
            },
            {
                data: 'FirstName',
                type: 'string',
                title: 'First Name'
            },
            {
                data: 'LastName',
                type: 'string',
                title: 'Last Name',
            },
            {
                data: 'IsLocked',
                type: 'boolean',
                render: (data) => {
                    if (data === true) {
                        return '<i class="fa fa-check" aria-hidden="true"></i>';
                    }
                    return '<i class="fa fa-times" aria-hidden="true"></i>';
                },
                title: 'Locked'
            },
            {
                data: 'WhenLastAuthenticated',
                type: 'date',
                render: (data) => {
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
                title: 'Last Login'
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
        this.baseInit('#users', new FilterViewModel());
    }
}

// tslint:disable-next-line:no-unused-expression
new UsersList();
