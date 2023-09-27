import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useLocation, useNavigate } from "react-router-dom";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import { LearnMoreWrapper } from "../StyledSecurity";
import toastr from "@docspace/components/toast/toastr";
import UserFields from "../sub-components/user-fields";
import { size } from "@docspace/components/utils/device";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import isEqual from "lodash/isEqual";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { isMobile } from "react-device-detect";
import TrustedMailLoader from "../sub-components/loaders/trusted-mail-loader";

const MainContainer = styled.div`
  width: 100%;

  .box {
    margin-bottom: 11px;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

const TrustedMail = (props) => {
  const {
    t,

    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
    currentColorScheme,
    trustedMailDomainSettingsUrl,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const regexp = /^[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9](?:\.[a-zA-Z]{1,})+/; //check domain name valid

  const [type, setType] = useState("0");
  const [domains, setDomains] = useState([]);
  const [showReminder, setShowReminder] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const getSettings = () => {
    const currentSettings = getFromSessionStorage("currentTrustedMailSettings");

    const defaultData = {
      type: String(trustedDomainsType),
      domains: trustedDomains,
    };
    saveToSessionStorage("defaultTrustedMailSettings", defaultData);

    if (currentSettings) {
      setType(currentSettings.type);
      setDomains(currentSettings.domains);
    } else {
      setType(String(trustedDomainsType));
      setDomains(trustedDomains);
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
      location.pathname.includes("trusted-mail") &&
      navigate("/portal-settings/security/access-portal");
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
      setIsSaving(false);
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

  if (isMobile && !isLoading) {
    return <TrustedMailLoader />;
  }

  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text fontSize="13px" fontWeight="400">
          {t("TrustedMailSettingDescription")}
        </Text>
        <Text fontSize="13px" fontWeight="400" className="learn-subtitle">
          <Trans t={t} i18nKey="TrustedMailSave" />
        </Text>
        <Link
          className="link-learn-more"
          color={currentColorScheme.main.accent}
          target="_blank"
          isHovered
          href={trustedMailDomainSettingsUrl}
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
            id: "trusted-mail-disabled",
            label: t("Disabled"),
            value: "0",
          },
          {
            id: "any-domains",
            label: t("AllDomains"),
            value: "2",
          },
          {
            id: "custom-domains",
            label: t("CustomDomains"),
            value: "1",
          },
        ]}
        selected={type}
        onClick={onSelectDomainType}
      />

      {type === "1" && (
        <UserFields
          inputs={domains}
          buttonLabel={t("AddTrustedDomain")}
          onChangeInput={onChangeInput}
          onDeleteInput={onDeleteInput}
          onClickAdd={onClickAdd}
          regexp={regexp}
          classNameAdditional="add-trusted-domain"
        />
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
        isSaving={isSaving}
        additionalClassSaveButton="trusted-mail-save"
        additionalClassCancelButton="trusted-mail-cancel"
      />
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const {
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
    helpLink,
    currentColorScheme,
    trustedMailDomainSettingsUrl,
  } = auth.settingsStore;

  return {
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
    helpLink,
    currentColorScheme,
    trustedMailDomainSettingsUrl,
  };
})(withTranslation(["Settings", "Common"])(observer(TrustedMail)));
