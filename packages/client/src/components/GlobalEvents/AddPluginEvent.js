import React from "react";
import { inject, observer } from "mobx-react";
import AddPluginDialog from "../dialogs/AddPluginDialog";

const AddPluginEvent = ({ visible, enablePlugins, onClose }) => {
  React.useEffect(() => {
    if (!enablePlugins) onClose();
  }, [enablePlugins, onClose]);

  return <AddPluginDialog visible={visible} onClose={onClose} />;
};

export default inject(({ auth }) => {
  return {
    enablePlugins: auth.settingsStore.enablePlugins,
  };
})(observer(AddPluginEvent));
