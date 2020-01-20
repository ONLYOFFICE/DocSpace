import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, text, select} from '@storybook/addon-knobs/react';
import ProfileMenu from '.';
import Section from '../../../.storybook/decorators/section';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';

const roleOptions = ['owner', 'admin','guest','user'];
const defaultAvatar = 'https://static-www.onlyoffice.com/images/team/developers_photos/personal_44_2x.jpg';

storiesOf('Components|ProfileMenu', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))

  .add('base', () => (
    <Section>
      <ProfileMenu
        avatarRole={select('avatarRole', roleOptions, 'admin')}
        avatarSource={text('avatarSource','') || defaultAvatar}
        displayName={text('displayName','') || 'Jane Doe'}
        email={text('email','') || 'janedoe@gmail.com'} 
      />
    </Section>
  ));