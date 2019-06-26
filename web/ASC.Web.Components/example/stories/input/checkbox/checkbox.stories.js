import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Value } from 'react-value';
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Checkbox } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('checkbox', () => (
    <Section>
      <Value
        defaultValue={true}
        render={(isChecked, onChange) => (
          <Checkbox
            id={text('id', 'id')}
            name={text('name', 'name')}
            value={text('value', 'value')}
            label={text('label', 'label')}
            isChecked={boolean('isChecked', isChecked)}
            isIndeterminate={boolean('isIndeterminate', false)}
            isDisabled={boolean('isDisabled', false)}
            onChange={event => {
              action('onChange')(event);
              onChange(event.target.checked);
            }}
          />
        )}
      />
    </Section>
  ));
