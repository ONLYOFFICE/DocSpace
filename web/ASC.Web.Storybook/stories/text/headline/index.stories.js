import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select, color } from '@storybook/addon-knobs/react';
import { Text } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const size = ['big', 'medium', 'small'];

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('headline', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text.Headline
          color={color('color', '#333333')}
          as={text ('as', 'h2')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isDisabled={boolean('isDisabled', false)}
          isInline={boolean('isInline', false)}
          size={select('size', size, 'big')}
        >
          {text('Text', 'Sample text Headline')}
        </Text.Headline>
      </div>
    </Section>
  ));