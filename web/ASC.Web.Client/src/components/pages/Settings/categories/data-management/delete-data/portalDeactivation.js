import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { MainContainer } from "./StyledDeleteData";
import { size } from "@appserver/components/utils/device";

const PortalDeactivation = (props) => {
  const { t, history } = props;
  const [isDesktop, setIsDesktop] = useState(true);

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      history.location.pathname.includes("deactivation") &&
      history.push("/settings/datamanagement/delete-data");

    window.innerWidth >= size.desktop
      ? setIsDesktop(true)
      : setIsDesktop(false);
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
      <Button
        className="button"
        label={t("Deactivate")}
        primary
        size={isDesktop ? "small" : "normal"}
      />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(PortalDeactivation));
