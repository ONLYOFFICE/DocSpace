import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { MainButton, Button, Icons } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const iconNames = Object.keys(Icons);

function ClickMainButton(e, credentials) {
  console.log("ClickMainButton", e, credentials);
}

function ClickSecondaryButton(e, credentials) {
  console.log("ClickSecondaryButton", e, credentials);
}

storiesOf('Components|MainButton', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('Main button', () => {

    let isDropdown=boolean('isDropdown', false);

    let icon = !isDropdown ? {iconName: `${select('iconName', iconNames, 'PeopleIcon')}`} : {};

    return (
    <Section>
      <MainButton
        isDisabled={boolean('isDisabled', false)}
        isDropdown={isDropdown}
        text={text('text', 'Actions')}
        
        clickAction={ClickMainButton}
        clickActionSecondary={ClickSecondaryButton}
        {...icon}
      >
        
        <Button 
            label='Base button' 
            size='base' 
        />
        <Button 
            label='Base button' 
            size='base' 
        />
        <Button 
            label='Base button' 
            size='base' 
        />
      </MainButton>
    </Section>
  )});
