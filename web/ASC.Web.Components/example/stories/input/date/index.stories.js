import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { DateValue } from 'react-values'
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { DateInput } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withReadme(Readme))
  .add('date', () => (
    <Section>
      <DateValue>
        {({ value, set }) => (
          <DateInput
            selected={value}
            dateFormat="dd.MM.yyyy"
            onChange={date => {
              action('onChange')(date);
              set(date);
            }}
          />
        )}
      </DateValue>
    </Section>
  ));
