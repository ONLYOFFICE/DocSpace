import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import { sendSuspendPortalEmail } from "@docspace/common/api/portal";
import { isDesktop } from "@docspace/components/utils/device";

const PortalDeactivation = (props) => {
  const { t, getPortalOwner, owner } = props;
  const [isDesktopView, setIsDesktopView] = useState(false);

  const fetchData = async () => {
    await getPortalOwner();
  };

  useEffect(() => {
    setDocumentTitle(t("PortalDeactivation"));
    fetchData();
    onCheckView();
    window.addEventListener("resize", onCheckView);
    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  const onCheckView = () => {
    if (isDesktop()) setIsDesktopView(true);
    else setIsDesktopView(false);
  };

  const onDeactivateClick = async () => {
    try {
      await sendSuspendPortalEmail();
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
        size={isDesktopView ? "small" : "normal"}
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
