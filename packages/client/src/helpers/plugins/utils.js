import combineUrl from "@docspace/common/utils/combineUrl";

import toastr from "@docspace/components/toast/toastr";

import config from "PACKAGE_FILE";

import { PluginActions, PluginToastType } from "./constants";

export const messageActions = (
  message,
  setElementProps,
  setAcceptButtonProps,
  pluginId,
  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus
) => {
  if (!message) return;

  message.actions.forEach((action) => {
    switch (action) {
      case PluginActions.updateProps:
        setElementProps({ ...message.newProps });

        break;
      case PluginActions.updateAcceptButtonProps:
        setAcceptButtonProps({ ...message.acceptButtonProps });

        break;
      case PluginActions.updateStatus:
        updatePluginStatus && updatePluginStatus(pluginId);

        break;
      case PluginActions.showToast:
        message?.toastProps.forEach((toast) => {
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
        break;

      case PluginActions.showSettingsModal:
        setSettingsPluginDialogVisible(true);
        setCurrentSettingsDialogPlugin(pluginId);
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
