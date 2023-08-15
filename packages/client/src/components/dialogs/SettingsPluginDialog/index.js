import React from "react";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";

import {
  PluginSettingsType,
  PluginComponents,
} from "SRC_DIR/helpers/plugins/constants";
import WrappedComponent from "SRC_DIR/helpers/plugins/WrappedComponent";

const SettingsPluginDialog = ({
  plugin,

  onLoad,
  customSettings,

  settingsPluginDialogVisible,
  currentSettingsDialogPlugin,

  onClose,

  isUserDialog,

  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,

  updateStatus,
  setPluginDialogVisible,
  setPluginDialogProps,
}) => {
  const [customSettingsProps, setCustomSettingsProps] =
    React.useState(customSettings);

  console.log(customSettings);

  const onLoadAction = React.useCallback(async () => {
    if (!onLoad) return;
    const res = await onLoad();

    setCustomSettingsProps(res.customSettings);
  }, [onLoad]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  const onCloseAction = () => {
    onClose();
  };

  return (
    <ModalDialog
      visible={settingsPluginDialogVisible}
      displayType="modal"
      onClose={onCloseAction}
      autoMaxHeight
    >
      <ModalDialog.Header>{plugin?.name}</ModalDialog.Header>
      <ModalDialog.Body>
        <WrappedComponent
          pluginId={plugin.id}
          component={{
            component: PluginComponents.box,
            props: customSettingsProps,
          }}
        />
      </ModalDialog.Body>
    </ModalDialog>
  );
};

export default inject(({ pluginStore }) => {
  const {
    pluginList,
    settingsPluginDialogVisible,
    setSettingsPluginDialogVisible,
    currentSettingsDialogPlugin,
    setCurrentSettingsDialogPlugin,
    setIsAdminSettingsDialog,
    isAdminSettingsDialog,
    updateStatus,
    setPluginDialogVisible,
    setPluginDialogProps,
  } = pluginStore;

  const isUserDialog = !isAdminSettingsDialog;

  const plugin = pluginList.find((p) => p.id === currentSettingsDialogPlugin);

  const pluginSettings = isUserDialog
    ? plugin?.getUserPluginSettings()
    : plugin?.getAdminPluginSettings();

  const onClose = () => {
    setSettingsPluginDialogVisible(false);
    setCurrentSettingsDialogPlugin(null);
    setIsAdminSettingsDialog(false);
  };

  if (pluginSettings.type === PluginSettingsType.settingsPage) {
    onClose();
  }

  return {
    plugin,
    ...pluginSettings,
    settingsPluginDialogVisible,
    currentSettingsDialogPlugin,
    onClose,
    isUserDialog,
    setSettingsPluginDialogVisible,
    setCurrentSettingsDialogPlugin,
    updateStatus,
    setPluginDialogVisible,
    setPluginDialogProps,
  };
})(observer(SettingsPluginDialog));
