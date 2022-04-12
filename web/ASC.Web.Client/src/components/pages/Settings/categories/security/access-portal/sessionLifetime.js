import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import toastr from "@appserver/components/toast/toastr";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

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
  const { t, history, lifetime, setSessionLifetimeSettings } = props;
  const [type, setType] = useState(false);
  const [sessionLifetime, setSessionLifetime] = useState("0");
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage(
      "currentSessionLifetimeSettings"
    );
    const defaultSettings = getFromSessionStorage(
      "defaultSessionLifetimeSettings"
    );

    if (defaultSettings) {
      saveToSessionStorage("defaultSessionLifetimeSettings", defaultSettings);
    } else {
      saveToSessionStorage("defaultSessionLifetimeSettings", {
        lifetime: lifetime.toString(),
        type: lifetime > 0 ? true : false,
      });
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
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, [isLoading]);

  useEffect(() => {
    if (!isLoading) return;

    const defaultSettings = getFromSessionStorage(
      "defaultSessionLifetimeSettings"
    );
    const newSettings = {
      lifetime: sessionLifetime,
      type: type,
    };

    saveToSessionStorage("currentSessionLifetimeSettings", newSettings);

    if (defaultSettings.type === newSettings.type) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type, sessionLifetime]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("lifetime") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectType = (e) => {
    setType(e.target.value === "enable" ? true : false);
  };

  const onChangeInput = (e) => {
    setSessionLifetime(e.target.value);
  };

  const onSaveClick = async () => {
    try {
      setSessionLifetimeSettings(sessionLifetime);
      toastr.success(t("SuccessfullySaveSettingsMessage"));
      saveToSessionStorage("defaultSessionLifetimeSettings", {
        lifetime: lifetime,
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
            isAutoFocussed={true}
            value={sessionLifetime}
            onChange={onChangeInput}
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

export default inject(({ auth }) => {
  const { sessionLifetime, setSessionLifetimeSettings } = auth.settingsStore;

  return {
    lifetime: sessionLifetime,
    setSessionLifetimeSettings,
  };
})(
  withTranslation(["Settings", "Common"])(withRouter(observer(SessionLifetime)))
);
