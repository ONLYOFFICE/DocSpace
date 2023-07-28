import React from "react";
import { inject, observer } from "mobx-react";

import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader/RectangleLoader";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";

import {
  PluginSettingsType,
  PluginComponents,
} from "SRC_DIR/helpers/plugins/constants";
import ControlGroup from "SRC_DIR/helpers/plugins/components/ControlGroup";
import { messageActions } from "SRC_DIR/helpers/plugins/utils";
import WrappedComponent from "SRC_DIR/helpers/plugins/WrappedComponent";

const SettingsPluginDialog = ({
  plugin,

  groups,
  isLoading,
  onLoad,
  withAcceptButton,
  acceptButtonProps,
  cancelButtonProps,

  withCustomSettings,
  customSettings,

  settingsPluginDialogVisible,
  currentSettingsDialogPlugin,

  onClose,

  isUserDialog,

  updatePluginSettings,

  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updateStatus,
  setPluginDialogVisible,
  setPluginDialogProps,

  ...rest
}) => {
  const [groupsProps, setGroupsProps] = React.useState(groups);
  const [acceptButton, setAcceptButton] = React.useState({});
  const [isLoadingState, setIsLoadingState] = React.useState(isLoading);
  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const onLoadAction = React.useCallback(async () => {
    const res = await onLoad();

    const settings = isUserDialog
      ? plugin.getUserPluginSettings()
      : plugin.getAdminPluginSettings();
    setGroupsProps(settings.groups);
    setIsLoadingState(res);
  }, [plugin, onLoad, isUserDialog]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  React.useEffect(() => {
    if (withAcceptButton) setAcceptButton({ ...acceptButtonProps });
  }, [withAcceptButton, acceptButtonProps]);

  const onCloseAction = () => {
    if (cancelButtonProps) {
      cancelButtonProps.onClick();
    }

    onClose();
  };

  const getAcceptButtonElement = () => {
    if (isLoadingState)
      return <RectangleLoader width={"160px"} height={"40px"} />;
    const onClick = async () => {
      if (!acceptButton.onClick) return;

      setIsRequestRunning(true);

      const message = await acceptButton.onClick();

      messageActions(
        message,
        setAcceptButton,
        null,
        plugin.id,
        setSettingsPluginDialogVisible,
        setCurrentSettingsDialogPlugin,
        updateStatus,
        null,
        setPluginDialogVisible,
        setPluginDialogProps
      );

      setIsRequestRunning(false);
      onCloseAction();
    };

    return (
      <Button
        {...acceptButton}
        onClick={onClick}
        isLoading={isRequestRunning}
        size={"normal"}
      />
    );
  };

  const element = getAcceptButtonElement();

  return (
    <ModalDialog
      visible={settingsPluginDialogVisible}
      displayType="modal"
      onClose={onCloseAction}
      autoMaxHeight
      isLarge
    >
      <ModalDialog.Header>{plugin?.name}</ModalDialog.Header>
      <ModalDialog.Body>
        {withCustomSettings ? (
          <WrappedComponent
            pluginId={plugin.id}
            component={{
              component: PluginComponents.box,
              props: customSettings,
            }}
          />
        ) : (
          groupsProps?.map((group) => (
            <ControlGroup
              key={group.header}
              group={group}
              setAcceptButtonProps={setAcceptButton}
              isLoading={isLoadingState}
            />
          ))
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        {!withCustomSettings && (
          <>
            {element}

            {isLoadingState ? (
              <RectangleLoader width={"160px"} height={"40px"} />
            ) : (
              <Button
                {...cancelButtonProps}
                onClick={onCloseAction}
                size={"normal"}
              />
            )}
          </>
        )}
      </ModalDialog.Footer>
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
    updatePluginSettings,
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
    updatePluginSettings,
    setSettingsPluginDialogVisible,
    setCurrentSettingsDialogPlugin,
    updateStatus,
    setPluginDialogVisible,
    setPluginDialogProps,
  };
})(observer(SettingsPluginDialog));
