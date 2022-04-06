import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "./tfa";
import PasswordStrengthSection from "./passwordStrength";
import TrustedMailSection from "./trustedMail";
import IpSecuritySection from "./ipSecurity";
import MobileView from "./mobileView";
import CategoryWrapper from "../sub-components/category-wrapper";
import { size } from "@appserver/components/utils/device";

const AccessPortal = (props) => {
  const { t } = props;
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
        title={t("SettingPasswordStrength")}
        tooltipContent={t("SettingPasswordStrengthDescription")}
      />
      <PasswordStrengthSection />
      <hr />
      <CategoryWrapper
        title={t("TwoFactorAuth")}
        tooltipContent={t("TwoFactorAuthDescription")}
      />
      <TfaSection />
      <hr />
      <CategoryWrapper
        title={t("TrustedMail")}
        tooltipContent={t("TrustedMailDescription")}
      />
      <TrustedMailSection />
      <hr />
      <CategoryWrapper
        title={t("IPSecurity")}
        tooltipContent={t("IPSecurityDescription")}
      />
      <IpSecuritySection />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(AccessPortal));
