import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import SocialButtonWithToggle from '.';
 
storiesOf('Components|Buttons|SocialButtonsWithToggle', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('social button with toggle', () => {
    const socialNetworks = ['ShareGoogleIcon', 
      'ShareFacebookIcon', 
      'ShareTwitterIcon',
      'ShareLinkedInIcon'];
    const iconName = select("iconName", ['', ...socialNetworks], 'ShareFacebookIcon');

    return (
          <SocialButtonWithToggle
            label={text('label', 'Facebook')}
            iconName={iconName}
            isDisabled={boolean('isDisabled', false)}
            isChecked={boolean('isChecked', false)}
            onClick={action('clicked')}  
            onChange={(checked) => { 
              window.__STORYBOOK_ADDONS.channel.emit('storybookjs/knobs/change', {
                name: 'isChecked',
                value: checked
              });
            }}
          />
    )
  });
