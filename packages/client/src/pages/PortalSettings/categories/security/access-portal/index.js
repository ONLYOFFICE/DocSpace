import React, { useEffect, useState } from "react";
import { Trans, withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "./tfa";
import PasswordStrengthSection from "./passwordStrength";
import TrustedMailSection from "./trustedMail";
import IpSecuritySection from "./ipSecurity";
import AdminMessageSection from "./adminMessage";
import SessionLifetimeSection from "./sessionLifetime";
import BruteForceProtectionSection from "./bruteForceProtection";
import MobileView from "./mobileView";
import StyledSettingsSeparator from "SRC_DIR/pages/PortalSettings/StyledSettingsSeparator";
import { size } from "@docspace/components/utils/device";
import { inject, observer } from "mobx-react";
import Link from "@docspace/components/link";

const AccessPortal = (props) => {
  const {
    t,
    currentColorScheme,
    passwordStrengthSettingsUrl,
    tfaSettingsUrl,
    trustedMailDomainSettingsUrl,
    administratorMessageSettingsUrl,
    lifetimeSettingsUrl,
    ipSettingsUrl,
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
      <Text className="subtitle">{t("PortalSecurityTitle")}</Text>

      <Text fontSize="16px" fontWeight="700">
        {t("SettingPasswordTittle")}
      </Text>

      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("SettingPasswordDescription")}
        </Text>
        <Text fontSize="13px" fontWeight="400">
          <Trans t={t} i18nKey="SettingPasswordDescriptionSave" />
        </Text>
        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={passwordStrengthSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <PasswordStrengthSection />
      <StyledSettingsSeparator />
      <Text fontSize="16px" fontWeight="700">
        {t("TwoFactorAuth")}
      </Text>

      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("TwoFactorAuthEnableDescription")}
        </Text>
        <Text fontSize="13px" fontWeight="400">
          <Trans t={t} i18nKey="TwoFactorAuthSave" />
        </Text>
        <Text fontSize="13px" fontWeight="400">
          <Trans t={t} i18nKey="TwoFactorAuthNote" />
        </Text>
        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={tfaSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <TfaSection />
      <StyledSettingsSeparator />

      <Text fontSize="16px" fontWeight="700">
        {t("TrustedMail")}
      </Text>
      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("TrustedMailSettingDescription")}
        </Text>
        <Text fontSize="13px" fontWeight="400">
          <Trans t={t} i18nKey="TrustedMailSave" />
        </Text>
        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={trustedMailDomainSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <TrustedMailSection />
      <StyledSettingsSeparator />
      <Text fontSize="16px" fontWeight="700">
        {t("IPSecurity")}
      </Text>
      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("IPSecuritySettingDescription")}
        </Text>

        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={ipSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <IpSecuritySection />

      <StyledSettingsSeparator />

      <Text fontSize="16px" fontWeight="700">
        {t("BruteForceProtection")}
      </Text>

      <BruteForceProtectionSection />

      <StyledSettingsSeparator />

      <Text fontSize="16px" fontWeight="700">
        {t("AdminsMessage")}
      </Text>
      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("AdminsMessageSettingDescription")}
        </Text>
        <Text fontSize="13px" fontWeight="400">
          <Trans t={t} i18nKey="AdminsMessageSave" />
        </Text>

        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={administratorMessageSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

      <AdminMessageSection />

      <StyledSettingsSeparator />
      <Text fontSize="16px" fontWeight="700">
        {t("SessionLifetime")}
      </Text>

      <div className="category-item-description">
        <Text fontSize="13px" fontWeight="400">
          {t("SessionLifetimeSettingDescription")}
        </Text>

        <Link
          className="link-learn-more"
          target="_blank"
          isHovered
          color={currentColorScheme.main.accent}
          href={lifetimeSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>

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
    lifetimeSettingsUrl,
    ipSettingsUrl,
  } = auth.settingsStore;
  return {
    currentColorScheme,
    passwordStrengthSettingsUrl,
    tfaSettingsUrl,
    trustedMailDomainSettingsUrl,
    administratorMessageSettingsUrl,
    lifetimeSettingsUrl,
    ipSettingsUrl,
  };
})(withTranslation(["Settings", "Profile"])(observer(AccessPortal)));
