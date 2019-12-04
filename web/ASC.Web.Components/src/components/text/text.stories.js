import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, color, number, select } from '@storybook/addon-knobs/react';
import Text from '.';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const textTags = ['p', 'span', 'div'];

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text
          title={text('title', '')}
          as={select('as', textTags , 'p')}
          fontSize={number('fontSize', 13)}
          fontWeight={number('fontWeight')}
          truncate={boolean('truncate', false)}
          color={color('color', '#333333')}
          backgroundColor={color('backgroundColor', '')}
          isBold={boolean('isBold', false)}
          isItalic={boolean('isItalic', false)}
          isInline={boolean('isInline', false)}
          display={text('display')}
        >
          {text('Text', 'Sample text')}
        </Text>
      </div>
    </Section>
  ));