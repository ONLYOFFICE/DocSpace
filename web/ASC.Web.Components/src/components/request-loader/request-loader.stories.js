import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, number, text, color } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import RequestLoader from '.';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Loaders', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('request-loader', () => (
    <Section>
      <RequestLoader 
        visible={boolean('visible', true)}
        zIndex={number('zIndex', 256)}
        loaderSize={number('loaderSize', 16)}
        loaderColor={color('loaderColor', '#999')}
        label={text('label', 'Loading... Please wait...')}
        fontSize={number('fontSize', 12)}
        fontColor={color('fontColor', '#999')} />
    </Section>
  ));