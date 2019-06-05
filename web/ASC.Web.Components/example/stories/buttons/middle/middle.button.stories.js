import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Button } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

storiesOf('Components|Buttons', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('middle', () => (
    <Section>
      <Button
        size={select('size', sizeOptions, 'middle')}
        primary={boolean('primary', true)}
        isDisabled={boolean('isDisabled', false)}
        onClick={action('clicked')}
      >
        {text('Label', 'Middle button')}
      </Button>
    </Section>
  ));
