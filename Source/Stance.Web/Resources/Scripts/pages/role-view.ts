import 'gijgo'
import { Validator } from '../services/validator';


export class RoleView {
    private tree: Types.Tree;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    init() {
        const contextThis = this;
        let $tree = $('#tree');
        this.tree = $tree.tree({
            uiLibrary: 'bootstrap4',
            dataSource: $tree.data('route'),
            primaryKey: 'id',
            childrenField: 'simpleResources',
            textField: 'name',
            checkboxes: true,
            dataBound: (e: any) => contextThis.treeOnDataBound(e)
        });
      
    }

     

    private treeOnDataBound(e: Event){
        const contextThis = this;

        const resources = document.querySelectorAll('input[type="hidden"][name="pagemodel.resources"]');
        Array.prototype.forEach.call(resources,
            (element: HTMLInputElement) => {
                const node = contextThis.tree.getNodeById(element.value);
                if (node) {
                    contextThis.tree.check(node);                    
                }
                element.parentElement.removeChild(element);
            });
            this.tree.disableAll()
            this.tree.expandAll()
        

    }
}

new RoleView();