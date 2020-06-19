import React from 'react';

import {
  Heading,
  Label,
  EmailInput,
  Text,
  Icons,
  utils
} from 'asc-web-components';

import StyledLanguageAndTimezoneBox from './StyledRegistrationSettingsBox';

const { EmailSettings } = utils.email;
const settings = new EmailSettings();
settings.allowDomainPunycode = true;

const RegistrationSettingsBox = ({
  domain, t
}) => (
  <StyledLanguageAndTimezoneBox>
    <Heading className="reg-settings-title" title="Registration Settings" className="header-base">
      {t('registrationSettingsBox.heading')} 
    </Heading>

      <Label 
        className="settings-label"
        text={t('registrationSettingsBox.labelEmail')}
        title="registered email"
        display="block"
        htmlFor="input-email"
      />

      <EmailInput
          id="input-email"
          name="email-wizard"
          placeholder={t('registrationSettingsBox.placeholder')}
          emailSettings={settings}
          onValidateInput={result => {
            console.log("onValidateInput", result.value, result.isValid, result.errors)
          }}
      />

      <Label 
        className="settings-label"
        text={t('registrationSettingsBox.labelDomain')}
        title="registered domain"
        display="block"
        htmlFor="input-email"
      />
      <Text className="domain-name">
        {domain}
      </Text>
      <Icons.QuestionIcon size="small" className="question-icon"/>
  </StyledLanguageAndTimezoneBox>
);

export default RegistrationSettingsBox;