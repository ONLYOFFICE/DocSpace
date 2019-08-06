import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { Text } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const color = ['black', 'gray', 'lightGray'];

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('body', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Text.Body
          title={text('title', '')}
          tag={text('tag', 'p')}
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
      </div>
    </Section>
  ));