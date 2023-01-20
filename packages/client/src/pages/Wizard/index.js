import React, { useState, useRef, useEffect } from "react";
import axios from "axios";
import { useTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { isMobileOnly } from "react-device-detect";

import Text from "@docspace/components/text";
import FormWrapper from "@docspace/components/form-wrapper";
import EmailInput from "@docspace/components/email-input";
import PasswordInput from "@docspace/components/password-input";
import IconButton from "@docspace/components/icon-button";
import ComboBox from "@docspace/components/combobox";
import Link from "@docspace/components/link";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";

import Loader from "@docspace/components/loader";

import withCultureNames from "@docspace/common/hoc/withCultureNames";
import { EmailSettings } from "@docspace/components/utils/email";
import {
  combineUrl,
  createPasswordHash,
  convertLanguage,
} from "@docspace/common/utils";

import {
  Wrapper,
  WizardContainer,
  StyledLink,
  StyledInfo,
  StyledAcceptTerms,
} from "./StyledWizard";
import DocspaceLogo from "SRC_DIR/DocspaceLogo";

const emailSettings = new EmailSettings();
emailSettings.allowDomainPunycode = true;

const Wizard = (props) => {
  const {
    passwordSettings,
    isWizardLoaded,
    setIsWizardLoaded,
    wizardToken,
    history,
    getPortalPasswordSettings,
    getMachineName,
    getIsRequiredLicense,
    getPortalTimezones,
    machineName,
    urlLicense,
    theme,
    cultureNames,
    culture,
    timezone,
  } = props;
  const { t } = useTranslation(["Wizard", "Common"]);

  const [email, setEmail] = useState("");
  const [hasErrorEmail, setHasErrorEmail] = useState(false);
  const [password, setPassword] = useState("");
  const [hasErrorPass, setHasErrorPass] = useState(false);
  const [agreeTerms, setAgreeTerms] = useState(false);
  const [selectedLanguage, setSelectedLanguage] = useState(null);
  const [timezones, setTimezones] = useState(null);
  const [selectedTimezone, setSelectedTimezone] = useState(null);

  const refPassInput = useRef(null);

  const convertedCulture = convertLanguage(culture);

  const mapTimezonesToArray = (timezones) => {
    return timezones.map((timezone) => {
      return { key: timezone.id, label: timezone.displayName };
    });
  };

  const fetchData = async () => {
    await axios
      .all([
        getPortalPasswordSettings(wizardToken),
        getMachineName(wizardToken),
        getIsRequiredLicense(),
        getPortalTimezones(wizardToken).then((data) => {
          const zones = mapTimezonesToArray(data);
          const select = zones.filter((zone) => zone.key === timezone);

          setTimezones(zones);
          setSelectedTimezone({
            key: select[0].key,
            label: select[0].label,
          });
        }),
      ])
      .then(() => {
        let select = cultureNames.filter(
          (lang) => lang.key === convertedCulture
        );
        if (!select.length)
          select = cultureNames.filter((lang) => lang.key === "en");

        setSelectedLanguage({
          key: select[0].key,
          label: select[0].label,
          icon: select[0].icon,
        });
        setIsWizardLoaded(true);
      });
  };

  useEffect(() => {
    if (!wizardToken)
      history.push(combineUrl(window.DocSpaceConfig?.proxy?.url, "/"));
    else fetchData();
  }, []);

  const onEmailChangeHandler = (result) => {
    console.log(result);
    setEmail(result.value);
    setHasErrorEmail(!result.isValid);
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  const onLanguageSelect = (lang) => {
    setSelectedLanguage(lang);
  };

  const onTimezoneSelect = (timezone) => {
    setSelectedTimezone(timezone);
  };

  if (!isWizardLoaded)
    return <Loader className="pageLoader" type="rombs" size="40px" />;

  return (
    <Wrapper>
      <WizardContainer>
        <DocspaceLogo className="docspace-logo" />
        <Text fontWeight={700} fontSize="23px" className="welcome-text">
          {t("WelcomeTitle")}
        </Text>
        <FormWrapper>
          <Text fontWeight={600} fontSize="16px" className="form-header">
            {t("WizardTitle")}
          </Text>
          <EmailInput
            name="wizard-email"
            tabIndex={1}
            size="large"
            scale={true}
            placeholder={t("Common:Email")}
            emailSettings={emailSettings}
            hasError={hasErrorEmail}
            onValidateInput={onEmailChangeHandler}
          />
          <PasswordInput
            className="wizard-password"
            ref={refPassInput}
            tabIndex={2}
            size="large"
            scale={true}
            inputValue={password}
            passwordSettings={passwordSettings}
            isDisabled={false}
            placeholder={t("Common:Password")}
            hideNewPasswordButton={true}
            isDisableTooltip={true}
            isTextTooltipVisible={false}
            hasError={hasErrorPass}
            onChange={onChangePassword}
            autoComplete="current-password"
            //onValidateInput={isValidPassHandler}
          />
          <StyledLink>
            <IconButton
              size="12"
              iconName="/static/images/refresh.react.svg"
              onClick={() => refPassInput.current.onGeneratePassword()}
            />
            <Link
              className="generate-password-link"
              type="action"
              fontWeight={600}
              isHovered={true}
              onClick={() => refPassInput.current.onGeneratePassword()}
            >
              {t("GeneratePassword")}
            </Link>
          </StyledLink>
          <StyledInfo>
            <Text color="#A3A9AE" fontWeight={400}>
              {t("Domain")}
            </Text>
            <Text fontWeight={600} className="machine-name">
              {machineName}
            </Text>
          </StyledInfo>
          <StyledInfo>
            <Text color="#A3A9AE" fontWeight={400}>
              {t("Common:Language")}
            </Text>
            <ComboBox
              directionY="both"
              options={cultureNames}
              selectedOption={selectedLanguage}
              onSelect={onLanguageSelect}
              isDisabled={false}
              scaled={isMobileOnly}
              scaledOptions={false}
              size="content"
              showDisabledItems={true}
              dropDownMaxHeight={364}
              manualWidth="250px"
              isDefaultMode={!isMobileOnly}
              withBlur={isMobileOnly}
              fillIcon={false}
              modernView={true}
            />
          </StyledInfo>
          <StyledInfo>
            <Text color="#A3A9AE" fontWeight={400}>
              {t("Timezone")}
            </Text>
            <ComboBox
              directionY="both"
              options={timezones}
              selectedOption={selectedTimezone}
              onSelect={onTimezoneSelect}
              isDisabled={false}
              scaled={isMobileOnly}
              scaledOptions={false}
              size="content"
              showDisabledItems={true}
              dropDownMaxHeight={364}
              manualWidth="250px"
              isDefaultMode={!isMobileOnly}
              withBlur={isMobileOnly}
              fillIcon={false}
              modernView={true}
            />
          </StyledInfo>

          <StyledAcceptTerms>
            <Checkbox
              className="wizard-checkbox"
              id="license"
              name="confirm"
              label={t("License")}
              isChecked={agreeTerms}
              onChange={() => setAgreeTerms(!agreeTerms)}
            />
            <Link
              type="page"
              color={theme.client.wizard.linkColor}
              fontSize="13px"
              target="_blank"
              href={
                urlLicense
                  ? urlLicense
                  : "https://gnu.org/licenses/gpl-3.0.html"
              }
            >
              {t("LicenseLink")}
            </Link>
          </StyledAcceptTerms>
          <Button
            size="medium"
            scale={true}
            primary
            label={t("Common:ContinueButton")}
            //onClick={onContinueHandler}
          />
        </FormWrapper>
      </WizardContainer>
    </Wrapper>
  );
};

export default inject(({ auth, wizard }) => {
  const {
    passwordSettings,
    wizardToken,
    timezone,
    urlLicense,
    hashSettings,
    getPortalSettings,
    setWizardComplete,
    getPortalTimezones,
    getPortalPasswordSettings,
    theme,
  } = auth.settingsStore;

  const { language } = auth;
  const {
    isWizardLoaded,
    machineName,
    isLicenseRequired,
    licenseUpload,
    setIsWizardLoaded,
    getMachineName,
    getIsRequiredLicense,
    setPortalOwner,
    setLicense,
    resetLicenseUploaded,
  } = wizard;

  return {
    theme,
    isLoaded: auth.isLoaded,
    culture: language,
    wizardToken,
    passwordSettings,
    timezone,
    urlLicense,
    hashSettings,
    isWizardLoaded,
    machineName,
    isLicenseRequired,
    licenseUpload,
    getPortalSettings,
    setWizardComplete,
    getPortalPasswordSettings,
    getPortalTimezones,
    setIsWizardLoaded,
    getMachineName,
    getIsRequiredLicense,
    setPortalOwner,
    setLicense,
    resetLicenseUploaded,
  };
})(withRouter(withCultureNames(observer(Wizard))));
