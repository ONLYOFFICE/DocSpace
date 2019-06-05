import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, number, text, select, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Loader } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const typeOptions = ['base', 'oval', 'dual-ring', 'rombs'];

storiesOf('Components|Loaders', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('oval', () => (
    <Section>
      <Loader 
        type={select('type', typeOptions, 'oval')} 
        color={color('color', '#63686a')} 
        size={number('size', 40)}
        label={text('label', "Loading content, please wait.")} />
    </Section>
  ));