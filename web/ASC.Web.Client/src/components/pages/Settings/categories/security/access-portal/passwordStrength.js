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
import SectionLoader from "../sub-components/section-loader";
import { getLanguage } from "@appserver/common/utils";
import { ButtonsWrapper, LearnMoreWrapper } from "../StyledSecurity";
import toastr from "@appserver/components/toast/toastr";
import Buttons from "../sub-components/buttons";
import { size } from "@appserver/components/utils/device";

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
    setCurrentState([
      passwordSettings.minLength,
      passwordSettings.upperCase,
      passwordSettings.digits,
      passwordSettings.specSymbols,
    ]);
    setPasswordLen(passwordSettings.minLength);
    setUseUpperCase(passwordSettings.upperCase);
    setUseDigits(passwordSettings.digits);
    setUseSpecialSymbols(passwordSettings.specSymbols);
    setIsLoading(true);
  };

  useEffect(() => {
    checkWidth();
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("password") &&
      history.push("/settings/security/access-portal");
  };

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
        toastr.success(t("SuccessfullySaveSettingsMessage"));
      })
      .catch((e) => toastr.error(e));
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
