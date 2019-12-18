import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import DropDown from '../drop-down';
import DropDownItem from '../drop-down-item';
import DropDownProfileItem from '.';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components | DropDown', module)
  .addDecorator(withReadme(Readme))
  .add('base profile item', () => {

    return (
      <Section>
        <DropDown
          isUserPreview={true}
          withArrow={true}
          directionX='right'
          manualY='1%'
          open={true}>
            <DropDownProfileItem
              avatarRole='admin'
              avatarSource='https://static-www.onlyoffice.com/images/team/developers_photos/personal_44_2x.jpg'
              displayName='Jane Doe'
              email='janedoe@gmail.com' />
          <DropDownItem
            label='Profile'
            onClick={() => console.log('Profile clicked')} />
          <DropDownItem
            label='About'
            onClick={() => console.log('About clicked')} />
          <DropDownItem
            label='Log out'
            onClick={() => console.log('Log out clicked')} />
        </DropDown>
      </Section>
    )
  });