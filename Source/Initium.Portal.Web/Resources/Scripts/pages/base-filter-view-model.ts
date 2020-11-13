import { ICustomQuery, ISimpleStateData, IStateData } from '../providers';

export abstract class BaseFilterViewModel {
    abstract getFilter(): ICustomQuery;

    abstract hydrateFromParams(params: URLSearchParams);

    abstract generateStateData(stateData: IStateData, simpleStateData: ISimpleStateData)
        : { stateData: IStateData, simpleStateData: ISimpleStateData };

    abstract hydrateFromState(userStateData: IStateData);

    abstract reset();

    abstract createInternalRequest();
}