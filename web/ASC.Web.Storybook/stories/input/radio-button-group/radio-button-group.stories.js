import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { RadioButtonGroup } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { withKnobs, text, boolean, number } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import { action } from '@storybook/addon-actions';
import { optionsKnob as options } from '@storybook/addon-knobs';


storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('radio button group', () => {

    const values = ['first', 'second', 'third'];
    const valuesMultiSelect = {
      radio1: 'radio1',
      radio2: 'radio2',
      radio3: 'radio3'
    };

    const optionsMultiSelect = options('options', valuesMultiSelect, ['radio1','radio2'], {
      display: 'multi-select',
    });

    let children = [];
    optionsMultiSelect.forEach(function (item, i) {
      switch (item) {
        case 'radio1':
          children.push({ value: values[0], label: text('label 1', 'First radiobtn') });
          break;
        case 'radio2':
          children.push({ value: values[1], label: text('label 2', 'Second radiobtn') });
          break;
        case 'radio3':
          children.push({ value: values[2], label: text('label 3', 'Third radiobtn') });
          break;
        default:
          break;
      }
    });

    return (
      <Section>
        <>
          <RadioButtonGroup
            onClick={e => {
              action('onChange')(e);
              console.log('Value of selected radiobutton: ', e.target.value);
            }}
            isDisabled={boolean('isDisabled', false)}
            selected={values[0]}
            spacing={number('spacing', 33)}
            name={text('name', 'group')}
            options={children}
          />
        </>
      </Section>
    );
  });
