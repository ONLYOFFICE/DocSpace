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

const PortalDeactivation = (props) => {
  const { t, getPortalOwner, owner } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalDeactivation"));
    getPortalOwner();
  }, []);

  const onDeactivateClick = async () => {
    try {
      await api.portal.sendSuspendPortalEmail();
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

export default inject(({ auth }) => {
  const { getPortalOwner, owner } = auth.settingsStore;
  return {
    getPortalOwner,
    owner,
  };
})(withTranslation("Settings")(withRouter(PortalDeactivation)));
