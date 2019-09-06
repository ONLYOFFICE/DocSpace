import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { withKnobs, boolean, text, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { PasswordInput, TextInput } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('advanced password', () => {
    const isDisabled = boolean('isDisabled', false);
    const settingsUpperCase = boolean('settingsUpperCase', false);
    const settingsDigits = boolean('settingsDigits', false);
    const settingsSpecSymbols = boolean('settingsSpecSymbols', false);

    const fakeSettings = {
      minLength: 6,
      upperCase: settingsUpperCase,
      digits: settingsDigits,
      specSymbols: settingsSpecSymbols
    };

    const tooltipPasswordLength = 'from ' + fakeSettings.minLength + ' to 30 characters';

    return (
      <Section>
        <div style={{height: '110px'}}></div> 
        <TextInput
          name='demoEmailInput'
          size='base'
          isDisabled={isDisabled}
          isReadOnly={true}
          scale={true}
          value='demo@gmail.com'
        />
        <br />
        <StringValue>
          {({ value, set }) => (
            <PasswordInput
              inputName='demoPasswordInput'
              emailInputName='demoEmailInput'
              inputValue={value}
              onChange={e => {
                set(e.target.value);
              }}
              clipActionResource='Copy e-mail and password'
              clipEmailResource='E-mail: '
              clipPasswordResource='Password: '
              tooltipPasswordTitle='Password must contain:'
              tooltipPasswordLength={tooltipPasswordLength}
              tooltipPasswordDigits='digits'
              tooltipPasswordCapital='capital letters'
              tooltipPasswordSpecial='special characters (!@#$%^&*)'
              generatorSpecial='!@#$%^&*'
              passwordSettings={fakeSettings}
              isDisabled={isDisabled}
              placeholder='password'
              maxLength={30}
            />
          )}
        </StringValue>
      </Section>
    )
  });