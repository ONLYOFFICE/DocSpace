import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, color, select, date } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { NewCalendar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import moment from 'moment';
import 'moment/min/locales'


function myDateKnob(name, defaultValue) {
  const stringTimestamp = date(name, defaultValue)
  return new Date(stringTimestamp)
}

const locales = moment.locales();

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('new-calendar', () => (
    <Section>
      <NewCalendar
        onChange={date => {
          action('Selected date')(date);
        }}
        disabled={boolean('disabled', false)}
        themeColor={color('themeColor', '#ED7309')}
        selectedDate={myDateKnob('selectedDate', new Date())}
        openToDate={myDateKnob('openToDate', new Date())}
        minDate={myDateKnob('minDate', new Date("1970/01/01"))}
        maxDate={myDateKnob('maxDate', new Date("3000/01/01"))}
        locale={select('location', locales, 'en')}
        scaled = {boolean('scaled', false)}
      />
    </Section>
  ));
