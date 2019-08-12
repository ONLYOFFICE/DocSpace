import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, color, select, date  } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Calendar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

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
          action('onChange')(date);
        }}
        dateFormat={text('dateFormat', 'dd.MM.yyyy')}
        startDate={myDateKnob('startDate', new Date())}
        disabled={boolean('disabled', false)}
        themeColor={color('themeColor', '#ED7309')}
        openToDate={myDateKnob('openToDate', new Date())}
        minDate = {myDateKnob('minDate', new Date("2018/01/01"))}
        maxDate = {myDateKnob('maxDate', new Date("2020/01/01"))}
      />
    </Section>
  ));
