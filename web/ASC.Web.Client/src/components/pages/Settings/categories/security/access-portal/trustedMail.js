import React, { useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Button from "@appserver/components/button";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import { isMobile } from "react-device-detect";
import { getLanguage } from "@appserver/common/utils";
import { ButtonsWrapper } from "../StyledSecurity";
import UserFields from "../sub-components/user-fields";
import Buttons from "../sub-components/buttons";

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
  const { t } = props;
  const [type, setType] = useState("none");
  const [showReminder, setShowReminder] = useState(false);

  const onSelectDomainType = (e) => {
    if (type !== e.target.value) {
      setType(e.target.value);
      setShowReminder(true);
    }
  };

  const onSaveClick = () => {};
  const onCancelClick = () => {};

  const lng = getLanguage(localStorage.getItem("language") || "en");
  return (
    <MainContainer>
      {isMobile && (
        <>
          <Text className="page-subtitle">{t("TrustedMailHelper")}</Text>
          <Link
            className="learn-more"
            target="_blank"
            href={`https://helpcenter.onlyoffice.com/${lng}/administration/configuration.aspx#ChangingSecuritySettings_block`}
          >
            {t("Common:LearnMore")}
          </Link>
        </>
      )}

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
            label: t("AllDomains"),
            value: "any",
          },
          {
            label: t("CustomDomains"),
            value: "custom",
          },
        ]}
        selected={type}
        onClick={onSelectDomainType}
      />

      {type === "custom" && <UserFields t={t} className="user-fields" />}

      <Text
        color="#F21C0E"
        fontSize="16px"
        fontWeight="700"
        className="warning-text"
      >
        {t("Common:Warning")}
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

export default inject(({ auth }) => {
  const { setPortalPasswordSettings } = auth.settingsStore;

  return {
    setPortalPasswordSettings,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(TrustedMail))));
