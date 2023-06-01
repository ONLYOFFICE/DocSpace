import React from "react";
import { Trans, useTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import { sendDeletePortalEmail } from "@docspace/common/api/portal";

import ModalDialogContainer from "../ModalDialogContainer";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const DeletePortalDialog = (props) => {
  const { t, ready } = useTranslation("Settings", "Common");
  const { visible, onClose, owner, stripeUrl } = props;

  const onDeleteClick = async () => {
    try {
      await sendDeletePortalEmail();
      toastr.success(
        t("PortalDeletionEmailSended", { ownerEmail: owner.email })
      );
      onClose();
    } catch (error) {
      toastr.error(error);
    }
  };

  return (
    <ModalDialogContainer
      isLoading={!ready}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("DeleteDocSpace")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Trans t={t} i18nKey="DeleteDocSpaceInfo" ns="Settings">
          Before you delete the portal, please make sure that automatic billing
          is turned off. You may check the status of automatic billing in
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontSize="13px"
            fontWeight="600"
            href={stripeUrl}
            target="_blank"
          >
            on your Stripe customer portal.
          </ColorTheme>
        </Trans>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="DeletePortalBtn"
          label={t("Common:Delete")}
          size="normal"
          scale
          primary={true}
          onClick={onDeleteClick}
        />
        <Button
          key="CancelDeleteBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default DeletePortalDialog;
