import combineUrl from "@docspace/common/utils/combineUrl";

import toastr from "@docspace/components/toast/toastr";

import config from "PACKAGE_FILE";

import { PluginActions, PluginToastType } from "./constants";
import { Events } from "@docspace/common/constants";

export const messageActions = (
  message,
  setElementProps,
  setAcceptButtonProps,
  pluginId,
  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus,
  updatePropsContext,
  setPluginDialogVisible,
  setPluginDialogProps
) => {
  if (!message) return;

  message.actions.forEach((action) => {
    switch (action) {
      case PluginActions.updateProps:
        setElementProps && setElementProps({ ...message.newProps });

        break;
      case PluginActions.updateAcceptButtonProps:
        setAcceptButtonProps &&
          setAcceptButtonProps({ ...message.acceptButtonProps });

        break;
      case PluginActions.updateContext:
        if (message.contextProps) {
          updatePropsContext(
            message.contextProps.name,
            message.contextProps.props
          );
        }
        break;
      case PluginActions.updateStatus:
        updatePluginStatus && updatePluginStatus(pluginId);

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
        if (pluginId) {
          setSettingsPluginDialogVisible &&
            setSettingsPluginDialogVisible(true);
          setCurrentSettingsDialogPlugin &&
            setCurrentSettingsDialogPlugin(pluginId);
        }
        break;
      case PluginActions.showCreateDialogModal:
        if (message.createDialogProps) {
          const payload = { ...message.createDialogProps, pluginId };

          const event = new Event(Events.CREATE_PLUGIN_FILE);

          event.payload = payload;

          window.dispatchEvent(event);
        }
        break;
      case PluginActions.showModal:
        if (message.modalDialogProps) {
          setPluginDialogVisible(true);
          setPluginDialogProps(message.modalDialogProps);
        }

        break;
      case PluginActions.closeModal:
        setPluginDialogVisible(false);
        setPluginDialogProps(null);
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
