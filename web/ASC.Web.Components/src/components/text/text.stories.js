import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, color, number, select } from '@storybook/addon-knobs/react';
import { Text } from '.';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const textTags = ['p', 'span', 'div'];
const size = ['big', 'medium', 'small'];

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('body', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text.Body
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
          {text('Text', 'Sample text Headline')}
        </Text.Body>
      </div>
    </Section>
  ))
  .add('content header', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text.ContentHeader
          color={color('color', '#333333')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isInline={boolean('isInline', false)}
        >
          {text('Text', 'Sample text Headline')}
        </Text.ContentHeader>
      </div>
    </Section>
  ))
  .add('headline', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text.Headline
          color={color('color', '#333333')}
          as={select ('as', textTags, 'h2')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isInline={boolean('isInline', false)}
          size={select('size', size, 'big')}
        >
          {text('Text', 'Sample text Headline')}
        </Text.Headline>
      </div>
    </Section>
  ))
  .add('menu header', () => (
    <Section>
      <div style={{ width: "100%" }}>
        <Text.MenuHeader
          color={color('color', '#333333')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isInline={boolean('isInline', false)}
        >
          {text('Text', 'Sample text Headline')}
        </Text.MenuHeader>
      </div>
    </Section>
  ));