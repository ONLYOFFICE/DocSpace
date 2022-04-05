import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Slider from "@appserver/components/slider";
import Checkbox from "@appserver/components/checkbox";
import SectionLoader from "../sub-components/section-loader";
import { getLanguage } from "@appserver/common/utils";
import { ButtonsWrapper, LearnMoreWrapper } from "../StyledSecurity";
import toastr from "@appserver/components/toast/toastr";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";

const MainContainer = styled.div`
  width: 100%;

  .page-subtitle {
    margin-bottom: 10px;
  }

  .password-slider {
    width: 160px;
    height: 8px;
    margin: 24px 16px 24px 0px;
  }

  .checkboxes {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-top: 18px;
    margin-bottom: 24px;
  }
`;

const PasswordStrength = (props) => {
  const { t, history, setPortalPasswordSettings, passwordSettings } = props;
  const [passwordLen, setPasswordLen] = useState(8);
  const [useUpperCase, setUseUpperCase] = useState(false);
  const [useDigits, setUseDigits] = useState(false);
  const [useSpecialSymbols, setUseSpecialSymbols] = useState(false);

  const [currentState, setCurrentState] = useState([8, false, false, false]);

  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentPasswordSettings");

    if (currentSettings) {
      setPasswordLen(currentSettings.minLength);
      setUseUpperCase(currentSettings.upperCase);
      setUseDigits(currentSettings.digits);
      setUseSpecialSymbols(currentSettings.specSymbols);
    } else {
      setPasswordLen(passwordSettings.minLength);
      setUseUpperCase(passwordSettings.upperCase);
      setUseDigits(passwordSettings.digits);
      setUseSpecialSymbols(passwordSettings.specSymbols);
    }

    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");
    const defaultData = {
      minLength: passwordSettings.minLength,
      upperCase: passwordSettings.upperCase,
      digits: passwordSettings.digits,
      specSymbols: passwordSettings.specSymbols,
    };

    saveToSessionStorage(
      "defaultPasswordSettings",
      defaultSettings || defaultData
    );

    setIsLoading(true);
  };

  useEffect(() => {
    checkWidth();
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  useEffect(() => {
    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");
    const newSettings = {
      minLength: passwordLen,
      upperCase: useUpperCase,
      digits: useDigits,
      specSymbols: useSpecialSymbols,
    };
    saveToSessionStorage("currentPasswordSettings", newSettings);

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [passwordLen, useUpperCase, useDigits, useSpecialSymbols]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("password") &&
      history.push("/settings/security/access-portal");
  };

  const onSliderChange = (e) => {
    setPasswordLen(Number(e.target.value));
  };

  const onClickCheckbox = (e) => {
    switch (e.target.value) {
      case "upperCase":
        setUseUpperCase(e.target.checked);
        break;
      case "digits":
        setUseDigits(e.target.checked);
        break;
      case "special":
        setUseSpecialSymbols(e.target.checked);
        break;
    }
  };

  const onSaveClick = () => {
    setPortalPasswordSettings(
      passwordLen,
      useUpperCase,
      useDigits,
      useSpecialSymbols
    )
      .then(() => {
        setShowReminder(false);
        saveToSessionStorage("defaultPasswordSettings", {
          minLength: passwordLen,
          upperCase: useUpperCase,
          digits: useDigits,
          specSymbols: useSpecialSymbols,
        });
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      })
      .catch((e) => toastr.error(e));
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");
    setPasswordLen(defaultSettings.minLength);
    setUseUpperCase(defaultSettings.upperCase);
    setUseDigits(defaultSettings.digits);
    setUseSpecialSymbols(defaultSettings.specSymbols);
    setShowReminder(false);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");
  if (!isLoading) return <SectionLoader />;
  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="page-subtitle">
          {t("SettingPasswordStrengthHelper")}
        </Text>
        <Link
          color="#316DAA"
          target="_blank"
          isHovered
          href={`https://helpcenter.onlyoffice.com/${lng}/administration/configuration.aspx#ChangingSecuritySettings_block`}
        >
          {t("Common:LearnMore")}
        </Link>
      </LearnMoreWrapper>

      <Text fontSize="14px" fontWeight="600" className="length-subtitle">
        {t("PasswordMinLenght")}
      </Text>

      <Box displayProp="flex" flexDirection="row" alignItems="center">
        <Slider
          className="password-slider"
          min="8"
          max="30"
          step="1"
          withPouring={true}
          value={passwordLen}
          onChange={onSliderChange}
        />
        <Text>
          {t("Characters", {
            length: passwordLen,
          })}
        </Text>
      </Box>

      <Box className="checkboxes">
        <Checkbox
          onChange={onClickCheckbox}
          label={t("UseUpperCase")}
          isChecked={useUpperCase}
          value="upperCase"
        />
        <Checkbox
          onChange={onClickCheckbox}
          label={t("UseDigits")}
          isChecked={useDigits}
          value="digits"
        />
        <Checkbox
          onChange={onClickCheckbox}
          label={t("UseSpecialChar")}
          isChecked={useSpecialSymbols}
          value="special"
        />
      </Box>

      <ButtonsWrapper>
        <Button
          label={t("Common:SaveButton")}
          size="small"
          primary={true}
          className="button"
          onClick={onSaveClick}
          isDisabled={!showReminder}
        />
        <Button
          label={t("Common:CancelButton")}
          size="small"
          className="button"
          onClick={onCancelClick}
          isDisabled={!showReminder}
        />
        {showReminder && (
          <Text
            color="#A3A9AE"
            fontSize="12px"
            fontWeight="600"
            className="reminder"
          >
            {t("YouHaveUnsavedChanges")}
          </Text>
        )}
      </ButtonsWrapper>
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const { setPortalPasswordSettings, passwordSettings } = auth.settingsStore;

  return {
    setPortalPasswordSettings,
    passwordSettings,
  };
})(
  withTranslation(["Settings", "Common"])(
    withRouter(observer(PasswordStrength))
  )
);
