import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, color, select } from '@storybook/addon-knobs/react';
import Heading from '.';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const textTags = ['p', 'span', 'div'];
const size = ['big', 'medium', 'small'];

storiesOf('Components|Heading', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <div style={{ width: "100%" }}>
        <Heading
          color={color('color', '#333333')}
          as={select('as', textTags, 'h2')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isInline={boolean('isInline', false)}
          size={select('size', size, 'big')}
        >
          {text('Text', 'Sample text Heading')}
        </Heading>
      </div>
    </Section>
  ));