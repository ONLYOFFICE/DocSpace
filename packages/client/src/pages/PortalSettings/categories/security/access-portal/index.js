import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "./tfa";
import PasswordStrengthSection from "./passwordStrength";
import TrustedMailSection from "./trustedMail";
import IpSecuritySection from "./ipSecurity";
import AdminMessageSection from "./adminMessage";
import SessionLifetimeSection from "./sessionLifetime";
import MobileView from "./mobileView";
import CategoryWrapper from "../sub-components/category-wrapper";
import StyledSettingsSeparator from "SRC_DIR/pages/PortalSettings/StyledSettingsSeparator";
import { size } from "@docspace/components/utils/device";
import { inject, observer } from "mobx-react";

const AccessPortal = (props) => {
  const {
    t,
    currentColorScheme,
    passwordStrengthSettingsUrl,
    tfaSettingsUrl,
    trustedMailDomainSettingsUrl,
    administratorMessageSettingsUrl,
  } = props;
  const [isMobileView, setIsMobileView] = useState(false);

  useEffect(() => {
    setDocumentTitle(t("PortalAccess"));
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth <= size.smallTablet
      ? setIsMobileView(true)
      : setIsMobileView(false);
  };

  if (isMobileView) return <MobileView />;
  return (
    <MainContainer className="desktop-view">
      <Text className="subtitle">{t("PortalAccessSubTitle")}</Text>
      <CategoryWrapper
        t={t}
        title={t("SettingPasswordStrength")}
        tooltipTitle={t("SettingPasswordStrengthDescription")}
        tooltipUrl={passwordStrengthSettingsUrl}
        currentColorScheme={currentColorScheme}
      />
      <PasswordStrengthSection />
      <StyledSettingsSeparator />
      <CategoryWrapper
        t={t}
        title={t("TwoFactorAuth")}
        tooltipTitle={t("TwoFactorAuthDescription")}
        tooltipUrl={tfaSettingsUrl}
        currentColorScheme={currentColorScheme}
      />
      <TfaSection />
      <StyledSettingsSeparator />
      <CategoryWrapper
        t={t}
        title={t("TrustedMail")}
        tooltipTitle={t("TrustedMailDescription")}
        tooltipUrl={trustedMailDomainSettingsUrl}
        currentColorScheme={currentColorScheme}
      />
      <TrustedMailSection />
      <StyledSettingsSeparator />
      <CategoryWrapper
        t={t}
        title={t("IPSecurity")}
        tooltipContent={t("IPSecurityDescription")}
        tooltipTitle={t("IPSecurityDescription")}
      />
      <IpSecuritySection />
      <StyledSettingsSeparator />
      <CategoryWrapper
        t={t}
        title={t("AdminsMessage")}
        tooltipTitle={t("AdminsMessageDescription")}
        tooltipUrl={administratorMessageSettingsUrl}
        currentColorScheme={currentColorScheme}
      />
      <AdminMessageSection />

      <StyledSettingsSeparator />
      <CategoryWrapper
        t={t}
        title={t("SessionLifetime")}
        tooltipTitle={t("SessionLifetimeDescription")}
      />
      <SessionLifetimeSection />
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const {
    currentColorScheme,
    passwordStrengthSettingsUrl,
    tfaSettingsUrl,
    trustedMailDomainSettingsUrl,
    administratorMessageSettingsUrl,
  } = auth.settingsStore;
  return {
    currentColorScheme,
    passwordStrengthSettingsUrl,
    tfaSettingsUrl,
    trustedMailDomainSettingsUrl,
    administratorMessageSettingsUrl,
  };
})(withTranslation("Settings")(observer(AccessPortal)));
