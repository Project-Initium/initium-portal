

interface JQuery {
    stick_in_parent(args: any): JQuery;
}

declare namespace DataTables {
    interface Api {
        processing(state: boolean)
    }
}