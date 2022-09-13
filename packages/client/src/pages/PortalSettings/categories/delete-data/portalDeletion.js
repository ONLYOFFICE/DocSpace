import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import api from "@docspace/common/api";

const PortalDeletion = (props) => {
  const { t, getPortalOwner, owner } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalDeletion"));
    getPortalOwner();
  }, []);

  const onDeleteClick = async () => {
    try {
      await api.portal.sendDeletePortalEmail();
      toastr.success(
        t("PortalDeletionEmailSended", { ownerEmail: owner.email })
      );
    } catch (error) {
      toastr.error(error);
    }
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

export default inject(({ auth }) => {
  const { getPortalOwner, owner } = auth.settingsStore;
  return {
    getPortalOwner,
    owner,
  };
})(withTranslation(["Settings", "Common"])(withRouter(PortalDeletion)));
