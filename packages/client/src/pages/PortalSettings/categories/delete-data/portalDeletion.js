import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { MainContainer } from "./StyledDeleteData";
import { setDocumentTitle } from "../../../../helpers/utils";
import { DeletePortalDialog } from "SRC_DIR/components/dialogs";

const PortalDeletion = (props) => {
  const { t, getPortalOwner, owner } = props;
  const [isDialogVisible, setIsDialogVisible] = useState(false);

  useEffect(() => {
    setDocumentTitle(t("PortalDeletion"));
    getPortalOwner();
  }, []);

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
        onClick={() => setIsDialogVisible(true)}
      />

      <DeletePortalDialog
        visible={isDialogVisible}
        onClose={() => setIsDialogVisible(false)}
        owner={owner}
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
