import React from 'react';
import { storiesOf } from '@storybook/react';
import { StringValue } from 'react-values';
import { text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import EmailInput from '.';
import Section from '../../../.storybook/decorators/section';
import { action } from '@storybook/addon-actions';

const sizeInput = ['base', 'middle', 'big', 'huge'];

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('email input', () => {

    return (
      <Section>
        <StringValue>
          {({ value, set }) => (
            <EmailInput
              placeholder={text('placeholder', 'Input email')}
              size={select('size', sizeInput, 'base')}
              scale={boolean('scale', false)}
              isDisabled={boolean('isDisabled', false)}
              isReadOnly={boolean('isReadOnly', false)}
              maxLength={number('maxLength', 255)}
              id={text('id', 'emailId')}
              name={text('name', 'demoEmailInput')}
              value={value}
              onChange={e => {
                set(e.target.value);
              }}
              onValidateInput={(isEmailValid) => action('validateEmail')(isEmailValid)}
            />
          )}
        </StringValue>
      </Section>
    )
  });