import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, color, select, date } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Calendar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
//import moment from 'moment/min/moment-with-locales';
import moment from 'moment';
import 'moment/min/locales'

function myDateKnob(name, defaultValue) {
  const stringTimestamp = date(name, defaultValue)
  return new Date(stringTimestamp)
}

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('calendar', () => (
    <Section>
      <Calendar
        onChange={date => {
          action('Selected date')(date);
        }}
        //dateFormat={text('dateFormat', 'dd.MM.yyyy')}
        disabled={boolean('disabled', false)}
        themeColor={color('themeColor', '#ED7309')}
        selectedDate={myDateKnob('selectedDate', new Date())}
        openToDate={myDateKnob('openToDate', new Date())}
        minDate={myDateKnob('minDate', new Date("2018/02/01"))}
        maxDate={myDateKnob('maxDate', new Date("2019/09/01"))}
        location={select('location', moment.locales(), 'en')}
      />
    </Section>
  ));
