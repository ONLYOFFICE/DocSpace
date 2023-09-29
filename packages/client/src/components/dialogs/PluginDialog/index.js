import React from "react";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";

import WrappedComponent from "SRC_DIR/helpers/plugins/WrappedComponent";
import { PluginComponents } from "SRC_DIR/helpers/plugins/constants";
import { messageActions } from "SRC_DIR/helpers/plugins/utils";

const PluginDialog = ({
  isVisible,
  dialogHeader,
  dialogBody,
  dialogFooter,
  onClose,
  onLoad,
  eventListeners,

  pluginId,
  pluginName,
  pluginSystem,

  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus,

  setPluginDialogVisible,
  setPluginDialogProps,

  updateContextMenuItems,
  updateInfoPanelItems,
  updateMainButtonItems,
  updateProfileMenuItems,
  updateEventListenerItems,
  updateFileItems,
  ...rest
}) => {
  const [dialogHeaderProps, setDialogHeaderProps] =
    React.useState(dialogHeader);
  const [dialogBodyProps, setDialogBodyProps] = React.useState(dialogBody);
  const [dialogFooterProps, setDialogFooterProps] =
    React.useState(dialogFooter);

  const [modalRequestRunning, setModalRequestRunning] = React.useState(false);

  const functionsRef = React.useRef([]);

  const onCloseAction = async () => {
    if (modalRequestRunning) return;
    const message = await onClose();

    messageActions(
      message,
      null,

      pluginId,
      pluginName,
      pluginSystem,

      setSettingsPluginDialogVisible,
      setCurrentSettingsDialogPlugin,
      updatePluginStatus,
      null,
      setPluginDialogVisible,
      setPluginDialogProps,

      updateContextMenuItems,
      updateInfoPanelItems,
      updateMainButtonItems,
      updateProfileMenuItems,
      updateEventListenerItems,
      updateFileItems
    );
  };

  React.useEffect(() => {
    if (eventListeners) {
      eventListeners.forEach((e) => {
        const onAction = async (evt) => {
          const message = await e.onAction(evt);

          messageActions(
            message,
            null,

            pluginId,
            pluginName,
            pluginSystem,

            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            null,
            setPluginDialogVisible,
            setPluginDialogProps,

            updateContextMenuItems,
            updateInfoPanelItems,
            updateMainButtonItems,
            updateProfileMenuItems,
            updateEventListenerItems,
            updateFileItems
          );
        };

        functionsRef.current.push(onAction);

        window.addEventListener(e.name, onAction);
      });
    }

    return () => {
      if (eventListeners) {
        eventListeners.forEach((e, index) => {
          window.removeEventListener(e.name, functionsRef.current[index]);
        });
      }
    };
  }, []);

  const onLoadAction = React.useCallback(async () => {
    // if (onLoad) {
    //   const res = await onLoad();
    //   setDialogHeaderProps(res.newDialogHeader);
    //   setDialogBodyProps(res.newDialogBody);
    //   setDialogFooterProps(res.newDialogFooter);
    // }
  }, [onLoad]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  return (
    <ModalDialog visible={isVisible} onClose={onCloseAction} {...rest}>
      <ModalDialog.Header>{dialogHeaderProps}</ModalDialog.Header>
      <ModalDialog.Body>
        <WrappedComponent
          pluginId={pluginId}
          pluginName={pluginName}
          pluginSystem={pluginSystem}
          component={{
            component: PluginComponents.box,
            props: dialogBodyProps,
          }}
          setModalRequestRunning={setModalRequestRunning}
        />
      </ModalDialog.Body>
      {dialogFooterProps && (
        <ModalDialog.Footer>
          <WrappedComponent
            pluginId={pluginId}
            pluginName={pluginName}
            pluginSystem={pluginSystem}
            component={{
              component: PluginComponents.box,
              props: dialogFooterProps,
            }}
            setModalRequestRunning={setModalRequestRunning}
          />
        </ModalDialog.Footer>
      )}
    </ModalDialog>
  );
};

export default inject(({ pluginStore }) => {
  const {
    pluginDialogProps,
    setSettingsPluginDialogVisible,
    setCurrentSettingsDialogPlugin,
    updatePluginStatus,

    setPluginDialogVisible,
    setPluginDialogProps,

    updateContextMenuItems,
    updateInfoPanelItems,
    updateMainButtonItems,
    updateProfileMenuItems,
    updateEventListenerItems,
    updateFileItems,
  } = pluginStore;

  return {
    ...pluginDialogProps,
    setSettingsPluginDialogVisible,
    setCurrentSettingsDialogPlugin,
    updatePluginStatus,

    setPluginDialogVisible,
    setPluginDialogProps,

    updateContextMenuItems,
    updateInfoPanelItems,
    updateMainButtonItems,
    updateProfileMenuItems,
    updateEventListenerItems,
    updateFileItems,
  };
})(observer(PluginDialog));
