import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Plugin from "SRC_DIR/helpers/plugins/components/Plugin";
import EmptyScreen from "SRC_DIR/helpers/plugins/components/EmptyScreen";
import Button from "@docspace/components/button";
import { Events } from "@docspace/common/constants";

const StyledPluginsSettings = styled.div`
  width: 100%;

  .add-button {
    margin-bottom: 32px;
  }
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
  uninstallPlugin,
  updatePluginStatus,
}) => {
  const { t } = useTranslation(["PluginsSettings", "FilesSettings"]);

  const onAddAction = () => {
    const event = new Event(Events.ADD_PLUGIN);
    window.dispatchEvent(event);
  };

  return (
    <StyledPluginsSettings>
      {pluginList.length > 0 ? (
        <>
          <Button
            className={"add-button"}
            label={t("AddPlugin")}
            primary
            size={"normal"}
            onClick={onAddAction}
          />
          <StyledPluginList>
            {pluginList.map((plugin, index) => (
              <Plugin
                key={`plugin-${plugin.name}-${plugin.version}`}
                {...plugin}
                changePluginStatus={changePluginStatus}
                withDelete={withDelete}
                openSettingsDialog={openSettingsDialog}
                isLast={index === pluginList.length - 1}
                uninstallPlugin={uninstallPlugin}
                updatePluginStatus={updatePluginStatus}
              />
            ))}
          </StyledPluginList>
        </>
      ) : (
        <EmptyScreen t={t} withUpload={true} />
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
    uninstallPlugin,
    updatePluginStatus,
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
    uninstallPlugin,
    updatePluginStatus,
  };
})(observer(PluginPage));
