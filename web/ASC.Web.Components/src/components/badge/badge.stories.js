import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import withReadme from 'storybook-readme/with-readme';
import { withKnobs, number, color, text } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import Section from '../../../.storybook/decorators/section';
import Badge from '.';

storiesOf('Components|Badge', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
      <Badge
        number={number('number', 10)}
        backgroundColor={color('backgroundColor', '#ED7309')}
        color={color('color', '#FFFFFF')}
        fontSize={text('fontSize', '11px')}
        fontWeight={number('fontWeight', 800)}
        borderRadius={text('borderRadius', '11px')}
        padding={text('padding', '0 5px')}
        maxWidth={text('maxWidth', '50px')}
        onClick={(e)=>{
          action('onClick')(e);
        }}
      />
    </Section>
  ));