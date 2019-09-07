import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { DateValue } from 'react-values'
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import DateInput from '.';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('date', () => (
    <Section>
      <DateValue>
        {({ value, set }) => (
          <DateInput
            id={text('id', '')}
            name={text('name', '')}  
            disabled={boolean('disabled', false)}
            readOnly={boolean('readOnly', false)}
            selected={value}
            onChange={date => {
              action('onChange')(date);
              set(date);
            }}
            dateFormat={text('dateFormat', 'dd.MM.yyyy')}
            hasError={boolean('hasError', false)}
            hasWarning={boolean('hasWarning', false)}
          />
        )}
      </DateValue>
    </Section>
  ));
