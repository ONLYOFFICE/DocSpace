import React from "react";
import { useNavigate } from "react-router-dom";
import { inject, observer } from "mobx-react";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";

import {
  StyledPluginsSettings,
  StyledPluginList,
} from "./StyledPluginsSettings";

const PluginsSettingsBodyContent = ({
  withDelete,
  enabledPluginList,
  changePluginStatus,
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
            withDelete={withDelete}
            isUserSettings={true}
            isLast={index === enabledPluginList.length - 1}
            updatePluginStatus={updatePluginStatus}
          />
        ))}
      </StyledPluginList>
    </StyledPluginsSettings>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { enablePlugins, pluginOptions } = auth.settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  const { enabledPluginList, changePluginStatus, updatePluginStatus } =
    pluginStore;

  return {
    enablePlugins,
    withUpload,
    withDelete,
    enabledPluginList,
    changePluginStatus,
    updatePluginStatus,
  };
})(observer(PluginsSettingsBodyContent));
