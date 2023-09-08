import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useNavigate, useLocation } from "react-router-dom";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import toastr from "@docspace/components/toast/toastr";
import { LearnMoreWrapper } from "../StyledSecurity";
import { size } from "@docspace/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import isEqual from "lodash/isEqual";
import { isMobile } from "react-device-detect";
import AdmMsgLoader from "../sub-components/loaders/admmsg-loader";

const MainContainer = styled.div`
  width: 100%;

  .page-subtitle {
    margin-bottom: 10px;
  }

  .box {
    margin-bottom: 24px;
  }
`;

const AdminMessage = (props) => {
  const {
    t,

    enableAdmMess,
    setMessageSettings,
    initSettings,
    isInit,
    currentColorScheme,
    administratorMessageSettingsUrl,
  } = props;
  const [type, setType] = useState("");
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const getSettings = () => {
    const currentSettings = getFromSessionStorage(
      "currentAdminMessageSettings"
    );

    const enable = enableAdmMess ? "enable" : "disabled";

    saveToSessionStorage("defaultAdminMessageSettings", enable);

    if (currentSettings) {
      setType(currentSettings);
    } else {
      setType(enable);
    }
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
      "defaultAdminMessageSettings"
    );
    saveToSessionStorage("currentAdminMessageSettings", type);

    if (isEqual(defaultSettings, type)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      location.pathname.includes("admin-message") &&
      navigate("/portal-settings/security/access-portal");
  };

  const onSelectType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
    }
  };

  const onSaveClick = () => {
    const turnOn = type === "enable" ? true : false;
    setMessageSettings(turnOn);
    toastr.success(t("SuccessfullySaveSettingsMessage"));
    saveToSessionStorage("defaultAdminMessageSettings", type);
    setShowReminder(false);
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage(
      "defaultAdminMessageSettings"
    );
    setType(defaultSettings);
    setShowReminder(false);
  };

  if (isMobile && !isInit && !isLoading) {
    return <AdmMsgLoader />;
  }

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text>{t("AdminsMessageSettingDescription")}</Text>
        <Text fontSize="13px" fontWeight="400" className="learn-subtitle">
          <Trans t={t} i18nKey="AdminsMessageSave" />
        </Text>

        <Link
          className="link-learn-more"
          color={currentColorScheme.main.accent}
          target="_blank"
          isHovered
          href={administratorMessageSettingsUrl}
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
            id: "admin-message-disabled",
            label: t("Disabled"),
            value: "disabled",
          },
          {
            id: "admin-message-enable",
            label: t("Common:Enable"),
            value: "enable",
          },
        ]}
        selected={type}
        onClick={onSelectType}
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
        additionalClassSaveButton="admin-message-save"
        additionalClassCancelButton="admin-message-cancel"
      />
    </MainContainer>
  );
};

export default inject(({ auth, setup }) => {
  const {
    enableAdmMess,
    setMessageSettings,
    currentColorScheme,
    administratorMessageSettingsUrl,
  } = auth.settingsStore;
  const { initSettings, isInit } = setup;

  return {
    enableAdmMess,
    setMessageSettings,
    initSettings,
    isInit,
    currentColorScheme,
    administratorMessageSettingsUrl,
  };
})(withTranslation(["Settings", "Common"])(observer(AdminMessage)));
