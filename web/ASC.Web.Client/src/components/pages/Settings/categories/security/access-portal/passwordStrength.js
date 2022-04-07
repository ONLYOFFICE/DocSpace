import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Slider from "@appserver/components/slider";
import Checkbox from "@appserver/components/checkbox";
import { getLanguage } from "@appserver/common/utils";
import { LearnMoreWrapper } from "../StyledSecurity";
import toastr from "@appserver/components/toast/toastr";
import Buttons from "../sub-components/buttons";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";

const MainContainer = styled.div`
  width: 100%;

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
  }
`;

const PasswordStrength = (props) => {
  const { t, history, setPortalPasswordSettings, passwordSettings } = props;
  const [passwordLen, setPasswordLen] = useState(8);
  const [useUpperCase, setUseUpperCase] = useState(false);
  const [useDigits, setUseDigits] = useState(false);
  const [useSpecialSymbols, setUseSpecialSymbols] = useState(false);

  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentPasswordSettings");
    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");

    if (defaultSettings) {
      saveToSessionStorage("defaultPasswordSettings", defaultSettings);
    } else {
      const defaultData = {
        minLength: passwordSettings.minLength,
        upperCase: passwordSettings.upperCase,
        digits: passwordSettings.digits,
        specSymbols: passwordSettings.specSymbols,
      };
      saveToSessionStorage("defaultPasswordSettings", defaultData);
    }

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

    setIsLoading(true);
  };

  useEffect(() => {
    checkWidth();
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, [isLoading]);

  useEffect(() => {
    if (!isLoading) return;
    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");

    const newSettings = {
      minLength: passwordLen,
      upperCase: useUpperCase,
      digits: useDigits,
      specSymbols: useSpecialSymbols,
    };

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      saveToSessionStorage("currentPasswordSettings", newSettings);
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
        const data = {
          minLength: passwordLen,
          upperCase: useUpperCase,
          digits: useDigits,
          specSymbols: useSpecialSymbols,
        };
        saveToSessionStorage("currentPasswordSettings", data);
        saveToSessionStorage("defaultPasswordSettings", data);
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      })
      .catch((e) => toastr.error(e));
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultPasswordSettings");
    saveToSessionStorage("currentPasswordSettings", defaultSettings);
    setPasswordLen(defaultSettings.minLength);
    setUseUpperCase(defaultSettings.upperCase);
    setUseDigits(defaultSettings.digits);
    setUseSpecialSymbols(defaultSettings.specSymbols);
    setShowReminder(false);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="learn-subtitle">
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

      <Buttons
        t={t}
        showReminder={showReminder}
        onSaveClick={onSaveClick}
        onCancelClick={onCancelClick}
      />
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
