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

  .page-subtitle {
    margin-bottom: 10px;
  }
`;

const AdminMessage = (props) => {
  const { t, history, enableAdmMess, setMessageSettings } = props;
  const [type, setType] = useState(false);
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage(
      "currentAdminMessageSettings"
    );
    const defaultSettings = getFromSessionStorage(
      "defaultAdminMessageSettings"
    );

    if (defaultSettings) {
      saveToSessionStorage("defaultAdminMessageSettings", defaultSettings);
    } else {
      saveToSessionStorage("defaultAdminMessageSettings", enableAdmMess);
    }

    if (currentSettings) {
      setType(currentSettings);
    } else {
      setType(enableAdmMess);
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
      "defaultAdminMessageSettings"
    );
    saveToSessionStorage("currentAdminMessageSettings", type);

    if (defaultSettings === type) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("admin-message") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectType = (e) => {
    setType(e.target.value === "enable" ? true : false);
  };

  const onSaveClick = () => {
    setMessageSettings(type);
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

  const lng = getLanguage(localStorage.getItem("language") || "en");

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="page-subtitle">{t("AdminsMessageHelper")}</Text>
        <Link
          color="#316DAA"
          target="_blank"
          isHovered
          href={`https://helpcenter.onlyoffice.com/${lng}/administration/configuration.aspx#ChangingSecuritySettings_block`}
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
  const { enableAdmMess, setMessageSettings } = auth.settingsStore;

  return {
    enableAdmMess,
    setMessageSettings,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(AdminMessage))));
