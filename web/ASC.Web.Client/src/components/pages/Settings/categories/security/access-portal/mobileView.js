import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer } from "../StyledSecurity";
import MobileCategoryWrapper from "../sub-components/mobile-category-wrapper";

const MobileView = (props) => {
  const { t, history } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalAccess"));
  }, []);

  const onClickLink = (e) => {
    e.preventDefault();
    history.push(e.target.pathname);
  };

  return (
    <MainContainer>
      <MobileCategoryWrapper
        title={t("SettingPasswordStrength")}
        subtitle={t("SettingPasswordStrengthDescription")}
        url="/settings/security/access-portal/password"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("TwoFactorAuth")}
        subtitle={t("TwoFactorAuthDescription")}
        url="/settings/security/access-portal/tfa"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("TrustedMail")}
        subtitle={t("TrustedMailDescription")}
        url="/settings/security/access-portal/trusted-mail"
        onClickLink={onClickLink}
      />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(MobileView));
