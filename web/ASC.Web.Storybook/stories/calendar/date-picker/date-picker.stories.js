import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, color, select, date } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { DatePicker } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

function myDateKnob(name, defaultValue) {
  const stringTimestamp = date(name, defaultValue)
  return new Date(stringTimestamp)
}

storiesOf('Components|Calendar', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('date-picker', () => (
    <Section>
      <DatePicker
        selectedDate={myDateKnob('selectedDate', new Date())}
        openToDate={myDateKnob('openToDate', new Date())}
        isDisabled = {boolean("isDisabled", false)}
        isReadOnly = {boolean("isReadOnly", false)}
        hasError = {boolean("hasError", false)}
        hasWarning = {boolean("hasWarning", false)}
        isOpen={boolean('isOpen', false)}
      />
    </Section>
  ));
