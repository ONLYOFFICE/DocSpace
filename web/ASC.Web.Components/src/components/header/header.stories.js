import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, color, select } from '@storybook/addon-knobs/react';
import Header from '.';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const type = ['content', 'menu'];

storiesOf('Components|Header', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  
  .add('base', () => (

    <Section>
      <div style={{ width: "100%" }}>
        <Header 
        type={select('type', type, 'content')}
          color={color('color', '#333333')}
          title={text('title', '')}
          truncate={boolean('truncate', false)}
          isInline={boolean('isInline', false)}
        >
          {text('Text', 'Sample text Header')}
        </Header>
      </div>
    </Section>
  ));