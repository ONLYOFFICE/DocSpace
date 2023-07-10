import toastr from "@docspace/components/toast/toastr";

import { PluginActions } from "./constants";
import { PluginToastType } from "./constants";

export const messageActions = (
  message,
  setElementProps,
  setAcceptButtonProps
) => {
  if (!message) return;
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
        switch (message?.toastProps?.type) {
          case PluginToastType.success:
            toastr.success(message?.toastProps?.title);
            break;
          case PluginToastType.info:
            toastr.info(message?.toastProps?.title);
            break;
          case PluginToastType.error:
            toastr.error(message?.toastProps?.title);
            break;
          case PluginToastType.warning:
            toastr.warning(message?.toastProps?.title);
            break;
        }
        break;
      case PluginActions.updateAcceptButtonProps:
        setAcceptButtonProps({ ...message.acceptButtonProps });
        break;
    }
  });
};
