import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import { Text } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
//import withReadme from 'storybook-readme/with-readme';
//import Readme from './README.md';

const elementType = ['h1','h2','h3','p'];
const styleType = ['default','grayBackground','metaInfo','disabled'];


storiesOf('Components|Text', module)
  .addDecorator(withKnobs)
  .add('Text', () => {

    return (
    <Section>
      <Text
       elementType={select('elementType', elementType, 'p')}
       styleType={select('styleType', styleType, 'default')}
       title={text('title', '')}
       truncate={boolean('truncate', false)}
       text={text('text', 'Example')}
        >
          
      </Text>
    </Section>
  )});
