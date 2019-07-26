import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import { RadioButtonGroup } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { withKnobs, text, boolean, number } from '@storybook/addon-knobs/react';
import Readme from './README.md';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('radio-button-group', () => {
    return (
      <Section>
        <>
          <RadioButtonGroup
            isDisabledGroup={boolean('isDisabledGroup', false)}
            selected={text('selected radio button(value)', 'second')}
            spaceBtwnElems={number('spaceBtwnElems', 33)}
            name={text('name of your group', 'group')}
            options={
              [
                { value: text('value 1st radiobutton', 'first'), label: text('label 1st radiobutton', 'First radiobtn'), disabled: boolean('isDisabled 1st radiobutton', false) },
                { value: text('value 2nd radiobutton', 'second'), label: text('label 2nd radiobutton', 'Second radiobtn'), disabled: boolean('isDisabled 2nd radiobutton', true) },
                { value: text('value 3rd radiobutton', 'third'), label: text('label 3rd radiobutton', '3nd radiobtn'), disabled: boolean('isDisabled 3rd radiobutton', false) },
                { value: text('value 4th radiobutton', 'fourth'), label: text('label 4th radiobutton', '4th radiobtn'), disabled: boolean('isDisabled 4th radiobutton', true) },

              ]
            }
          />
        </>
      </Section>
    );
  });
