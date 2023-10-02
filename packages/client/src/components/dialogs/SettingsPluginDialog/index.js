import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import ModalDialog from "@docspace/components/modal-dialog";

import { PluginComponents } from "SRC_DIR/helpers/plugins/constants";
import WrappedComponent from "SRC_DIR/helpers/plugins/WrappedComponent";

import Header from "./sub-components/Header";
import Info from "./sub-components/Info";
import Footer from "./sub-components/Footer";
import Button from "@docspace/components/button";

const SettingsPluginDialog = ({
  plugin,
  withDelete,
  onLoad,

  settings,
  saveButton,

  settingsPluginDialogVisible,

  onClose,
  onDelete,

  ...rest
}) => {
  const { t } = useTranslation(["WebPlugins", "Common", "Files", "People"]);

  const [customSettingsProps, setCustomSettingsProps] =
    React.useState(settings);

  const [saveButtonProps, setSaveButtonProps] = React.useState(saveButton);

  const [modalRequestRunning, setModalRequestRunning] = React.useState(false);

  const onLoadAction = React.useCallback(async () => {
    if (!onLoad) return;
    const res = await onLoad();

    setCustomSettingsProps(res.settings);
    if (res.saveButton)
      setSaveButtonProps({
        ...res.saveButton,
        props: { ...res.saveButton, scale: true },
      });
  }, [onLoad]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  const onCloseAction = () => {
    if (modalRequestRunning) return;
    onClose();
  };

  const onDeleteAction = () => {
    if (modalRequestRunning) return;
    onClose();

    onDelete();
  };

  return (
    <ModalDialog
      visible={settingsPluginDialogVisible}
      displayType="aside"
      onClose={onCloseAction}
      withBodyScroll
      withFooterBorder
    >
      <ModalDialog.Header>
        <Header t={t} name={plugin?.name} />
      </ModalDialog.Header>
      <ModalDialog.Body>
        <WrappedComponent
          pluginId={plugin.id}
          pluginName={plugin.name}
          pluginSystem={plugin.system}
          component={{
            component: PluginComponents.box,
            props: customSettingsProps,
          }}
          saveButton={saveButton}
          setSaveButtonProps={setSaveButtonProps}
          setModalRequestRunning={setModalRequestRunning}
        />
        <Info t={t} plugin={plugin} withDelete={withDelete} />
        {withDelete && (
          <Button
            label={t("DeletePlugin")}
            onClick={onDeleteAction}
            scale
            size={"normal"}
          />
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Footer
          t={t}
          id={plugin?.id}
          pluginName={plugin.name}
          pluginSystem={plugin.system}
          saveButtonProps={saveButtonProps}
          setModalRequestRunning={setModalRequestRunning}
          onCloseAction={onCloseAction}
          modalRequestRunning={modalRequestRunning}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth, pluginStore }) => {
  const {
    pluginList,
    settingsPluginDialogVisible,
    setSettingsPluginDialogVisible,
    currentSettingsDialogPlugin,
    setCurrentSettingsDialogPlugin,
    setDeletePluginDialogVisible,
    setDeletePluginDialogProps,
  } = pluginStore;

  const { pluginOptions } = auth.settingsStore;

  const { pluginId, pluginSystem, pluginName } = currentSettingsDialogPlugin;

  const plugin = pluginSystem
    ? pluginList.find((p) => p.name === pluginName)
    : pluginList.find((p) => p.id === pluginId);

  const withDelete = pluginOptions.includes("delete") && !pluginSystem;

  const pluginSettings = plugin?.getAdminPluginSettings();

  const onClose = () => {
    setSettingsPluginDialogVisible(false);
    setCurrentSettingsDialogPlugin(null);
  };

  const onDelete = () => {
    setDeletePluginDialogVisible(true);
    setDeletePluginDialogProps({ pluginId, pluginSystem, pluginName });
  };

  return {
    plugin,
    withDelete,
    ...pluginSettings,
    settingsPluginDialogVisible,

    onClose,
    onDelete,
  };
})(observer(SettingsPluginDialog));
