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
  addPlugin,
}) => {
  const { t } = useTranslation(["PluginsSettings", "FilesSettings"]);

  const pluginInputRef = React.useRef(null);

  const onAddAction = () => {
    pluginInputRef.current.click();
  };

  const onInputClick = (e) => {
    e.target.value = null;
  };

  const onFileChange = (e) => {
    let formData = new FormData();

    formData.append("file", e.target.files[0]);

    console.log(e.target.files[0]);

    addPlugin(formData);
  };

  return (
    <StyledPluginsSettings>
      {pluginList.length > 0 ? (
        <>
          <Button
            className={"add-button"}
            label={t("UploadPlugin")}
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
        <EmptyScreen t={t} onAddAction={onAddAction} />
      )}
      <input
        id="customPluginInput"
        className="custom-file-input"
        type="file"
        accept=".zip"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={pluginInputRef}
        style={{ display: "none" }}
      />
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
    addPlugin,
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
    addPlugin,
  };
})(observer(PluginPage));
