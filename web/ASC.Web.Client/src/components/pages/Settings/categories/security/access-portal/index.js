import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "./tfa";
import PasswordStrengthSection from "./passwordStrength";
import TrustedMailSection from "./trustedMail";
import MobileView from "./mobileView";
import CategoryWrapper from "../sub-components/category-wrapper";
import { size } from "@appserver/components/utils/device";
import { inject, observer } from "mobx-react";

const AccessPortal = (props) => {
  const { t, helpLink } = props;
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
        tooltipUrl={`${helpLink}/administration/configuration.aspx#ChangingSecuritySettings_block`}
      />
      <PasswordStrengthSection />
      <hr />
      <CategoryWrapper
        t={t}
        title={t("TwoFactorAuth")}
        tooltipTitle={t("TwoFactorAuthDescription")}
        tooltipUrl={`${helpLink}/administration/two-factor-authentication.aspx`}
      />
      <TfaSection />
      <hr />
      <CategoryWrapper
        t={t}
        title={t("TrustedMail")}
        tooltipTitle={t("TrustedMailDescription")}
        tooltipUrl={`${helpLink}/administration/configuration.aspx#ChangingSecuritySettings_block`}
      />
      <TrustedMailSection />
    </MainContainer>
  );
};

export default inject(({ auth }) => {
  const { helpLink } = auth.settingsStore;
  return { helpLink };
})(withTranslation("Settings")(withRouter(observer(AccessPortal))));
