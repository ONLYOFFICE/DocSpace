import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

import Header from "./sub-components/header";
import UploadButton from "./sub-components/button";
import PluginItem from "./sub-components/plugin";

import { PluginListContainer, StyledContainer } from "./StyledPlugins";
import EmptyScreen from "./sub-components/EmptyScreen";

const PluginPage = ({
  withUpload,

  pluginList,

  openSettingsDialog,

  changePluginStatus,

  addPlugin,

  currentColorScheme,
  theme,
}) => {
  const { t } = useTranslation(["WebPlugins", "Common"]);

  React.useEffect(() => {
    setDocumentTitle(t("Plugins"));
  }, []);

  const learnMoreLink = "/";

  return (
    <>
      {pluginList.length === 0 ? (
        <EmptyScreen
          t={t}
          theme={theme}
          onAddAction={addPlugin}
          currentColorScheme={currentColorScheme}
          learnMoreLink={learnMoreLink}
          withUpload={withUpload}
        />
      ) : (
        <StyledContainer>
          <Header
            t={t}
            currentColorScheme={currentColorScheme}
            learnMoreLink={learnMoreLink}
          />
          {withUpload && <UploadButton t={t} addPlugin={addPlugin} />}
          <PluginListContainer>
            {pluginList.map((plugin) => (
              <PluginItem
                key={`plugin-${plugin.name}-${plugin.version}`}
                openSettingsDialog={openSettingsDialog}
                changePluginStatus={changePluginStatus}
                {...plugin}
              />
            ))}
          </PluginListContainer>
        </StyledContainer>
      )}
    </>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { pluginOptions, currentColorScheme, theme } = auth.settingsStore;

  const withUpload = pluginOptions.includes("upload");

  const {
    pluginList,
    changePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,

    addPlugin,
  } = pluginStore;

  const openSettingsDialog = (pluginId, pluginName, pluginSystem) => {
    setSettingsPluginDialogVisible(true);
    setCurrentSettingsDialogPlugin({ pluginId, pluginName, pluginSystem });
  };

  return {
    withUpload,

    pluginList,

    changePluginStatus,

    openSettingsDialog,

    addPlugin,

    currentColorScheme,
    theme,
  };
})(observer(PluginPage));
