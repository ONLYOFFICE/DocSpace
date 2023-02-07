import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@docspace/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { isMobile } from "react-device-detect";
import TfaLoader from "../sub-components/loaders/tfa-loader";

const MainContainer = styled.div`
  width: 100%;

  .box {
    margin-bottom: 24px;
  }
`;

const TwoFactorAuth = (props) => {
  const { t, history, initSettings, isInit, setIsInit, helpLink } = props;
  const [type, setType] = useState("none");

  const [smsDisabled, setSmsDisabled] = useState(false);
  const [appDisabled, setAppDisabled] = useState(false);
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const getSettings = () => {
    const { tfaSettings, smsAvailable, appAvailable } = props;
    const currentSettings = getFromSessionStorage("currentTfaSettings");

    saveToSessionStorage("defaultTfaSettings", tfaSettings);

    if (currentSettings) {
      setType(currentSettings);
    } else {
      setType(tfaSettings);
    }

    setSmsDisabled(smsAvailable);
    setAppDisabled(appAvailable);
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
      history.push("/portal-settings/security/access-portal");
  };

  const onSelectTfaType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
    }
  };

  const onSaveClick = async () => {
    const { t, setTfaSettings, getTfaConfirmLink, history } = props;

    setIsSaving(true);

    try {
      const res = await setTfaSettings(type);

      toastr.success(t("SuccessfullySaveSettingsMessage"));
      saveToSessionStorage("defaultTfaSettings", type);
      setIsSaving(false);
      setShowReminder(false);

      if (res) {
        setIsInit(false);
        history.push(res.replace(window.location.origin, ""));
      }
    } catch (error) {
      toastr.error(error);
    }
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultTfaSettings");
    setType(defaultSettings);
    setShowReminder(false);
  };

  if (isMobile && !isInit && !isLoading) {
    return <TfaLoader />;
  }

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="learn-subtitle">{t("TwoFactorAuthHelper")}</Text>
        <Link
          color="#316DAA"
          target="_blank"
          isHovered
          href={`${helpLink}/administration/two-factor-authentication.aspx`}
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
    setTfaSettings,
    getTfaConfirmLink,
    tfaSettings,
    smsAvailable,
    appAvailable,
  } = auth.tfaStore;

  const { isInit, initSettings, setIsInit } = setup;
  const { helpLink } = auth.settingsStore;

  return {
    setTfaSettings,
    getTfaConfirmLink,
    tfaSettings,
    smsAvailable,
    appAvailable,
    isInit,
    initSettings,
    setIsInit,
    helpLink,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(TwoFactorAuth)))
);
