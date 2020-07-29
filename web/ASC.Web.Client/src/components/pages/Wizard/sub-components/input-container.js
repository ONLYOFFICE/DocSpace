import React, { useRef } from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { 
  Box, 
  EmailInput, 
  FileInput, 
  PasswordInput, 
  Link, 
  Checkbox, 
  utils 
} from 'asc-web-components';

const { tablet } = utils.device;

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 32px auto 0 auto;
    
  .wizard-pass-box { 
    width: 360px;
    margin-top: 16px;

    @media ${tablet} {
      width: 100%;
    }
  }

  .input-file {
    width: 100%;
    margin: 16px auto;
  }

  .generate-pass-link {
    display: block;
    margin: 16px 0 32px 0;
  }

  .checkbox-container {
    width: 100%;
    margin: 16px auto 0 auto;
  }

  .wizard-checkbox {
    display: inline-block;
  }

  .wizard-checkbox span {
    margin-right: 0.3em;
    vertical-align: middle;
  }

  .link {
    vertical-align: middle;
  }

  @media ${tablet} {
    width: 100%;
  }
`;

const InputContainer = ({
  t, 
  settingsPassword,  
  emailNeeded, 
  onEmailHandler, 
  onInputFileHandler, 
  password, 
  onChangePassword, 
  isValidPassHandler, 
  license, 
  onChangeLicense, 
  settings, 
  isRequiredLicense
}) => {
  const refPassInput = useRef(null);

  const tooltipPassTitle = t('tooltipPasswordTitle');
  const tooltipPassLength = `${settingsPassword.minLength} ${t('tooltipPasswordLength')}`;
  const tooltipPassDigits = settingsPassword.digits ? `${t('tooltipPasswordDigits')}` : null;
  const tooltipPassCapital = settingsPassword.upperCase ? `${t('tooltipPasswordCapital')}` : null;
  const tooltipPassSpecial = settingsPassword.specSymbols ? `${t('tooltipPasswordSpecial')}` : null;

  const inputEmail = emailNeeded 
    ? <EmailInput
        name="wizard-email"
        tabIndex={1}
        size="large"
        scale={true}
        placeholder={t('email')}
        emailSettings={settings}
        onValidateInput={onEmailHandler}
      />
    : null;

  const inputLicenseFile = isRequiredLicense 
    ? <Box className="input-file">
        <FileInput
          tabIndex={3}
          placeholder={t('placeholderLicense')}
          size="large"
          scale={true}
          onInput={onInputFileHandler}
        />
      </Box>
    : null; 

  return (
    <StyledContainer>
      {inputEmail}
      <Box className="wizard-pass-box" >
      <PasswordInput
        ref={refPassInput}
        className="wizard-pass" 
        emailInputName="wizard-email"
        tabIndex={2}
        inputName="firstPass"
        size="large"
        scale={true}
        inputValue={password}
        passwordSettings={settingsPassword}
        isDisabled={false}
        placeholder={t('placeholderPass')}
        tooltipPasswordTitle={tooltipPassTitle}
        tooltipPasswordLength={tooltipPassLength}
        tooltipPasswordDigits={tooltipPassDigits}
        tooltipPasswordCapital={tooltipPassCapital}
        tooltipPasswordSpecial={tooltipPassSpecial}
        onChange={onChangePassword}
        onValidateInput={isValidPassHandler}
      />
      </Box>
      { inputLicenseFile }
      {!isRequiredLicense 
        ? <Link 
            className='generate-pass-link'
            type="action"
            fontWeight="normal"
            onClick={() => refPassInput.current.onGeneratePassword()}>
              {t('generatePassword')}
          </Link>
        : null
      }
      <Box className="checkbox-container">
        <Checkbox
          className="wizard-checkbox"
          id="license"
          name="confirm"
          label={t('license')}
          isChecked={license}
          isDisabled={false}
          onChange={onChangeLicense}
        />
        <Link 
          className="link"
          type="page" 
          color="#116d9d" 
          fontSize="13px"
          href="https://gnu.org/licenses/gpl-3.0.html" 
          isBold={false}
        >{t('licenseLink')}</Link>
      </Box>
    </StyledContainer>
  );
}

InputContainer.propTypes = {
  t: PropTypes.func.isRequired,
  settingsPassword: PropTypes.object.isRequired,
  emailNeeded: PropTypes.bool.isRequired,
  onEmailHandler: PropTypes.func.isRequired,
  onInputFileHandler: PropTypes.func.isRequired,
  password: PropTypes.string.isRequired,
  onChangePassword: PropTypes.func.isRequired,
  isValidPassHandler: PropTypes.func.isRequired,
  license: PropTypes.bool.isRequired,
  onChangeLicense: PropTypes.func.isRequired
};

export default InputContainer;