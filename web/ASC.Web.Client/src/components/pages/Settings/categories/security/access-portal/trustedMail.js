import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import { LearnMoreWrapper } from "../StyledSecurity";
import { getLanguage } from "@appserver/common/utils";
import toastr from "@appserver/components/toast/toastr";
import UserFields from "../sub-components/user-fields";
import Buttons from "../sub-components/buttons";
import { size } from "@appserver/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";

const MainContainer = styled.div`
  width: 100%;

  .user-fields {
    margin-bottom: 18px;
  }

  .box {
    margin-bottom: 11px;
  }

  .warning-text {
    margin-bottom: 9px;
  }
`;

const TrustedMail = (props) => {
  const {
    t,
    history,
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
  } = props;

  const regexp = /^[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9](?:\.[a-zA-Z]{1,})+/; //check domain name valid

  const [type, setType] = useState("0");
  const [domains, setDomains] = useState([]);
  const [showReminder, setShowReminder] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentTrustedMailSettings");
    const defaultSettings = getFromSessionStorage("defaultTrustedMailSettings");

    if (defaultSettings) {
      saveToSessionStorage("defaultTrustedMailSettings", defaultSettings);
    } else {
      const defaultData = {
        type: String(trustedDomainsType),
        domains: trustedDomains,
      };
      saveToSessionStorage("defaultTrustedMailSettings", defaultData);
    }

    if (currentSettings) {
      setType(currentSettings.type);
      setDomains(currentSettings.domains);
    } else {
      setType(String(trustedDomainsType));
      setDomains(trustedDomains);
    }
  };

  useEffect(() => {
    checkWidth();
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  useEffect(() => {
    const defaultSettings = getFromSessionStorage("defaultTrustedMailSettings");
    const newSettings = {
      type: type,
      domains: domains,
    };
    saveToSessionStorage("currentTrustedMailSettings", newSettings);

    if (isEqual(defaultSettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [type, domains]);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("trusted-mail") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectDomainType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
    }
  };

  const onClickAdd = () => {
    setDomains([...domains, ""]);
  };

  const onChangeInput = (e, index) => {
    let newInputs = Array.from(domains);
    newInputs[index] = e.target.value;
    setDomains(newInputs);
  };

  const onDeleteInput = (index) => {
    let newInputs = Array.from(domains);
    newInputs.splice(index, 1);
    setDomains(newInputs);
  };

  const onSaveClick = async () => {
    setIsSaving(true);
    const valid = domains.map((domain) => regexp.test(domain));
    if (type === "1" && valid.includes(false)) {
      toastr.error(t("Common:IncorrectDomain"));
      return;
    }

    try {
      const data = {
        type: Number(type),
        domains: domains,
        inviteUsersAsVisitors: true,
      };
      await setMailDomainSettings(data);
      saveToSessionStorage("defaultTrustedMailSettings", {
        type: type,
        domains: domains,
      });
      setShowReminder(false);
      toastr.success(t("SuccessfullySaveSettingsMessage"));
    } catch (error) {
      toastr.error(error);
    }

    setIsSaving(false);
  };

  const onCancelClick = () => {
    const defaultSettings = getFromSessionStorage("defaultTrustedMailSettings");
    setType(defaultSettings.type);
    setDomains(defaultSettings.domains);
    setShowReminder(false);
  };

  const lng = getLanguage(localStorage.getItem("language") || "en");
  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="learn-subtitle">{t("TrustedMailHelper")}</Text>
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
            value: "0",
          },
          {
            label: t("AllDomains"),
            value: "2",
          },
          {
            label: t("CustomDomains"),
            value: "1",
          },
        ]}
        selected={type}
        onClick={onSelectDomainType}
      />

      {type === "1" && (
        <UserFields
          className="user-fields"
          inputs={domains}
          buttonLabel={t("AddTrustedDomain")}
          onChangeInput={onChangeInput}
          onDeleteInput={onDeleteInput}
          onClickAdd={onClickAdd}
          regexp={regexp}
        />
      )}

      <Buttons
        t={t}
        showReminder={showReminder}
        onSaveClick={onSaveClick}
        onCancelClick={onCancelClick}
        isLoading={isSaving}
      />
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const {
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
  } = auth.settingsStore;

  return {
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(TrustedMail))));
