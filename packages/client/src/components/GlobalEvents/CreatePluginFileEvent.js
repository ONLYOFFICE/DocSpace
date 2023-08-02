import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { messageActions } from "SRC_DIR/helpers/plugins/utils";

import Dialog from "./sub-components/Dialog";

const CreatePluginFile = ({
  visible,
  title,
  startValue,
  onSave,
  onCancel,
  onClose,
  isCreateDialog,
  options,
  selectedOption,
  onSelect,
  extension,
  pluginId,
  updatePluginStatus,
  setCurrentSettingsDialogPlugin,
  setSettingsPluginDialogVisible,
  setPluginDialogVisible,
  setPluginDialogProps,
}) => {
  const { t } = useTranslation(["Translations", "Common", "Files"]);

  const onSaveAction = async (e, value) => {
    if (!onSave) return onCloseAction();

    const message = await onSave(e, value);

    messageActions(
      message,
      null,
      null,
      pluginId,
      setSettingsPluginDialogVisible,
      setCurrentSettingsDialogPlugin,
      updatePluginStatus,
      null,
      setPluginDialogVisible,
      setPluginDialogProps
    );
    onCloseAction();
  };

  const onCloseAction = (e) => {
    onCancel && onCancel();
    onClose && onClose();
  };

  const onSelectAction = (option) => {
    if (!onSelect) return;
    const message = onSelect(option);

    messageActions(
      message,
      null,
      null,
      pluginId,
      setSettingsPluginDialogVisible,
      setCurrentSettingsDialogPlugin,
      updatePluginStatus,
      null,
      setPluginDialogVisible,
      setPluginDialogProps
    );
  };

  return (
    <Dialog
      t={t}
      visible={visible}
      title={title}
      startValue={startValue}
      onSave={onSaveAction}
      onCancel={onCloseAction}
      onClose={onCloseAction}
      isCreateDialog={isCreateDialog}
      extension={extension}
      options={options}
      selectedOption={selectedOption}
      onSelect={onSelectAction}
    />
  );
};

export default inject(({ pluginStore }) => {
  const {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setPluginDialogVisible,
    setPluginDialogProps,
  } = pluginStore;
  return {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setPluginDialogVisible,
    setPluginDialogProps,
  };
})(observer(CreatePluginFile));
