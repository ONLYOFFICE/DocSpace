import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Value } from 'react-value';
import { withKnobs, boolean, text, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { DateInput } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('date', () => (
    <Section>
      <Value
        defaultValue="99/99/9999"
        render={(value, onChange) => (
          <DateInput
            id={text('id', '')}
            name={text('name', '')}
            value={text('value', value)}
            placeholder={text('placeholder', 'This is placeholder')}
            onChange={event => {
              action('onChange')(event);
              onChange(event.target.value);
            }}
            onBlur={action('onBlur')}
            onFocus={action('onFocus')}
            isDisabled={boolean('isDisabled', false)}
            isReadOnly={boolean('isReadOnly', false)}
            hasError={boolean('hasError', false)}
            hasWarning={boolean('hasWarning', false)}
            tabIndex={number('tabIndex', 1)}
          />
        )}
      />
    </Section>
  ));
