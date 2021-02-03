import React, { useRef } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import {
  Box,
  EmailInput,
  FileInput,
  PasswordInput,
  Link,
  Checkbox,
  utils,
} from "asc-web-components";

const { tablet } = utils.device;

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr;
  grid-row-gap: 16px;

  .generate-pass-link {
    margin-bottom: 11px;
  }

  .wizard-checkbox {
    display: inline-block;
  }

  .wizard-checkbox span {
    margin-right: 0.3em;
    vertical-align: middle;
  }

  .wizard-checkbox svg {
    margin-right: 8px;
  }

  .link {
    vertical-align: -2px;
    margin-top: 2px;
  }

  @media ${tablet} {
    width: 100%;
  }
`;

const InputContainer = ({
  t,
  settingsPassword,
  emailNeeded,
  onEmailChangeHandler,
  onInputFileHandler,
  password,
  onChangePassword,
  isValidPassHandler,
  license,
  onChangeLicense,
  settings,
  isLicenseRequired,
  hasErrorEmail,
  hasErrorPass,
  hasErrorLicense,
  urlLicense,
}) => {
  const refPassInput = useRef(null);

  const tooltipPassTitle = t("TooltipPasswordTitle");
  const tooltipPassLength = `${settingsPassword.minLength}${t(
    "TooltipPasswordLength"
  )}`;
  const tooltipPassDigits = settingsPassword.digits
    ? `${t("TooltipPasswordDigits")}`
    : null;
  const tooltipPassCapital = settingsPassword.upperCase
    ? `${t("TooltipPasswordCapital")}`
    : null;
  const tooltipPassSpecial = settingsPassword.specSymbols
    ? `${t("TooltipPasswordSpecial")}`
    : null;

  const inputEmail = emailNeeded ? (
    <EmailInput
      name="wizard-email"
      tabIndex={1}
      size="large"
      scale={true}
      placeholder={t("PlaceholderEmail")}
      emailSettings={settings}
      hasError={hasErrorEmail}
      onValidateInput={onEmailChangeHandler}
    />
  ) : null;

  const inputLicenseFile = isLicenseRequired ? (
    <Box>
      <FileInput
        tabIndex={3}
        placeholder={t("PlaceholderLicense")}
        size="large"
        scale={true}
        accept=".lic"
        hasError={hasErrorLicense}
        onInput={onInputFileHandler}
      />
    </Box>
  ) : null;

  return (
    <StyledContainer>
      {inputEmail}

      <PasswordInput
        ref={refPassInput}
        tabIndex={2}
        size="large"
        scale={true}
        inputValue={password}
        passwordSettings={settingsPassword}
        isDisabled={false}
        placeholder={t("PlaceholderPass")}
        hideNewPasswordButton={true}
        isDisableTooltip={true}
        isTextTooltipVisible={true}
        hasError={hasErrorPass}
        tooltipPasswordTitle={tooltipPassTitle}
        tooltipPasswordLength={tooltipPassLength}
        tooltipPasswordDigits={tooltipPassDigits}
        tooltipPasswordCapital={tooltipPassCapital}
        tooltipPasswordSpecial={tooltipPassSpecial}
        onChange={onChangePassword}
        onValidateInput={isValidPassHandler}
      />
      {inputLicenseFile}
      {!isLicenseRequired ? (
        <Link
          className="generate-pass-link"
          type="action"
          fontWeight="400"
          isHovered={true}
          onClick={() => refPassInput.current.onGeneratePassword()}
        >
          {t("GeneratePassword")}
        </Link>
      ) : null}
      <Box>
        <Checkbox
          className="wizard-checkbox"
          id="license"
          name="confirm"
          label={t("License")}
          isChecked={license}
          isDisabled={false}
          onChange={onChangeLicense}
        />
        <Link
          className="link"
          type="page"
          color="#116d9d"
          fontSize="13px"
          target="_blank"
          href={
            urlLicense ? urlLicense : "https://gnu.org/licenses/gpl-3.0.html"
          }
          isBold={false}
        >
          {t("LicenseLink")}
        </Link>
      </Box>
    </StyledContainer>
  );
};

InputContainer.propTypes = {
  t: PropTypes.func.isRequired,
  settingsPassword: PropTypes.object.isRequired,
  emailNeeded: PropTypes.bool.isRequired,
  onEmailChangeHandler: PropTypes.func.isRequired,
  onInputFileHandler: PropTypes.func.isRequired,
  password: PropTypes.string.isRequired,
  onChangePassword: PropTypes.func.isRequired,
  isValidPassHandler: PropTypes.func.isRequired,
  license: PropTypes.bool.isRequired,
  onChangeLicense: PropTypes.func.isRequired,
  urlLicense: PropTypes.string,
};

export default InputContainer;
