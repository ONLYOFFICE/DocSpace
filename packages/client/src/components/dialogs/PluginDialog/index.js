import React from "react";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";

import WrappedComponent from "SRC_DIR/helpers/plugins/WrappedComponent";
import { PluginComponents } from "SRC_DIR/helpers/plugins/constants";

const PluginDialog = ({
  isVisible,
  dialogHeader,
  dialogBody,
  dialogFooter,
  onClose,
  pluginId,
  ...rest
}) => {
  console.log(rest);
  const onCloseAction = () => {
    const message = onClose();
  };

  console.log(dialogBody);

  return (
    <ModalDialog visible={isVisible} onClose={onCloseAction} {...rest}>
      <ModalDialog.Header>{dialogHeader}</ModalDialog.Header>
      <ModalDialog.Body>
        <WrappedComponent
          pluginId={pluginId}
          component={{ component: PluginComponents.box, props: dialogBody }}
        />
      </ModalDialog.Body>
      {dialogFooter && (
        <ModalDialog.Footer>
          <WrappedComponent
            pluginId={pluginId}
            component={{ component: PluginComponents.box, props: dialogFooter }}
          />
        </ModalDialog.Footer>
      )}
    </ModalDialog>
  );
};

export default inject(({ pluginStore }) => {
  const { pluginDialogProps } = pluginStore;

  return { ...pluginDialogProps, isVisible: pluginDialogProps.visible };
})(observer(PluginDialog));
