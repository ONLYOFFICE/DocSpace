import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import Text from "@docspace/components/text";
import TextInput from "@docspace/components/text-input";
import toastr from "@docspace/components/toast/toastr";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@docspace/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import isEqual from "lodash/isEqual";
import { isMobile } from "react-device-detect";
import SessionLifetimeLoader from "../sub-components/loaders/session-lifetime-loader";

const MainContainer = styled.div`
  width: 100%;

  .lifetime {
    margin-top: 16px;
    margin-bottom: 8px;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

const SessionLifetime = (props) => {
  const {
    t,
    history,
    lifetime,
    setSessionLifetimeSettings,
    initSettings,
    isInit,
  } = props;
  const [type, setType] = useState(false);
  const [sessionLifetime, setSessionLifetime] = useState("0");
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage(
      "currentSessionLifetimeSettings"
    );
    const defaultData = {
      lifetime: lifetime.toString(),
      type: lifetime > 0 ? true : false,
    };
    saveToSessionStorage("defaultSessionLifetimeSettings", defaultData);

    if (currentSettings) {
      setSessionLifetime(currentSettings.lifetime);
      setType(currentSettings.type);
    } else {
      setSessionLifetime(lifetime.toString());
      setType(lifetime > 0 ? true : false);
    }

    if (currentSettings) {
      setType(currentSettings.type);
      setSessionLifetime(currentSettings.lifetime);
    } else {
      setType(lifetime > 0 ? true : false);
      setSessionLifetime(lifetime.toString());
    }
    setIsLoading(true);
  };

  useEffect(() => {
    checkWidth();

    if (!isInit) initSettings().then(() => setIsLoading(true));
    else setIsLoading(true);

    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  useEffect(() => {
    if (!isInit) return;
    getSettings();
  }, [isLoading]);

  useEffect(() => {
    if (!isLoading) return;

    const defaultSettings = getFromSessionStorage(
      "defaultSessionLifetimeSettings"
    );
    const newSettings = {
      lifetime: type === false ? "0" : sessionLifetime,
      type: type,
    };

    saveToSessionStorage("currentSessionLifetimeSettings", newSettings);

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type, sessionLifetime]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("lifetime") &&
      history.push("/portal-settings/security/access-portal");
  };

  const onSelectType = (e) => {
    setType(e.target.value === "enable" ? true : false);
  };

  const onChangeInput = (e) => {
    setSessionLifetime(e.target.value);
  };

  const onBlurInput = () => {
    !sessionLifetime ? setError(true) : setError(false);
  };

  const onFocusInput = () => {
    setError(false);
  };

  const onSaveClick = async () => {
    if (error) return;
    try {
      const lft = type === false ? "0" : sessionLifetime;
      setSessionLifetimeSettings(lft);
      toastr.success(t("SuccessfullySaveSettingsMessage"));
      saveToSessionStorage("defaultSessionLifetimeSettings", {
        lifetime: lft,
        type: type,
      });
      setShowReminder(false);
    } catch (error) {
      toastr.error(error);
    }
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage(
      "defaultSessionLifetimeSettings"
    );
    setType(defaultSettings.type);
    setSessionLifetime(defaultSettings.lifetime);
    setShowReminder(false);
  };

  if (isMobile && !isInit && !isLoading) {
    return <SessionLifetimeLoader />;
  }

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text>{t("SessionLifetimeHelper")}</Text>
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
            value: "disabled",
          },
          {
            label: t("Common:Enable"),
            value: "enable",
          },
        ]}
        selected={type ? "enable" : "disabled"}
        onClick={onSelectType}
      />

      {type && (
        <>
          <Text className="lifetime" fontSize="15px" fontWeight="600">
            {t("Lifetime")}
          </Text>
          <TextInput
            isAutoFocussed={false}
            value={sessionLifetime}
            onChange={onChangeInput}
            onBlur={onBlurInput}
            onFocus={onFocusInput}
            hasError={error}
          />
        </>
      )}

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
      />
    </MainContainer>
  );
};

export default inject(({ auth, setup }) => {
  const { sessionLifetime, setSessionLifetimeSettings } = auth.settingsStore;
  const { initSettings, isInit } = setup;

  return {
    lifetime: sessionLifetime,
    setSessionLifetimeSettings,
    initSettings,
    isInit,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(SessionLifetime)))
);
