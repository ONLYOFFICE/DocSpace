import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Button } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'big'];

storiesOf('Components|Buttons', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Button 
        label={text('label', 'Base button')}
        primary={boolean('primary', true)}
        size={select('size', sizeOptions, 'big')}

        isHovered={boolean('isHovered', false)}
        isClicked={boolean('isClicked', false)}
        isDisabled={boolean('isDisabled', false)}
        isLoading={boolean('isLoading', false)}

        onClick={action('clicked')}
      />
    </Section>
  ));
