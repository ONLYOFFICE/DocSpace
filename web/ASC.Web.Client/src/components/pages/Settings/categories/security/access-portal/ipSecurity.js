import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import toastr from "@appserver/components/toast/toastr";
import { LearnMoreWrapper } from "../StyledSecurity";
import UserFields from "../sub-components/user-fields";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";

const MainContainer = styled.div`
  width: 100%;

  .page-subtitle {
    margin-bottom: 10px;
  }

  .user-fields {
    margin-bottom: 18px;
  }

  .box {
    margin-bottom: 11px;
  }

  .warning-text {
    margin-bottom: 9px;
  }

  .warning-helper {
    margin-bottom: 24px;
  }
`;

const IpSecurity = (props) => {
  const {
    t,
    history,
    ipRestrictionEnabled,
    setIpRestrictionsEnable,
    ipRestrictions,
    setIpRestrictions,
  } = props;

  const regexp = /(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))|((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])/; //check ip valid

  const [enable, setEnable] = useState(false);
  const [ips, setIps] = useState();
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentIPSettings");
    const defaultSettings = getFromSessionStorage("defaultIPSettings");

    if (defaultSettings) {
      saveToSessionStorage("defaultIPSettings", defaultSettings);
    } else {
      const defaultData = {
        enable: ipRestrictionEnabled,
        ips: ipRestrictions,
      };
      saveToSessionStorage("defaultIPSettings", defaultData);
    }

    if (currentSettings) {
      setEnable(currentSettings.enable);
      setIps(currentSettings.ips);
    } else {
      setEnable(ipRestrictionEnabled);
      setIps(ipRestrictions);
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

    const defaultSettings = getFromSessionStorage("defaultIPSettings");
    const newSettings = {
      enable: enable,
      ips: ips,
    };
    saveToSessionStorage("currentIPSettings", newSettings);

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [enable, ips]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("ip") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectType = (e) => {
    setEnable(e.target.value === "enable" ? true : false);
  };

  const onChangeInput = (e, index) => {
    let newInputs = Array.from(ips);
    newInputs[index] = e.target.value;
    setIps(newInputs);
  };

  const onDeleteInput = (index) => {
    let newInputs = Array.from(ips);
    newInputs.splice(index, 1);
    setIps(newInputs);
  };

  const onClickAdd = () => {
    setIps([...ips, ""]);
  };

  const onSaveClick = () => {
    const valid = ips.map((ip) => regexp.test(ip));
    if (valid.includes(false)) {
      return;
    }

    setIpRestrictions(ips);
    setIpRestrictionsEnable(enable);

    saveToSessionStorage("defaultIPSettings", {
      enable: enable,
      ips: ips,
    });
    setShowReminder(false);
    toastr.success(t("SuccessfullySaveSettingsMessage"));
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultIPSettings");
    setEnable(defaultSettings.enable);
    setIps(defaultSettings.ips);
    setShowReminder(false);
  };

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="page-subtitle">{t("IPSecurityHelper")}</Text>
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
        selected={enable ? "enable" : "disabled"}
        onClick={onSelectType}
      />

      {enable && (
        <UserFields
          className="user-fields"
          inputs={ips}
          buttonLabel={t("AddAllowedIP")}
          onChangeInput={onChangeInput}
          onDeleteInput={onDeleteInput}
          onClickAdd={onClickAdd}
          regexp={regexp}
        />
      )}

      <Text
        color="#F21C0E"
        fontSize="16px"
        fontWeight="700"
        className="warning-text"
      >
        {t("Common:Warning")}!
      </Text>
      <Text className="warning-helper">{t("IPSecurityWarningHelper")}</Text>

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
  const {
    ipRestrictionEnabled,
    setIpRestrictionsEnable,
    ipRestrictions,
    setIpRestrictions,
  } = auth.settingsStore;

  return {
    ipRestrictionEnabled,
    setIpRestrictionsEnable,
    ipRestrictions,
    setIpRestrictions,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(IpSecurity))));
