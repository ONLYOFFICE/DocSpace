import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { MainButton, GroupButton, Button } from 'asc-web-components';
import Section from '../../.storybook/decorators/section';

const moduleNames = ['documents', 'people', 'mail', ''];

storiesOf('Components|MainButton', module)
  .addDecorator(withKnobs)
  .add('Main button', () => (
    <Section>
      <MainButton
        isDisable={boolean('isDisable', false)}
        isDropdown={boolean('isDropdown', false)}
        text={text('text', 'Actions')}
        moduleName={select('moduleName', moduleNames, 'people')}
      >
        <GroupButton 
          text='Group button' 
        />
        <Button 
            label='Base button' 
            size='base' 
        />
      </MainButton>
    </Section>
  ));
