import React from "react";
import { useNavigate } from "react-router-dom";
import { inject, observer } from "mobx-react";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";

import {
  StyledPluginsSettings,
  StyledPluginList,
} from "./StyledPluginsSettings";

const PluginsSettingsBodyContent = ({
  enabledPluginList,
  changePluginStatus,
  openSettingsDialog,
  updatePluginStatus,
}) => {
  const navigate = useNavigate();

  React.useEffect(() => {
    if (enabledPluginList.length === 0) {
      navigate("/");
    }
  }, [enabledPluginList]);

  return (
    <StyledPluginsSettings>
      <StyledPluginList>
        {enabledPluginList.map((plugin, index) => (
          <Plugin
            key={`plugin-${plugin.name}-${plugin.version}`}
            {...plugin}
            changePluginStatus={changePluginStatus}
            isUserSettings={true}
            openSettingsDialog={openSettingsDialog}
            updatePluginStatus={updatePluginStatus}
            isLast={index === enabledPluginList.length - 1}
          />
        ))}
      </StyledPluginList>
    </StyledPluginsSettings>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { enablePlugins } = auth.settingsStore;

  const {
    enabledPluginList,
    changePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setIsAdminSettingsDialog,

    updatePluginStatus,
  } = pluginStore;

  const openSettingsDialog = (pluginId) => {
    setSettingsPluginDialogVisible(true);
    setCurrentSettingsDialogPlugin(pluginId);
    setIsAdminSettingsDialog(false);
  };

  return {
    enablePlugins,

    enabledPluginList,
    changePluginStatus,

    openSettingsDialog,
    updatePluginStatus,
  };
})(observer(PluginsSettingsBodyContent));
