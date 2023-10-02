import combineUrl from "@docspace/common/utils/combineUrl";

import toastr from "@docspace/components/toast/toastr";

import config from "PACKAGE_FILE";

import { PluginActions, PluginToastType } from "./constants";
import { Events } from "@docspace/common/constants";

export const messageActions = (
  message,
  setElementProps,

  pluginId,
  pluginName,
  pluginSystem,

  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus,
  updatePropsContext,
  setPluginDialogVisible,
  setPluginDialogProps,

  updateContextMenuItems,
  updateInfoPanelItems,
  updateMainButtonItems,
  updateProfileMenuItems,
  updateEventListenerItems,
  updateFileItems
) => {
  if (!message || !message.actions || message.actions.length === 0) return;

  message.actions.forEach((action) => {
    switch (action) {
      case PluginActions.updateProps:
        setElementProps && setElementProps({ ...message.newProps });

        break;

      case PluginActions.updateContext:
        if (message.contextProps) {
          message.contextProps.forEach((prop) => {
            updatePropsContext && updatePropsContext(prop.name, prop.props);
          });
        }
        break;

      case PluginActions.updateStatus:
        updatePluginStatus &&
          updatePluginStatus(pluginId, pluginName, pluginSystem);

        break;

      case PluginActions.showToast:
        if (message.toastProps) {
          message.toastProps.forEach((toast) => {
            switch (toast.type) {
              case PluginToastType.success:
                toastr.success(toast.title);
                break;
              case PluginToastType.info:
                toastr.info(toast.title);
                break;
              case PluginToastType.error:
                toastr.error(toast.title);
                break;
              case PluginToastType.warning:
                toastr.warning(toast.title);
                break;
            }
          });
        }

        break;

      case PluginActions.showSettingsModal:
        if (pluginId || (pluginName && pluginSystem)) {
          setSettingsPluginDialogVisible &&
            setSettingsPluginDialogVisible(true);
          setCurrentSettingsDialogPlugin &&
            setCurrentSettingsDialogPlugin({
              pluginId,
              pluginName,
              pluginSystem,
            });
        }
        break;

      case PluginActions.closeSettingsModal:
        if (pluginId) {
          setSettingsPluginDialogVisible &&
            setSettingsPluginDialogVisible(false);
          setCurrentSettingsDialogPlugin &&
            setCurrentSettingsDialogPlugin(null);
        }
        break;

      case PluginActions.showCreateDialogModal:
        if (message.createDialogProps) {
          const payload = {
            ...message.createDialogProps,
            pluginId,
            pluginId,
            pluginName,
            pluginSystem,
          };

          const event = new Event(Events.CREATE_PLUGIN_FILE);

          event.payload = payload;

          window.dispatchEvent(event);
        }
        break;

      case PluginActions.showModal:
        if (message.modalDialogProps) {
          setPluginDialogVisible && setPluginDialogVisible(true);
          setPluginDialogProps &&
            setPluginDialogProps(message.modalDialogProps);
        }

        break;

      case PluginActions.closeModal:
        setPluginDialogVisible && setPluginDialogVisible(false);
        setPluginDialogProps && setPluginDialogProps(null);
        break;

      case PluginActions.updateContextMenuItems:
        updateContextMenuItems &&
          updateContextMenuItems(pluginId, pluginName, pluginSystem);

        break;
      case PluginActions.updateInfoPanelItems:
        updateInfoPanelItems &&
          updateInfoPanelItems(pluginId, pluginName, pluginSystem);

        break;
      case PluginActions.updateMainButtonItems:
        updateMainButtonItems &&
          updateMainButtonItems(pluginId, pluginName, pluginSystem);

        break;
      case PluginActions.updateProfileMenuItems:
        updateProfileMenuItems &&
          updateProfileMenuItems(pluginId, pluginName, pluginSystem);

        break;
      case PluginActions.updateEventListenerItems:
        updateEventListenerItems &&
          updateEventListenerItems(pluginId, pluginName, pluginSystem);

        break;
      case PluginActions.updateFileItems:
        updateFileItems && updateFileItems(pluginId, pluginName, pluginSystem);

        break;

      case PluginActions.sendPostMessage:
        if (!message.postMessage) return;

        const { postMessage } = message;

        const frame = document.getElementById(`${postMessage.frameId}`);

        if (frame) {
          frame.contentWindow.postMessage(
            JSON.stringify(postMessage.message),
            "*"
          );
        }

        break;
    }
  });
};

export const getPluginUrl = (url, file) => {
  const splittedURL = url.split("/");

  splittedURL.pop();

  const path = splittedURL.join("/");

  return combineUrl(
    window.DocSpaceConfig?.proxy?.url,
    config.homepage,
    path,
    file
  );
};
