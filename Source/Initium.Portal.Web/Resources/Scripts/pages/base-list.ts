import { CustomizedDataTableProvider, ICustomQuery, ISimpleStateData, IStateData } from '../providers';
import * as ko from 'knockout';
import KnockoutSecureBinding from 'knockout-secure-binding';
import { BaseFilterViewModel } from './base-filter-view-model';

export abstract class BaseList<TBaseFilterViewModel extends BaseFilterViewModel> {
    protected detailsUrl: string;
    protected exportUrl: string;
    protected filterVM: TBaseFilterViewModel;
    protected abstract readonly tableOptions: DataTables.Settings;
    protected customizedDataTable: CustomizedDataTableProvider;
    private filterToggleIcon: HTMLSpanElement;
    private searchFacets: HTMLDivElement;

    protected baseInit(tableSelector: string, filterVM: TBaseFilterViewModel) {

        const contextThis = this;
        const $tableElement = $(tableSelector);
        this.detailsUrl = $tableElement[0].dataset.details;
        this.filterVM = filterVM;

        this.customizedDataTable = new CustomizedDataTableProvider($tableElement, {
            route: $tableElement.data('routeFiltered'),
            externalHydration: (params: URLSearchParams) => contextThis.filterVM.hydrateFromParams(params),
            externalState: (stateData: IStateData, simpleStateData: ISimpleStateData) => contextThis.filterVM.generateStateData(stateData, simpleStateData),
            externalStateManager: (userStateData: IStateData) => contextThis.filterVM.hydrateFromState(userStateData),
            externalFilter: () => contextThis.filterVM.getFilter()

        }, this.tableOptions);

        const filterForm = document.querySelector<HTMLFormElement>('#filters');
        filterForm.addEventListener('reset', _ => contextThis.filterVM.reset());
        filterForm.addEventListener('submit', (event) => contextThis.search(event));

        const toggle = filterForm.querySelector('.filter-toggle');
        toggle.addEventListener('click', (event) => contextThis.toggleFilters(event));
        this.filterToggleIcon = toggle.querySelector('i');
        this.searchFacets = filterForm.querySelector('#filter-options');

        const exportBtn: HTMLButtonElement = document.querySelector('[data-export]');
        this.exportUrl = exportBtn.dataset.export;
        exportBtn.addEventListener('click', (event) => contextThis.requestExport(event));

        $tableElement.on('click', 'tbody tr', (event) => contextThis.rowClicked(event));

        ko.bindingProvider.instance = new KnockoutSecureBinding({
            attribute: 'data-bind',
            globals: window,
            bindings: ko.bindingHandlers,
            noVirtualElements: false
        });
        ko.applyBindings(this.filterVM);
    }

    protected rowClicked(event: JQuery.ClickEvent): void {
        window.location.href = this.detailsUrl.replace('__ID__', (this.customizedDataTable.tableApi.row(event.currentTarget).data() as any).Id);
    }

    search(event: Event): any {
        event.preventDefault();
        this.filterVM.createInternalRequest();
        this.customizedDataTable.tableApi.draw();
    }

    toggleFilters(event: Event): void {
        event.preventDefault();
        this.filterToggleIcon.classList.toggle('fa-caret-down');
        this.filterToggleIcon.classList.toggle('fa-caret-up');
        this.searchFacets.classList.toggle('d-none');
    }



    requestExport(event: MouseEvent): void {
        event.preventDefault();
        this.customizedDataTable.generateExport(this.exportUrl, () => this.filterVM.getFilter());
    }
}