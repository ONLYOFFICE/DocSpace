import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "./tfa";
import PasswordStrengthSection from "./passwordStrength";
import MobileView from "./mobileView";
import CategoryWrapper from "../sub-components/category-wrapper";

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
    window.innerWidth <= 375 ? setIsMobileView(true) : setIsMobileView(false);
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
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(AccessPortal));
