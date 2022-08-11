import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import api from "@docspace/common/api";

const PortalDeletion = (props) => {
  const { t } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalDeletion"));
  }, []);

  const onDeleteClick = async () => {
    await api.portal.sendDeletePortalEmail();
  };

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
        onClick={onDeleteClick}
      />
    </MainContainer>
  );
};

export default withTranslation(["Settings", "Common"])(
  withRouter(PortalDeletion)
);
