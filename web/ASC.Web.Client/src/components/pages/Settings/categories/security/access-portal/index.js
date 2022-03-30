import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer } from "../StyledSecurity";
import TfaSection from "../sub-components/tfa";
import MobileView from "./mobileView";
import { isMobile } from "react-device-detect";

const AccessPortal = (props) => {
  const { t } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalAccess"));
  }, []);

  if (isMobile) return <MobileView />;
  return (
    <MainContainer className="desktop-view">
      <Text className="subtitle">{t("PortalAccessSubTitle")} </Text>

      <Text className="category-title" fontSize="16px" fontWeight="700">
        {t("TwoFactorAuth")}
      </Text>
      <TfaSection />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(AccessPortal));
