import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { text, boolean, withKnobs } from '@storybook/addon-knobs/react';
import { FieldContainer, TextInput } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

storiesOf('Components|FieldContainer', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <StringValue
      onChange={e => { 
        action('onChange')(e);
      }}
    >
      {({ value, set }) => (
      <Section>
        <FieldContainer
          isRequired={boolean('isRequired', false)}
          hasError={boolean('hasError', false)}
          labelText={text('labelText', 'Name:')}
        >
          <TextInput
            value={value}
            hasError={boolean('hasError', false)}
            className="field-input"
            onChange={e => { 
              set(e.target.value);
            }}
          />
        </FieldContainer>
      </Section>
      )}
   </StringValue>
));
