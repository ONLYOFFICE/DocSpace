import React from "react";
import { useNavigate } from "react-router-dom";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import EmptyScreen from "./EmptyScreen";
import {
  StyledPluginsSettings,
  StyledPluginList,
} from "./StyledPluginsSettings";
import Plugin from "./sub-components/Plugin";

const PluginsSettingsBodyContent = ({
  enablePlugins,
  withUpload,
  withDelete,
  plugins,
  changePluginStatus,
}) => {
  const { t } = useTranslation(["PluginsSettings", "FilesSettings"]);
  const navigate = useNavigate();

  return (
    <StyledPluginsSettings>
      {plugins.length > 0 ? (
        <StyledPluginList>
          {plugins.map((plugin) => (
            <Plugin
              key={`plugin-${plugin.name}-${plugin.version}`}
              {...plugin}
              changePluginStatus={changePluginStatus}
            />
          ))}
        </StyledPluginList>
      ) : (
        <EmptyScreen t={t} withUpload={withUpload} />
      )}
    </StyledPluginsSettings>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { enablePlugins, pluginOptions } = auth.settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  const { plugins, changePluginStatus } = pluginStore;

  return { enablePlugins, withUpload, withDelete, plugins, changePluginStatus };
})(observer(PluginsSettingsBodyContent));
