import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import ModalDialogContainer from "../ModalDialogContainer";

const DeletePluginDialog = (props) => {
  const { t, ready } = useTranslation(["WebPlugins", "Common"]);
  const { isVisible, onClose, onDelete } = props;

  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const onDeleteClick = async () => {
    try {
      setIsRequestRunning(true);
      await onDelete();

      setIsRequestRunning(true);
      onClose();
    } catch (error) {
      toastr.error(error);
      onClose();
    }
  };

  return (
    <ModalDialogContainer
      isLoading={!ready}
      visible={isVisible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("DeletePluginTitle")}</ModalDialog.Header>
      <ModalDialog.Body>{t("DeletePluginDescription")}</ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="delete-button"
          key="DeletePortalBtn"
          label={t("Common:OkButton")}
          size="normal"
          scale
          primary={true}
          isLoading={isRequestRunning}
          onClick={onDeleteClick}
        />
        <Button
          className="cancel-button"
          key="CancelDeleteBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          isDisabled={isRequestRunning}
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ pluginStore }) => {
  const {
    deletePluginDialogProps,
    setDeletePluginDialogVisible,
    setDeletePluginDialogProps,
    uninstallPlugin,
  } = pluginStore;

  const onClose = () => {
    setDeletePluginDialogVisible(false);
    setDeletePluginDialogProps(null);
  };

  const { pluginId, pluginName, pluginSystem } = deletePluginDialogProps;

  const onDelete = async () => {
    await uninstallPlugin(pluginId, pluginName, pluginSystem);
  };

  return { onClose, onDelete };
})(observer(DeletePluginDialog));
