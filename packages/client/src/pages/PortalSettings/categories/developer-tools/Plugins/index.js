import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";
import EmptyScreen from "SRC_DIR/helpers/plugins/components/EmptyScreen";

const StyledPluginsSettings = styled.div`
  width: 100%;
`;

const StyledPluginList = styled.div`
  display: flex;

  flex-direction: column;
  gap: 32px;
`;

export { StyledPluginsSettings, StyledPluginList };

const PluginPage = ({
  withDelete,
  pluginList,
  changePluginStatus,
  openSettingsDialog,
}) => {
  const { t } = useTranslation(["PluginsSettings", "FilesSettings"]);

  return (
    <StyledPluginsSettings>
      {pluginList.length > 0 ? (
        <StyledPluginList>
          {pluginList.map((plugin) => (
            <Plugin
              key={`plugin-${plugin.name}-${plugin.version}`}
              {...plugin}
              changePluginStatus={changePluginStatus}
              withDelete={withDelete}
              openSettingsDialog={openSettingsDialog}
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

  const {
    pluginList,
    changePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setIsAdminSettingsDialog,
  } = pluginStore;

  const openSettingsDialog = (pluginId) => {
    setSettingsPluginDialogVisible(true);
    setCurrentSettingsDialogPlugin(pluginId);
    setIsAdminSettingsDialog(true);
  };

  return {
    enablePlugins,
    withUpload,
    withDelete,
    pluginList,
    changePluginStatus,
    openSettingsDialog,
  };
})(observer(PluginPage));
