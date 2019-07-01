import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { Text } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const tagHeadline = ['h1','h2','h3'];
const tagBody = ['p', 'span'];
const color = ['black', 'gray', 'lightGray'];
const headlineName = ['moduleName', 'mainTitle']

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('headline', () => (

    <Section>
      <Text.Headline
        tag={select('tag', tagHeadline, 'h1')}
        title={text('title', '')}
        truncate={boolean('truncate', false)}
        isDisabled={boolean('isDisabled', false)}
        isInline={boolean('isInline', false)}
      >
        {text('Text', 'Sample text Headline')}
      </Text.Headline>
    </Section>
  ))
  .add('body', () => (

    <Section>
      <Text.Body
        title={text('title', '')}
        tag = {select('tag', tagBody, 'p')}
        truncate={boolean('truncate', false)}
        isDisabled={boolean('isDisabled', false)}
        color={select('color', color, 'black')}
        backgroundColor={boolean('backgroundColor', false)}
        isBold={boolean('isBold', false)}
        isItalic={boolean('isItalic', false)}
        isInline={boolean('isInline', false)}
      >
        {text('Text', 'Sample text Headline')}
      </Text.Body>
    </Section>
  ))
  .add('main headline', () => (

    <Section>
      <Text.MainHeadline
        headlineName={select('headlineName', headlineName, 'moduleName')}
        title={text('title', '')}
        truncate={boolean('truncate', false)}
        isDisabled={boolean('isDisabled', false)}
        isInline={boolean('isInline', false)}
      >
        {text('Text', 'Sample text Headline')}
      </Text.MainHeadline>
    </Section>
  ));