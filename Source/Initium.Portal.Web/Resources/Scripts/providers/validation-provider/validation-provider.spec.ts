import { expect } from 'chai';
import {ValidationProvider} from './validation-provider';
import { JSDOM } from 'jsdom';

const dom = new JSDOM(`<!DOCTYPE html><html lang="en"><body></body></html>`);
const document = dom.window.document;

function generateInputGroup(inputId: string, inputType: string, validationType: string, value?: any): HTMLDivElement {
    const group = document.createElement('div');
    group.classList.add('form-group');
    const errorMessage = document.createElement('span');
    errorMessage.setAttribute('data-valmsg-for', inputId);

    const textInput = document.createElement('input');
    textInput.type = inputType;
    textInput.name = inputId;
    textInput.id = inputId;
    textInput.dataset[validationType] = 'some-message';
    if(value)
        textInput.value = value;
    group.appendChild(textInput);
    group.appendChild(errorMessage);

    return group;
}

describe('Validation Helper', () => {
    describe('validate', () => {
        it('when form has validate then result is false', () => {

            const form = document.createElement('form');
            form.dataset.noValidate = '';
            form.appendChild(generateInputGroup('input1', 'text', 'valRequired'));
            const validator: ValidationProvider = new ValidationProvider(form);
            expect(validator.validate().isValid).to.be.true;
        });
        it('when form has validatable elements and elements are valid then result is true', () => {
            const form = document.createElement('form');
            form.appendChild(generateInputGroup('input1', 'text', 'valRequired', 'value'));
            const validator: ValidationProvider = new ValidationProvider(form);
            expect(validator.validate().isValid).to.be.true;
        });
        it('when form has validatable elements and elements are not valid then result is false', () => {
            const form = document.createElement('form');
            form.appendChild(generateInputGroup('input1', 'text', 'valRequired'));
            const validator: ValidationProvider = new ValidationProvider(form);
            expect(validator.validate().isValid).to.be.false;
        });
        it('when form has validatable elements and elements are not valid then element should be decorated', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valRequired');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
            expect(group.querySelector('span[data-valmsg-for]').innerHTML).to.eq('some-message')
        });
        it('when form has validatable elements and elements are valid then element should be decorated', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valRequired', 'val');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-success')).to.be.true;
            expect(group.querySelector('span[data-valmsg-for]').innerHTML).to.eq('')
        });
    });
    describe('reset', () => {
        it('when form has been validate expect validation to be removed', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valRequired');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            validator.reset();
            expect(group.classList.contains('has-error')).to.be.false;
            expect(group.classList.contains('has-success')).to.be.false;
            expect(group.querySelector('span[data-valmsg-for]').innerHTML).to.eq('')
        });
    });
    describe('validation rules', () => {
        it('when valRequired expect a required validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valRequired');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
        it('when valEmail expect an email validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valEmail', 'value');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
        it('when valEmail expect an email validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valEmail', 'value');
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
        it('when valMinlength expect a length validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valMinlength', 'value');
            group.querySelector('input').dataset.valMinlengthMin = '6';
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
        it('when valEqualto expect a equality validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valEqualto', 'value');
            group.querySelector('input').dataset.valEqualtoOther = 'compare';
            const textInput = document.createElement('input');
            textInput.type = 'text';
            textInput.name = 'compare';
            textInput.id = 'compare';
            textInput.value = 'val';
            form.appendChild(group);
            form.appendChild(textInput);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
        it('when valRegex expect a format validation to be used', () => {
            const form = document.createElement('form');
            const group = generateInputGroup('input1', 'text', 'valRegex', 'value');
            group.querySelector('input').dataset.valRegexPattern = '^[0-9]$';
            form.appendChild(group);
            const validator: ValidationProvider = new ValidationProvider(form);
            validator.validate();
            expect(group.classList.contains('has-error')).to.be.true;
        });
    });
});