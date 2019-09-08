import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, text, number, select, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import Loader from '.';
import Section from '../../../.storybook/decorators/section';

const typeOptions = ['base', 'oval', 'dual-ring', 'rombs'];

storiesOf('Components|Loaders', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Loader 
        type={select('type', typeOptions, 'base')} 
        color={color('color', '#63686a')}
        size={number('size', 18)}
        label={text('label', "Loading content, please wait...")} />
    </Section>
  ))
  .add('dual-ring', () => (
    <Section>
      <Loader 
        type={select('type', typeOptions, 'dual-ring')} 
        color={color('color', '#63686a')} 
        size={number('size', 40)}
        label={text('label', "Loading content, please wait.")} />
    </Section>
  ))
  .add('oval', () => (
    <Section>
      <Loader 
        type={select('type', typeOptions, 'oval')} 
        color={color('color', '#63686a')} 
        size={number('size', 40)}
        label={text('label', "Loading content, please wait.")} />
    </Section>
  ))
  .add('rombs', () => (
    <Section>
      <Loader 
        type={select('type', typeOptions, 'rombs')}  
        size={number('size', 40)} />
    </Section>
  ));