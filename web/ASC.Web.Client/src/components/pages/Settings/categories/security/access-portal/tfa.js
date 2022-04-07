import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import toastr from "@appserver/components/toast/toastr";
import { getLanguage } from "@appserver/common/utils";
import Buttons from "../sub-components/buttons";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";

const MainContainer = styled.div`
  width: 100%;
`;

const TwoFactorAuth = (props) => {
  const { t, history } = props;
  const [type, setType] = useState("none");

  const [smsDisabled, setSmsDisabled] = useState(false);
  const [appDisabled, setAppDisabled] = useState(false);
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const { tfaSettings, smsAvailable, appAvailable } = props;
    const currentSettings = getFromSessionStorage("currentTfaSettings");
    const defaultSettings = getFromSessionStorage("defaultTfaSettings");

    if (defaultSettings) {
      saveToSessionStorage("defaultTfaSettings", defaultSettings);
    } else {
      saveToSessionStorage("defaultTfaSettings", tfaSettings);
    }

    if (currentSettings) {
      setType(currentSettings);
    } else {
      setType(tfaSettings);
    }

    setSmsDisabled(smsAvailable);
    setAppDisabled(appAvailable);
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

    const defaultSettings = getFromSessionStorage("defaultTfaSettings");
    saveToSessionStorage("currentTfaSettings", type);

    if (defaultSettings === type) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("tfa") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectTfaType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
    }
  };

  const onSaveClick = () => {
    const { t, setTfaSettings, getTfaConfirmLink, history } = props;

    setTfaSettings(type).then((res) => {
      toastr.success(t("SuccessfullySaveSettingsMessage"));
      if (type !== "none") {
        getTfaConfirmLink(res).then((link) =>
          history.push(link.replace(window.location.origin, ""))
        );
      }
      setType(type);
      saveToSessionStorage("defaultTfaSettings", type);

      setShowReminder(false);
    });
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultTfaSettings");
    setType(defaultSettings);
    setShowReminder(false);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");
  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="learn-subtitle">{t("TwoFactorAuthHelper")}</Text>
        <Link
          color="#316DAA"
          target="_blank"
          isHovered
          href={`https://helpcenter.onlyoffice.com/${lng}/administration/two-factor-authentication.aspx`}
        >
          {t("Common:LearnMore")}
        </Link>
      </LearnMoreWrapper>

      <RadioButtonGroup
        className="box"
        fontSize="13px"
        fontWeight="400"
        name="group"
        orientation="vertical"
        spacing="8px"
        options={[
          {
            label: t("Disabled"),
            value: "none",
          },
          {
            label: t("BySms"),
            value: "sms",
            disabled: !smsDisabled,
          },
          {
            label: t("ByApp"),
            value: "app",
            disabled: !appDisabled,
          },
        ]}
        selected={type}
        onClick={onSelectTfaType}
      />

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
  const {
    setTfaSettings,
    getTfaConfirmLink,
    tfaSettings,
    smsAvailable,
    appAvailable,
  } = auth.tfaStore;

  return {
    setTfaSettings,
    getTfaConfirmLink,
    tfaSettings,
    smsAvailable,
    appAvailable,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(TwoFactorAuth)))
);
