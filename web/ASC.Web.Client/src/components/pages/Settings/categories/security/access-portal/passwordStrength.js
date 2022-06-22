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
import { LearnMoreWrapper } from "../StyledSecurity";
import toastr from "@appserver/components/toast/toastr";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

const MainContainer = styled.div`
  width: 100%;

  .password-slider {
    width: 160px;
    height: 8px;
    margin: 24px 16px 24px 0px;
  }

  .checkboxes {
    display: inline-block;
    margin-top: 18px;
    margin-bottom: 24px;

    .second-checkbox {
      margin: 8px 0;
    }
  }
`;

const PasswordStrength = (props) => {
  const {
    t,
    history,
    setPortalPasswordSettings,
    passwordSettings,
    initSettings,
    isInit,
    helpLink,
  } = props;

  const [passwordLen, setPasswordLen] = useState(8);
  const [useUpperCase, setUseUpperCase] = useState(false);
  const [useDigits, setUseDigits] = useState(false);
  const [useSpecialSymbols, setUseSpecialSymbols] = useState(false);

  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentPasswordSettings");

    const defaultData = {
      minLength: passwordSettings.minLength,
      upperCase: passwordSettings.upperCase,
      digits: passwordSettings.digits,
      specSymbols: passwordSettings.specSymbols,
    };
    saveToSessionStorage("defaultPasswordSettings", defaultData);

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
  };

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);

    if (!isInit) initSettings().then(() => setIsLoading(true));
    else setIsLoading(true);

    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  useEffect(() => {
    if (!isInit) return;
    getSettings();
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

  const onSaveClick = async () => {
    setIsSaving(true);

    try {
      const data = {
        minLength: passwordLen,
        upperCase: useUpperCase,
        digits: useDigits,
        specSymbols: useSpecialSymbols,
      };
      await setPortalPasswordSettings(
        passwordLen,
        useUpperCase,
        useDigits,
        useSpecialSymbols
      );
      setShowReminder(false);
      saveToSessionStorage("currentPasswordSettings", data);
      saveToSessionStorage("defaultPasswordSettings", data);
      toastr.success(t("SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(e);
    }

    setIsSaving(false);
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
          href={`${helpLink}/administration/configuration.aspx#ChangingSecuritySettings_block`}
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
          className="second-checkbox"
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

      <SaveCancelButtons
        className="save-cancel-buttons"
        onSaveClick={onSaveClick}
        onCancelClick={onCancelClick}
        showReminder={showReminder}
        reminderTest={t("YouHaveUnsavedChanges")}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Common:CancelButton")}
        displaySettings={true}
        hasScroll={false}
        isSaving={isSaving}
      />
    </MainContainer>
  );
};

export default inject(({ auth, setup }) => {
  const {
    setPortalPasswordSettings,
    passwordSettings,
    helpLink,
  } = auth.settingsStore;
  const { initSettings, isInit } = setup;

  return {
    setPortalPasswordSettings,
    passwordSettings,
    initSettings,
    isInit,
    helpLink,
  };
})(
  withTranslation(["Settings", "Common"])(
    withRouter(observer(PasswordStrength))
  )
);
