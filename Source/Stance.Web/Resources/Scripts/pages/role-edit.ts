import 'gijgo'
import { Validator } from '../services/validator';


export class RoleEdit {
    private tree: Types.Tree;
    private form: HTMLFormElement;
    private validator: Validator;
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

        
         this.form = document.querySelector('form#role-create') as HTMLFormElement;
         this.validator = new Validator(this.form, false)
                 
         this.form.addEventListener('submit', (e) => { contextThis.formSubmit(e); });        
    }

     private formSubmit(event: Event): void {
         
        if (!this.validator.validate()) {
            this.tree.find('input[type=checkbox]:checked').each((index, value) => {
                var checkbox = $(value)
                checkbox.attr('name', 'pagemodel.resources')

                var parentLi = checkbox.closest('li');
                checkbox.val(parentLi.data('id'))
    
            })
        } else {
            event.preventDefault();
         }
     }

    private treeOnDataBound(e: Event){
        const contextThis = this;

        var resources = document.querySelectorAll('input[type="hidden"][name="pagemodel.resources"]');
        Array.prototype.forEach.call(resources,
            (element: HTMLInputElement) => {
                const node = contextThis.tree.getNodeById(element.value);
                if (node) {
                    contextThis.tree.check(node);                    
                }
                element.parentElement.removeChild(element);
            });

        

    }
}

new RoleEdit();