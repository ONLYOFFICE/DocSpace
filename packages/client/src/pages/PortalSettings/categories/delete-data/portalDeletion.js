import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import { DeletePortalDialog } from "SRC_DIR/components/dialogs";
import toastr from "@docspace/components/toast/toastr";
import {
  getPaymentAccount,
  sendDeletePortalEmail,
} from "@docspace/common/api/portal";

const PortalDeletion = (props) => {
  const { t, getPortalOwner, owner } = props;
  const [isDialogVisible, setIsDialogVisible] = useState(false);
  const [stripeUrl, setStripeUrl] = useState(null);

  const fetchData = async () => {
    const res = await getPaymentAccount();
    setStripeUrl(res);
  };

  useEffect(() => {
    setDocumentTitle(t("PortalDeletion"));
    getPortalOwner();
    fetchData();
  }, []);

  const onDeleteClick = async () => {
    if (stripeUrl) {
      setIsDialogVisible(true);
    } else {
      try {
        await sendDeletePortalEmail();
        toastr.success(
          t("PortalDeletionEmailSended", { ownerEmail: owner.email })
        );
      } catch (error) {
        toastr.error(error);
      }
    }
  };

  return (
    <MainContainer>
      <Text fontSize="16px" fontWeight="700" className="header">
        {t("DeleteDocSpace")}
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

      <DeletePortalDialog
        visible={isDialogVisible}
        onClose={() => setIsDialogVisible(false)}
        owner={owner}
        stripeUrl={stripeUrl}
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
