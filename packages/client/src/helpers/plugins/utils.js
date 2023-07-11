import toastr from "@docspace/components/toast/toastr";

import { PluginActions } from "./constants";
import { PluginToastType } from "./constants";

export const messageActions = (
  message,
  setElementProps,
  setAcceptButtonProps,
  pluginId,
  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin
) => {
  if (!message) return;

  console.log(message);

  message.actions.forEach((action) => {
    switch (action) {
      case PluginActions.updateProps:
        setElementProps({ ...message.newProps });
        break;
      case PluginActions.updateStatus:
        break;
      case PluginActions.closeModal:
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
      case PluginActions.updateAcceptButtonProps:
        setAcceptButtonProps({ ...message.acceptButtonProps });
        break;
      case PluginActions.showSettingsModal:
        setSettingsPluginDialogVisible(true);
        setCurrentSettingsDialogPlugin(pluginId);
        break;
    }
  });
};
