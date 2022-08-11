import React, { useRef } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Box from "@docspace/components/box";
import EmailInput from "@docspace/components/email-input";
import FileInput from "@docspace/components/file-input";
import PasswordInput from "@docspace/components/password-input";
import Link from "@docspace/components/link";
import Checkbox from "@docspace/components/checkbox";
import { PasswordLimitSpecialCharacters } from "@docspace/common/constants";
import { tablet } from "@docspace/components/utils/device";

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr;
  grid-row-gap: 16px;

  .password-field-wrapper {
    width: 100%;
  }

  .generate-pass-link {
    margin-bottom: 11px;
  }

  .wizard-checkbox {
    display: inline-block;
  }

  .wizard-checkbox span {
    margin-right: 0.3em;
    vertical-align: 2px;
  }

  .wizard-checkbox svg {
    margin-right: 8px;
  }

  .link {
    vertical-align: 2px;
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
  theme,
}) => {
  const refPassInput = useRef(null);

  const tooltipPassTitle = `${t("Common:PasswordLimitMessage")}:`;
  const tooltipPassLength = `${t("Common:PasswordLimitLength", {
    fromNumber: settingsPassword ? settingsPassword.minLength : 8,
    toNumber: 30,
  })}`;
  const tooltipPassDigits = settingsPassword.digits
    ? `${t("Common:PasswordLimitDigits")}`
    : null;
  const tooltipPassCapital = settingsPassword.upperCase
    ? `${t("Common:PasswordLimitUpperCase")}`
    : null;
  const tooltipPassSpecial = settingsPassword.specSymbols
    ? `${t(
        "Common:PasswordLimitSpecialSymbols"
      )} (${PasswordLimitSpecialCharacters})`
    : null;

  const inputEmail = emailNeeded ? (
    <EmailInput
      name="wizard-email"
      tabIndex={1}
      size="large"
      scale={true}
      placeholder={t("Common:Email")}
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
        placeholder={t("Common:Password")}
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
          color={theme.client.wizard.linkColor}
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
