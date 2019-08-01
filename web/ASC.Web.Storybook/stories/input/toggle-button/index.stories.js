import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import { action } from '@storybook/addon-actions';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { ToggleButton } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('toggle button', () => {
    return (
      <Section>
        <ToggleButton
          isChecked={boolean('isChecked', false)}
          isDisabled={boolean('isDisabled', false)}
          label={text('label', 'label text')}
          onChange={(e) => {
            window.__STORYBOOK_ADDONS.channel.emit('storybookjs/knobs/change', {
              name: 'isChecked',
              value: e.target.checked
            });
            action('onChange')(e);
          }}
        />
      </Section>
    )
  });