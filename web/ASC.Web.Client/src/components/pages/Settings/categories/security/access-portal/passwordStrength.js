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
import { isMobile } from "react-device-detect";
import { ButtonsWrapper } from "../StyledSecurity";
import { getPortalPasswordSettings } from "@appserver/common/api/settings";

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

  @media (max-width: 375px) {
    .length-subtitle {
      margin-top: 20px;
    }
  }
`;

const PasswordStrength = (props) => {
  const { t, setPortalPasswordSettings } = props;
  const [passwordLen, setPasswordLen] = useState(8);
  const [useUpperCase, setUseUpperCase] = useState(false);
  const [useDigits, setUseDigits] = useState(false);
  const [useSpecialSymbols, setUseSpecialSymbols] = useState(false);

  const [currentState, setCurrentState] = useState([8, false, false, false]);

  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = async () => {
    const settings = await getPortalPasswordSettings();
    setCurrentState([
      settings.minLength,
      settings.upperCase,
      settings.digits,
      settings.specSymbols,
    ]);
    setPasswordLen(settings.minLength);
    setUseUpperCase(settings.upperCase);
    setUseDigits(settings.digits);
    setUseSpecialSymbols(settings.specSymbols);
    return settings;
  };

  useEffect(() => {
    getSettings();
    setIsLoading(true);
  }, []);

  const onSliderChange = (e) => {
    setPasswordLen(Number(e.target.value));
    if (
      Number(e.target.value) === currentState[0] &&
      useUpperCase === currentState[1] &&
      useDigits === currentState[2] &&
      useSpecialSymbols === currentState[3]
    )
      setShowReminder(false);
    else setShowReminder(true);
  };

  const onClickCheckbox = (e) => {
    setShowReminder(true);
    switch (e.target.value) {
      case "upperCase":
        setUseUpperCase(e.target.checked);
        e.target.checked === currentState[1] &&
          passwordLen === currentState[0] &&
          setShowReminder(false);
        break;
      case "digits":
        setUseDigits(e.target.checked);
        e.target.checked === currentState[2] &&
          passwordLen === currentState[0] &&
          setShowReminder(false);
        break;
      case "special":
        setUseSpecialSymbols(e.target.checked);
        e.target.checked === currentState[3] &&
          passwordLen === currentState[0] &&
          setShowReminder(false);
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
        console.log("Success");
      })
      .catch((e) => console.error(e));
  };

  const onCancelClick = () => {
    setShowReminder(false);
    setPasswordLen(currentState[0]);
    setUseUpperCase(currentState[1]);
    setUseDigits(currentState[2]);
    setUseSpecialSymbols(currentState[3]);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");
  if (!isLoading) return <SectionLoader />;
  return (
    <MainContainer>
      {isMobile && (
        <>
          <Text className="page-subtitle">
            {t("SettingPasswordStrengthHelper")}
          </Text>
          <Link
            className="learn-more"
            target="_blank"
            href={`https://helpcenter.onlyoffice.com/${lng}/administration/configuration.aspx#ChangingSecuritySettings_block`}
          >
            {t("Common:LearnMore")}
          </Link>
        </>
      )}

      <Text fontSize="14px" fontWeight="600" className="length-subtitle">
        {t("PasswordMinLenght")}
      </Text>

      <Box displayProp="flex" flexDirection="row" alignItems="center">
        <Slider
          className="password-slider"
          min="8"
          max="30"
          step="1"
          value={passwordLen}
          onChange={onSliderChange}
        />
        <Text>
          {passwordLen} {t("Characters")}
        </Text>
      </Box>

      <Box className="checkboxes">
        <Checkbox
          onChange={onClickCheckbox}
          label={`${t("Use")} ${t("Common:PasswordLimitUpperCase")}`}
          isChecked={useUpperCase}
          value="upperCase"
        />
        <Checkbox
          onChange={onClickCheckbox}
          label={`${t("Use")} ${t("Common:PasswordLimitDigits")}`}
          isChecked={useDigits}
          value="digits"
        />
        <Checkbox
          onChange={onClickCheckbox}
          label={`${t("Use")} ${t("Common:PasswordLimitSpecialSymbols")}`}
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
  const { setPortalPasswordSettings } = auth.settingsStore;

  return {
    setPortalPasswordSettings,
  };
})(
  withTranslation(["Settings", "Common"])(
    withRouter(observer(PasswordStrength))
  )
);
