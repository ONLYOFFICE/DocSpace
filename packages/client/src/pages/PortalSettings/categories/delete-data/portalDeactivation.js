import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import api from "@docspace/common/api";

const PortalDeactivation = (props) => {
  const { t } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalDeactivation"));
  }, []);

  const onDeactivateClick = async () => {
    await api.portal.sendSuspendPortalEmail();
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
        size="normal"
        onClick={onDeactivateClick}
      />
    </MainContainer>
  );
};

export default withTranslation("Settings")(withRouter(PortalDeactivation));
