import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, color, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Calendar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
//import { object } from '@storybook/addon-knobs/dist/deprecated';




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
        startDate={new Date()}
        disabled={boolean('disabled', false)}
        themeColor={color('themeColor', '#ED7309')}
        openToDate={new Date()} //new Date("1993/09/28")
      />
    </Section>
  ));
