import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { BooleanValue } from 'react-values'
import { withKnobs, boolean, text} from '@storybook/addon-knobs/react';
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
        <BooleanValue>
          {({ value, toggle }) => (
            <ToggleButton
              value={text('value', 'value')}
              isChecked={value}
              isDisabled={boolean('isDisabled', false)}
              onChange={e => {
                action('onChange')(e);
                toggle(e.target.checked);
              }}
            />
          )}
        </BooleanValue>
      </Section>
    )
  });
