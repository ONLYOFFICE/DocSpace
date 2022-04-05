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
`;

const TrustedMail = (props) => {
  const {
    t,
    history,
    trustedDomainsType,
    trustedDomains,
    setMailDomainSettings,
  } = props;

  const [type, setType] = useState("0");
  const [domains, setDomains] = useState([]);
  const [showReminder, setShowReminder] = useState(false);

  const getSettings = async () => {
    setType(String(trustedDomainsType));
    setDomains(trustedDomains);
  };

  useEffect(() => {
    checkWidth();
    getSettings();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("trusted-mail") &&
      history.push("/settings/security/access-portal");
  };

  const onSelectDomainType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
      setShowReminder(true);
    }
  };

  const onClickAdd = () => {
    setDomains([...domains, ""]);
    setShowReminder(true);
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
    setShowReminder(true);
  };

  const onSaveClick = () => {
    const data = {
      type: Number(type),
      domains: domains,
      inviteUsersAsVisitors: true,
    };
    setMailDomainSettings(data);
    setShowReminder(false);
    toastr.success(t("SuccessfullySaveSettingsMessage"));
  };

  const onCancelClick = () => {};

  const lng = getLanguage(localStorage.getItem("language") || "en");
  return (
    <MainContainer>
      <LearnMoreWrapper>
        <Text className="page-subtitle">{t("TrustedMailHelper")}</Text>
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
      <Text>{t("TrustedMailWarningHelper")}</Text>

      <Buttons
        t={t}
        showReminder={showReminder}
        onSaveClick={onSaveClick}
        onCancelClick={onCancelClick}
      />
    </MainContainer>
  );
};

export default inject(({ auth, setup }) => {
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
