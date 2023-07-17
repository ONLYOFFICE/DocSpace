import React from "react";
import styled from "styled-components";

import PluginHeader from "./PluginHeader";
import PluginInfo from "./PluginInfo";
import PluginSettings from "./PluginSettings";

import { PluginSettingsType } from "SRC_DIR/helpers/plugins/constants";

const StyledPlugin = styled.div`
  display: flex;

  flex-direction: column;

  max-width: 700px;

  .plugin-separator {
    height: 1px;
    width: 100%;

    background-color: #858585;
  }
`;

const Plugin = ({
  id,
  name,
  image,
  isActive,
  version,
  author,
  uploader,
  status,
  description,

  userPluginSettings,
  getUserPluginSettings,

  adminPluginSettings,
  getAdminPluginSettings,

  settingsScope,

  changePluginStatus,

  withDelete,

  isUserSettings,

  openSettingsDialog,

  uninstallPlugin,

  updatePluginStatus,

  isLast,

  ...rest
}) => {
  const pluginSettings = isUserSettings
    ? userPluginSettings
    : adminPluginSettings;
  const getPluginSettings = isUserSettings
    ? getUserPluginSettings
    : getAdminPluginSettings;

  const showPluginSettings =
    settingsScope &&
    pluginSettings &&
    (pluginSettings.type === PluginSettingsType.both ||
      pluginSettings.type === PluginSettingsType.settingsPage);

  const showModalPluginSettings =
    settingsScope && pluginSettings && !showPluginSettings;

  return (
    <StyledPlugin>
      <PluginHeader
        id={id}
        name={name}
        isActive={isActive}
        changePluginStatus={changePluginStatus}
        isUserSettings={isUserSettings}
        withDelete={withDelete}
        showModalPluginSettings={showModalPluginSettings}
        openSettingsDialog={openSettingsDialog}
        uninstallPlugin={uninstallPlugin}
      />
      <PluginInfo
        image={image}
        version={version}
        author={author}
        uploader={uploader}
        status={status}
        description={description}
      />
      {showPluginSettings && (
        <PluginSettings
          {...pluginSettings}
          id={id}
          getPluginSettings={getPluginSettings}
          updatePluginStatus={updatePluginStatus}
        />
      )}
      {!isLast && <div className="plugin-separator"></div>}
    </StyledPlugin>
  );
};

export default Plugin;