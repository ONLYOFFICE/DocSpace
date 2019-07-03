import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import withReadme from 'storybook-readme/with-readme';
import { withKnobs, number, color, text } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import Section from '../../.storybook/decorators/section';
import {Badge} from 'asc-web-components';

storiesOf('Components|Badge', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('badge', () => (
    <Section>
      <Badge
        number={number('number', 10)}
        backgroundColor={color('backgroundColor', '#ED7309')}
        color={color('color', '#FFFFFF')}
        fontSize={text('fontSize', '11px')}
        fontWeight={number('fontWeight', 800)}
        onClick={(e)=>{
          action('onClick')(e);
        }}
      />
    </Section>
  ));