import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import { MainButton, DropDownItem, Icons } from 'asc-web-components';
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
  .add('main button', () => {

    let isDropdown=boolean('isDropdown', true);

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
        <DropDownItem label="New employee" onClick={() => action('New employee clicked')} />
        <DropDownItem label="New quest" onClick={() => action('New quest clicked')} />
        <DropDownItem label="New department" onClick={() => action('New department clicked')} />
        <DropDownItem isSeparator />
        <DropDownItem label="Invitation link" onClick={() => action('Invitation link clicked')} />
        <DropDownItem label="Invite again" onClick={() => action('Invite again clicked')} />
        <DropDownItem label="Import people" onClick={() => action('Import people clicked')} />
      </MainButton>
    </Section>
  )});
