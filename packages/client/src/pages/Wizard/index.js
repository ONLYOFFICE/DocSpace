import React, { useState, useRef, useEffect } from "react";
import axios from "axios";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
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
import FieldContainer from "@docspace/components/field-container";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import FileInput from "@docspace/components/file-input";

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
  StyledContent,
} from "./StyledWizard";
import { getUserTimezone, getSelectZone } from "./timezonesHelper";

import DocspaceLogo from "SRC_DIR/DocspaceLogo";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";
import {
  DEFAULT_SELECT_TIMEZONE,
  DEFAULT_SELECT_LANGUAGE,
} from "SRC_DIR/helpers/constants";

const emailSettings = new EmailSettings();
emailSettings.allowDomainPunycode = true;

const Wizard = (props) => {
  const {
    passwordSettings,
    isWizardLoaded,
    setIsWizardLoaded,
    wizardToken,

    getPortalPasswordSettings,
    getMachineName,
    getIsRequiredLicense,
    getPortalTimezones,
    machineName,
    urlLicense,
    theme,
    cultureNames,
    culture,
    hashSettings,
    setPortalOwner,
    setWizardComplete,
    isLicenseRequired,
    setLicense,
    licenseUpload,
    resetLicenseUploaded,
  } = props;

  const navigate = useNavigate();
  const { t } = useTranslation(["Wizard", "Common"]);

  const [email, setEmail] = useState("");
  const [hasErrorEmail, setHasErrorEmail] = useState(false);
  const [password, setPassword] = useState("");
  const [hasErrorPass, setHasErrorPass] = useState(false);
  const [agreeTerms, setAgreeTerms] = useState(false);
  const [hasErrorAgree, setHasErrorAgree] = useState(false);
  const [selectedLanguage, setSelectedLanguage] = useState(null);
  const [timezones, setTimezones] = useState(null);
  const [selectedTimezone, setSelectedTimezone] = useState(null);
  const [isCreated, setIsCreated] = useState(false);
  const [errorInitWizard, setErrorInitWizard] = useState(false);
  const [hasErrorLicense, setHasErrorLicense] = useState(false);
  const [invalidLicense, setInvalidLicense] = useState(false);

  const refPassInput = useRef(null);

  const userCulture = window.navigator
    ? window.navigator.language ||
      window.navigator.systemLanguage ||
      window.navigator.userLanguage
    : culture;

  const convertedCulture = convertLanguage(userCulture);

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
          const userTimezone = getUserTimezone();
          const zones = mapTimezonesToArray(data);
          const select = getSelectZone(zones, userTimezone);

          setTimezones(zones);
          if (!select) {
            setSelectedTimezone(DEFAULT_SELECT_TIMEZONE);
          } else {
            setSelectedTimezone(select[0]);
          }
        }),
      ])
      .then(() => {
        const select = cultureNames.filter(
          (lang) => lang.key === convertedCulture
        );

        if (!select) {
          setSelectedLanguage(DEFAULT_SELECT_LANGUAGE);
        } else {
          setSelectedLanguage(select[0]);
        }
        setIsWizardLoaded(true);
      })
      .catch((error) => {
        let errorMessage = "";
        if (typeof err === "object") {
          errorMessage =
            error?.response?.data?.error?.message ||
            error?.statusText ||
            error?.message ||
            "";
        } else {
          errorMessage = error;
        }
        console.error(errorMessage);
        setErrorInitWizard(true);
      });
  };

  useEffect(() => {
    if (!wizardToken)
      navigate(combineUrl(window.DocSpaceConfig?.proxy?.url, "/"));
    else fetchData();
  }, []);

  const onEmailChangeHandler = (result) => {
    setEmail(result.value);
    setHasErrorEmail(!result.isValid);
  };

  const onChangePassword = (e) => {
    setPassword(e.target.value);
  };

  const isValidPassHandler = (value) => {
    setHasErrorPass(!value);
  };

  const generatePassword = () => {
    if (isCreated) return;
    refPassInput.current.onGeneratePassword();
  };

  const onLanguageSelect = (lang) => {
    setSelectedLanguage(lang);
  };

  const onTimezoneSelect = (timezone) => {
    setSelectedTimezone(timezone);
  };

  const onLicenseFileHandler = (file) => {
    if (licenseUpload) resetLicenseUploaded();
    setHasErrorLicense(false);
    setInvalidLicense(false);

    let fd = new FormData();
    fd.append("files", file);

    setLicense(wizardToken, fd).catch((e) => {
      console.error(e);
      setHasErrorLicense(true);
      setInvalidLicense(true);
    });
  };

  const onAgreeTermsChange = () => {
    if (hasErrorAgree && !agreeTerms) setHasErrorAgree(false);
    setAgreeTerms(!agreeTerms);
  };

  const validateFields = () => {
    const emptyEmail = email.trim() === "";
    const emptyPassword = password.trim() === "";

    if (emptyEmail || emptyPassword) {
      emptyEmail && setHasErrorEmail(true);
      emptyPassword && setHasErrorPass(true);
    }

    if (!agreeTerms) {
      setHasErrorAgree(true);
    }

    if (isLicenseRequired && !licenseUpload) {
      setHasErrorLicense(true);
    }

    if (
      emptyEmail ||
      emptyPassword ||
      hasErrorEmail ||
      hasErrorPass ||
      !agreeTerms ||
      (isLicenseRequired && !licenseUpload)
    )
      return false;

    return true;
  };

  const onContinueClick = async () => {
    if (!validateFields()) return;

    setIsCreated(true);

    const emailTrim = email.trim();
    const analytics = true;
    const hash = createPasswordHash(password, hashSettings);

    try {
      await setPortalOwner(
        emailTrim,
        hash,
        selectedLanguage.key,
        selectedTimezone.key,
        wizardToken,
        analytics
      );
      setWizardComplete();

      // navigate(combineUrl(window.DocSpaceConfig?.proxy?.url, "/login"));

      window.location.replace(
        combineUrl(window.DocSpaceConfig?.proxy?.url, "/login")
      );
    } catch (error) {
      console.error(error);
      setIsCreated(false);
    }
  };

  const onClickRetry = () => {
    window.location.href = "/";
  };

  if (!isWizardLoaded)
    return <Loader className="pageLoader" type="rombs" size="40px" />;

  if (errorInitWizard)
    return (
      <ErrorContainer
        headerText={t("Common:SomethingWentWrong")}
        bodyText={t("ErrorInitWizard")}
        buttonText={t("ErrorInitWizardButton")}
        onClickButton={onClickRetry}
      />
    );

  return (
    <Wrapper>
      <div className="bg-cover"></div>
      <StyledContent>
        <WizardContainer>
          <DocspaceLogo className="docspace-logo" />
          <Text
            as="div"
            fontWeight={700}
            fontSize="23px"
            className="welcome-text"
          >
            {t("WelcomeTitle")}
          </Text>
          <FormWrapper>
            <Text fontWeight={600} fontSize="16px" className="form-header">
              {t("Desc")}
            </Text>
            <FieldContainer
              className="wizard-field"
              isVertical={true}
              labelVisible={false}
              hasError={hasErrorEmail}
              errorMessage={t("ErrorEmail")}
            >
              <EmailInput
                name="wizard-email"
                tabIndex={1}
                size="large"
                scale={true}
                placeholder={t("Common:Email")}
                emailSettings={emailSettings}
                hasError={hasErrorEmail}
                onValidateInput={onEmailChangeHandler}
                isDisabled={isCreated}
              />
            </FieldContainer>

            <FieldContainer
              className="wizard-field password-field"
              isVertical={true}
              labelVisible={false}
              hasError={hasErrorPass}
              errorMessage={t("ErrorPassword")}
            >
              <PasswordInput
                ref={refPassInput}
                tabIndex={2}
                size="large"
                scale={true}
                inputValue={password}
                passwordSettings={passwordSettings}
                isDisabled={isCreated}
                placeholder={t("Common:Password")}
                hideNewPasswordButton={true}
                isDisableTooltip={true}
                isTextTooltipVisible={false}
                hasError={hasErrorPass}
                onChange={onChangePassword}
                autoComplete="current-password"
                onValidateInput={isValidPassHandler}
              />
            </FieldContainer>
            <StyledLink>
              <IconButton
                size="12"
                iconName={RefreshReactSvgUrl}
                onClick={generatePassword}
              />
              <Link
                className="generate-password-link"
                type="action"
                fontWeight={600}
                isHovered={true}
                onClick={generatePassword}
              >
                {t("GeneratePassword")}
              </Link>
            </StyledLink>

            {isLicenseRequired && (
              <FieldContainer
                className="license-filed"
                isVertical={true}
                labelVisible={false}
                hasError={hasErrorLicense}
                errorMessage={
                  invalidLicense
                    ? t("ErrorLicenseBody")
                    : t("ErrorUploadLicenseFile")
                }
              >
                <FileInput
                  scale
                  size="large"
                  accept=".lic"
                  placeholder={t("PlaceholderLicense")}
                  onInput={onLicenseFileHandler}
                  hasError={hasErrorLicense}
                />
              </FieldContainer>
            )}
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
                withoutPadding
                directionY="both"
                options={cultureNames}
                selectedOption={selectedLanguage}
                onSelect={onLanguageSelect}
                isDisabled={isCreated}
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
                textOverflow
                withoutPadding
                directionY="both"
                options={timezones}
                selectedOption={selectedTimezone}
                onSelect={onTimezoneSelect}
                isDisabled={isCreated}
                scaled={isMobileOnly}
                scaledOptions={false}
                size="content"
                showDisabledItems={true}
                dropDownMaxHeight={364}
                manualWidth="350px"
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
                onChange={onAgreeTermsChange}
                isDisabled={isCreated}
                hasError={hasErrorAgree}
              />
              <Link
                type="page"
                color={
                  hasErrorAgree
                    ? theme.checkbox.errorColor
                    : theme.client.wizard.linkColor
                }
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
              isLoading={isCreated}
              onClick={onContinueClick}
            />
          </FormWrapper>
        </WizardContainer>
      </StyledContent>
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
})(withCultureNames(observer(Wizard)));
