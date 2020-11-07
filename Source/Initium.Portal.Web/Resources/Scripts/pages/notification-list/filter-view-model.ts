import ko from 'knockout';
import { ICustomQuery, ISimpleStateData, IStateData } from '../../providers';
import { BaseFilterViewModel } from '../base-list';


export class FilterViewModel extends BaseFilterViewModel {
    public searchTerm = ko.observable<string>('');
    private customQuery: ICustomQuery;


    constructor() {
        super();
    }

    createInternalRequest() {
        this.customQuery = {
            requestData: null,
            searchParam: this.searchTerm()
        };
    }

    generateStateData(stateData: IStateData, simpleStateData: ISimpleStateData):
        { stateData: IStateData; simpleStateData: ISimpleStateData } {
        stateData.search = this.searchTerm();
        return {stateData, simpleStateData};
    }

    getFilter(): ICustomQuery {
        return {
            searchParam: '',
            requestData: {
            }
        };
    }

    // tslint:disable-next-line:no-empty
    hydrateFromParams(params: URLSearchParams) {
    }

    hydrateFromState(userStateData: IStateData) {
        this.searchTerm(userStateData.search);
    }

    reset() {
        return;
    }
}