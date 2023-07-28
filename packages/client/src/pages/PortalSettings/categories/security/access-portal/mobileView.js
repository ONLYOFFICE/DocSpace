import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Trans, withTranslation } from "react-i18next";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { MainContainer } from "../StyledSecurity";
import MobileCategoryWrapper from "../sub-components/mobile-category-wrapper";

const MobileView = (props) => {
  const { t } = props;

  const navigate = useNavigate();

  useEffect(() => {
    setDocumentTitle(t("PortalAccess"));
  }, []);

  const onClickLink = (e) => {
    e.preventDefault();
    navigate(e.target.pathname);
  };

  return (
    <MainContainer>
      <MobileCategoryWrapper
        title={t("SettingPasswordStrength")}
        subtitle={
          <Trans
            i18nKey="SettingPasswordStrengthDescription"
            ns="Settings"
            t={t}
          />
        }
        url="/portal-settings/security/access-portal/password"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("TwoFactorAuth")}
        subtitle={
          <Trans i18nKey="TwoFactorAuthDescription" ns="Settings" t={t} />
        }
        url="/portal-settings/security/access-portal/tfa"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("TrustedMail")}
        subtitle={
          <Trans i18nKey="TrustedMailDescription" ns="Settings" t={t} />
        }
        url="/portal-settings/security/access-portal/trusted-mail"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("IPSecurity")}
        subtitle={<Trans i18nKey="IPSecurityDescription" ns="Settings" t={t} />}
        url="/portal-settings/security/access-portal/ip"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("AdminsMessage")}
        subtitle={
          <Trans i18nKey="AdminsMessageDescription" ns="Settings" t={t} />
        }
        url="/portal-settings/security/access-portal/admin-message"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("SessionLifetime")}
        subtitle={
          <Trans i18nKey="SessionLifetimeDescription" ns="Settings" t={t} />
        }
        url="/portal-settings/security/access-portal/lifetime"
        onClickLink={onClickLink}
      />
    </MainContainer>
  );
};

export default withTranslation("Settings")(MobileView);
