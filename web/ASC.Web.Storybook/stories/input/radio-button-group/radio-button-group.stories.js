import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { RadioButtonGroup } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { withKnobs, text, boolean, number } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import { action } from '@storybook/addon-actions';

const values = ['first', 'second', 'third'];

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('radio-button-group', () => {
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
            name={text('name of your group', 'group')}
            options={
              [
                { value: values[0], label: text('label 1st radiobutton', '1st button'), disabled: boolean('disabled 1st radiobutton', true) },
                { value: values[1], label: text('label 2nd radiobutton', '2nd button'), },
                { value: values[2], label: text('label 3rd radiobutton', '3rd button') },
              ]
            }
          />
        </>
      </Section>
    );
  });
