import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Button } from 'asc-web-components';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

storiesOf('Components|Button', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('huge', () => (
    <Button
      size={select('size', sizeOptions, 'huge')}
      primary={boolean('primary', true)}
      isDisabled={boolean('isDisabled', false)}
      onClick={action('clicked')}
    >
      {text('Label', 'Huge button')}
    </Button>
  ));
