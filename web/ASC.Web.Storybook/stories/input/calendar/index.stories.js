import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { DateValue } from 'react-values'
import { withKnobs, boolean, text, color, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Calendar } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { object } from '@storybook/addon-knobs/dist/deprecated';




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
                startDate = {new Date()}
                disabled={boolean('disabled', false)}
                themeColor={color('themeColor', '#ED7309')}
                
                
                />
        </Section>
    ));

/*


          const sizeOptions = ['base', 'medium', 'big'];
          size={select('size', sizeOptions, 'big')}

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
*/
