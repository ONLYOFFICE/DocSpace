import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { MainContainer } from "./StyledDeleteData";
import { size } from "@appserver/components/utils/device";

const PortalDeactivation = (props) => {
  const { t, history } = props;

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("deactivation") &&
      history.push("/settings/datamanagement/delete-data");
  };

  return (
    <MainContainer>
      <Text fontSize="16px" fontWeight="700" className="header">
        {t("PortalDeactivation")}
      </Text>
      <Text fontSize="12px" className="description">
        {t("PortalDeactivationDescription")}
      </Text>
      <Text className="helper">{t("PortalDeactivationHelper")}</Text>
      <Button label={t("Deactivate")} primary size="normal" />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(PortalDeactivation));
