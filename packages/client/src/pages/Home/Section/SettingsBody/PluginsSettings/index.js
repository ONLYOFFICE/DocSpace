import React from "react";
import { useNavigate } from "react-router-dom";
import { inject, observer } from "mobx-react";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";

import {
  StyledPluginsSettings,
  StyledPluginList,
} from "./StyledPluginsSettings";

const PluginsSettingsBodyContent = ({
  withDele,
  enabledPluginList,
  changePluginStatus,
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
            isLast={index === enabledPluginList.length - 1}
          />
        ))}
      </StyledPluginList>
    </StyledPluginsSettings>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { enablePlugins, pluginOptions } = auth.settingsStore;

  const { enabledPluginList, changePluginStatus } = pluginStore;

  return {
    enablePlugins,

    enabledPluginList,
    changePluginStatus,
  };
})(observer(PluginsSettingsBodyContent));
