import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs } from '@storybook/addon-knobs/react';
import { Label } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

storiesOf('Components|Label', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => {

    return (
      <Section>
        <Label
          isRequired={boolean('isRequired', false)}
          error={boolean('error', false)}
          isInline={boolean('isInline', true)}
          title={text('title', 'Fill the first name field')}
          truncate={boolean('truncate', false)}
          htmlFor={text('htmlFor')}
        >
          {text('Text', 'First name:')}
        </Label>
      </Section>
    )
  });
