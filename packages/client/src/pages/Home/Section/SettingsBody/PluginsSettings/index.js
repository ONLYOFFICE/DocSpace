import React from "react";

import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";
import EmptyScreen from "SRC_DIR/helpers/plugins/components/EmptyScreen";

import {
  StyledPluginsSettings,
  StyledPluginList,
} from "./StyledPluginsSettings";

const PluginsSettingsBodyContent = ({
  withDelete,
  pluginList,
  changePluginStatus,
  updatePluginStatus,
}) => {
  const { t } = useTranslation(["PluginsSettings", "FilesSettings"]);

  return (
    <StyledPluginsSettings>
      {pluginList.length > 0 ? (
        <StyledPluginList>
          {pluginList.map((plugin, index) => (
            <Plugin
              key={`plugin-${plugin.name}-${plugin.version}`}
              {...plugin}
              changePluginStatus={changePluginStatus}
              withDelete={withDelete}
              isUserSettings={true}
              isLast={index === pluginList.length - 1}
              updatePluginStatus={updatePluginStatus}
            />
          ))}
        </StyledPluginList>
      ) : (
        <EmptyScreen t={t} withUpload={false} />
      )}
    </StyledPluginsSettings>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { enablePlugins, pluginOptions } = auth.settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  const { pluginList, changePluginStatus, updatePluginStatus } = pluginStore;

  return {
    enablePlugins,
    withUpload,
    withDelete,
    pluginList,
    changePluginStatus,
    updatePluginStatus,
  };
})(observer(PluginsSettingsBodyContent));
