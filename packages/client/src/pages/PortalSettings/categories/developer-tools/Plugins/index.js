import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

import Header from "./sub-components/header";
import UploadButton from "./sub-components/button";
import PluginItem from "./sub-components/plugin";

import { StyledContainer } from "./StyledPlugins";

const PluginPage = ({
  withDelete,
  withUpload,

  pluginList,

  openSettingsDialog,

  changePluginStatus,

  addPlugin,
  uninstallPlugin,

  currentColorScheme,
}) => {
  const { t } = useTranslation(["WebPlugins", "Common", "FilesSettings"]);

  React.useEffect(() => {
    setDocumentTitle(t("Plugins"));
  }, []);

  return (
    <StyledContainer>
      <Header t={t} currentColorScheme={currentColorScheme} />
      {withUpload && <UploadButton t={t} addPlugin={addPlugin} />}
      {pluginList.map((plugin) => (
        <PluginItem
          key={`plugin-${plugin.name}-${plugin.version}`}
          withDelete={withDelete}
          openSettingsDialog={openSettingsDialog}
          uninstallPlugin={uninstallPlugin}
          changePluginStatus={changePluginStatus}
          {...plugin}
        />
      ))}
    </StyledContainer>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { pluginOptions, currentColorScheme } = auth.settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  const {
    pluginList,
    changePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,

    uninstallPlugin,

    addPlugin,
  } = pluginStore;

  const openSettingsDialog = (pluginId, pluginName, pluginSystem) => {
    setSettingsPluginDialogVisible(true);
    setCurrentSettingsDialogPlugin({ pluginId, pluginName, pluginSystem });
  };

  return {
    withUpload,
    withDelete,

    pluginList,

    changePluginStatus,

    openSettingsDialog,

    uninstallPlugin,
    addPlugin,

    currentColorScheme,
  };
})(observer(PluginPage));
