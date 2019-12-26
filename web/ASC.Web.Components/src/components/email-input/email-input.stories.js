import React from 'react';
import { storiesOf } from '@storybook/react';
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

    const placeholder = text('placeholder', 'Input email');
    const size = select('size', sizeInput, 'base');
    const scale = boolean('scale', false);
    const isDisabled = boolean('isDisabled', false);
    const isReadOnly = boolean('isReadOnly', false);
    const maxLength = number('maxLength', 255);
    const id = text('id', 'emailId');
    const name = text('name', 'demoEmailInput');

    const allowDomainPunycode = boolean('allowDomainPunycode', false);
    const allowLocalPartPunycode = boolean('allowLocalPartPunycode', false);
    const allowDomainIp = boolean('allowDomainIp', false);
    const allowStrictLocalPart = boolean('allowStrictLocalPart', true);
    const allowSpaces = boolean('allowSpaces', false);
    const allowName = boolean('allowName', false);
    const allowLocalDomainName = boolean('allowLocalDomainName', false);

    const settings = {
      allowDomainPunycode,
      allowLocalPartPunycode,
      allowDomainIp,
      allowStrictLocalPart,
      allowSpaces,
      allowName,
      allowLocalDomainName
    }

    return (
      <Section>
        <EmailInput
          placeholder={placeholder}
          size={size}
          scale={scale}
          isDisabled={isDisabled}
          isReadOnly={isReadOnly}
          maxLength={maxLength}
          id={id}
          name={name}
          emailSettings={settings}
          onValidateInput={(isEmailValid) => action('onValidateInput')(isEmailValid)}
        />
      </Section>
    );
  });