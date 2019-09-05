import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, color, select, date } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { DatePicker } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import moment from 'moment';
import 'moment/min/locales'

function myDateKnob(name, defaultValue) {
  const stringTimestamp = date(name, defaultValue)
  return new Date(stringTimestamp)
}

const locales = moment.locales();

storiesOf('Components|Calendar', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('date-picker', () => (
    <Section>
      <DatePicker
        onChange={date => {
          action('Selected date')(date);
        }}
        selectedDate={myDateKnob('selectedDate', new Date())}
        minDate={myDateKnob('minDate', new Date("1970/01/01"))}
        maxDate={myDateKnob('maxDate', new Date(new Date().getFullYear() + 1 + "/01/01"))}
        isDisabled={boolean("isDisabled", false)}
        isReadOnly={boolean("isReadOnly", false)}
        hasError={boolean("hasError", false)}
        hasWarning={boolean("hasWarning", false)}
        isOpen={boolean('isOpen', false)}
        themeColor={color('themeColor', '#ED7309')}
        locale={select('location', locales, moment.locale())}
      />
    </Section>
  ));
