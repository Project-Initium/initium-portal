import * as $ from 'jquery'
import 'gijgo'


export class RoleCreate {
    private tree: Types.Tree;
    private form: HTMLFormElement;
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
        });

        
         this.form = document.querySelector('form#role-create') as HTMLFormElement;
         
         this.form.addEventListener('submit', (e) => { contextThis.formSubmit(e); });        
    }

     private formSubmit(event: Event): void {
         
         
            this.tree.find('input[type=checkbox]:checked').each((index, value) => {
                var checkbox = $(value)
                checkbox.attr('name', 'pagemodel.roles')

                var parentLi = checkbox.closest('li');
                checkbox.val(parentLi.data('id'))
    
            })
         
     }

    private treeOnDataBound(e: Event){
        this.tree.find('input[type=checkbox]').each((index, value) => {
            var checkbox = $(value)
            checkbox.attr('name', 'roles')

        })
    }
}

new RoleCreate();