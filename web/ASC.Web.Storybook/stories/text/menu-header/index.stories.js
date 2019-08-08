import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, color } from '@storybook/addon-knobs/react';
import { Text } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
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