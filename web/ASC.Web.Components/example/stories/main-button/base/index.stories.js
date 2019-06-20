import React from 'react';
import { storiesOf } from '@storybook/react';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { MainButton, GroupButton, Button } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const moduleNames = ['documents', 'people', 'mail', ''];

function ClickMainButton(e, credentials) {
  console.log("ClickMainButton", e, credentials);
}

function ClickSecondaryButton(e, credentials) {
  console.log("ClickSecondaryButton", e, credentials);
}

storiesOf('Components|MainButton', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('Main button', () => (
    <Section>
      <MainButton
        isDisabled={boolean('isDisabled', false)}
        isDropdown={boolean('isDropdown', false)}
        text={text('text', 'Actions')}
        moduleName={select('moduleName', moduleNames, 'people')}
        clickAction={ClickMainButton}
        clickActionSecondary={ClickSecondaryButton}
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
