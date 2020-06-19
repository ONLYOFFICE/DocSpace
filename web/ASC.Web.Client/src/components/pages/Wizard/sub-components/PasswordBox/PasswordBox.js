import React from 'react';

import {
  Heading,
  Label,
  PasswordInput
} from 'asc-web-components';

import StyledPasswordBox from './StyledPasswordBox';

const settingsPassword = {
  minLength: 6,
  upperCase: true,
  digits: true,
  specSymbols: true
};

const PasswordBox = ({ 
  firstPass, secondPass, t
}) => (
  <StyledPasswordBox>
      <Heading level={2} className="header-base">
        {t('passwordBox.heading')}
      </Heading>

      <Label
        className="label-pass"
        text={t('passwordBox.labelFirstPass')}
        title="first pass"
        htmlFor="first"
        display="block"
      /> 

    <PasswordInput
      className="wizard-pass"
      id="first"
      inputName="firstPass"
      emailInputName="email-wizard"
      inputValue={firstPass}
      onChange={e => console.log(e)}
      tooltipPasswordTitle={t('passwordBox.tooltipPasswordTitle')}
      tooltipPasswordLength={t('passwordBox.tooltipPasswordLength')}
      tooltipPasswordDigits={t('passwordBox.tooltipPasswordDigits')}
      tooltipPasswordCapital={t('passwordBox.tooltipPasswordCapital')}
      tooltipPasswordSpecial={t('passwordBox.tooltipPasswordSpecial')}
      generatorSpecial="!@#$%^&*"
      passwordSettings={settingsPassword}
      isDisabled={false}
      placeholder={t('passwordBox.placeholder')}
      onValidateInput={a => console.log(a)}
    />

      <Label
        className="label-pass"
        text={t('passwordBox.labelSecondPass')}
        title="second pass"
        htmlFor="second"
        display="block"
      />
    <PasswordInput
      className="wizard-pass"
      id="second"
      inputName="secondPass"
      emailInputName="email-wizard"
      inputValue={secondPass}
      onChange={e => console.log(e)}
      tooltipPasswordTitle={t('passwordBox.tooltipPasswordTitle')}
      tooltipPasswordLength={t('passwordBox.tooltipPasswordLength')}
      tooltipPasswordDigits={t('passwordBox.tooltipPasswordDigits')}
      tooltipPasswordCapital={t('passwordBox.tooltipPasswordCapital')}
      tooltipPasswordSpecial={t('passwordBox.tooltipPasswordSpecial')}
      generatorSpecial="!@#$%^&*"
      passwordSettings={settingsPassword}
      isDisabled={false}
      placeholder={t('passwordBox.placeholder')}
      onValidateInput={a => console.log(a)}
    />
  </StyledPasswordBox> 
);

export default PasswordBox;