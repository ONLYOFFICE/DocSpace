import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import RadioButton from '.';
import Section from '../../../.storybook/decorators/section';
import { withKnobs, text, boolean } from '@storybook/addon-knobs/react';
import Readme from './README.md';


storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('radio button', () => {
    return (
      <Section>
          <RadioButton
            value={text('value', 'value')}
            name={text('name', 'name')}
            label={text('label', 'Label')}
            fontSize={text('fontSize', '13px')}
            fontWeight={text('fontWeight', '400')}
            isDisabled={boolean('isDisabled', false)}
            isChecked={boolean('isChecked', false)}
            onClick={(e) => {
              window.__STORYBOOK_ADDONS.channel.emit('storybookjs/knobs/change', {
                name: 'isChecked',
                value: e.target.checked
              });
              console.log('onChange', e);
            }
            }
          />
      </Section>
    );
  });
