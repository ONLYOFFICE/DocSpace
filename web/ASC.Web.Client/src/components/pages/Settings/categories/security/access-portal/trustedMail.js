import React, { useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Button from "@appserver/components/button";
import { isMobile } from "react-device-detect";
import { getLanguage } from "@appserver/common/utils";
import { ButtonsWrapper } from "../StyledSecurity";
import UserFields from "../sub-components/user-fields";

const MainContainer = styled.div`
  width: 100%;

  .page-subtitle {
    margin-bottom: 10px;
  }

  .user-fields {
    margin-bottom: 24px;
  }
`;

const TrustedMail = (props) => {
  const { t } = props;
  const [showReminder, setShowReminder] = useState(false);

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

      <UserFields />

      <ButtonsWrapper>
        <Button
          label={t("Common:SaveButton")}
          size="small"
          primary={true}
          className="button"
          //onClick={onSaveClick}
          isDisabled={!showReminder}
        />
        <Button
          label={t("Common:CancelButton")}
          size="small"
          className="button"
          //onClick={onCancelClick}
          isDisabled={!showReminder}
        />
        {showReminder && (
          <Text
            color="#A3A9AE"
            fontSize="12px"
            fontWeight="600"
            className="reminder"
          >
            {t("YouHaveUnsavedChanges")}
          </Text>
        )}
      </ButtonsWrapper>
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const { setPortalPasswordSettings } = auth.settingsStore;

  return {
    setPortalPasswordSettings,
  };
})(withTranslation(["Settings", "Common"])(withRouter(observer(TrustedMail))));
