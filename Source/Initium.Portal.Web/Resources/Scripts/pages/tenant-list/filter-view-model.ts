import ko from 'knockout';
import {ICustomQuery, ISimpleStateData, IStateData} from '../../providers';
import {BaseFilterViewModel} from '../base-list';



interface ITenantSimpleStateData extends ISimpleStateData {
}

interface ITenantStateData extends IStateData {
}

export class FilterViewModel extends BaseFilterViewModel {
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

    generateStateData(stateData: ITenantStateData, simpleStateData: ITenantSimpleStateData):
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