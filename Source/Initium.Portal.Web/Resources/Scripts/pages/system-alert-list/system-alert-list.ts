import 'datatables.net';
import 'datatables.net-bs4';
import { BaseList } from '../base-list';
import moment from 'moment';
import { FilterViewModel } from './filter-view-model';


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
                render: (data) => {
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
                title: 'When Show'
            },
            {
                data: 'WhenToHide',
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
                title: 'When Hide'
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
        this.baseInit('#system-alerts', new FilterViewModel());
    }
}
// tslint:disable-next-line:no-unused-expression
new SystemAlertList();
