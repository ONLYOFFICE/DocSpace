import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { MainContainer } from "./StyledDeleteData";

const PortalDeactivation = (props) => {
  const { t } = props;

  return (
    <MainContainer>
      <Text fontSize="16px" fontWeight="700" className="header">
        {t("PortalDeletion")}
      </Text>
      <Text fontSize="12px" className="description">
        {t("PortalDeletionDescription")}
      </Text>
      <Text className="helper">{t("PortalDeletionHelper")}</Text>
      <Button
        className="button"
        label={t("Common:Delete")}
        primary
        size="normal"
      />
    </MainContainer>
  );
};

export default withTranslation(["Settings", "Common"])(
  withRouter(PortalDeactivation)
);
