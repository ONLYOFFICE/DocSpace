import React from "react";
import styled from "styled-components";

import PluginHeader from "./PluginHeader";
import PluginInfo from "./PluginInfo";
import PluginSettings from "./PluginSettings";

import { PluginSettingsType } from "SRC_DIR/helpers/plugins/constants";

import { PluginScopes } from "../constants";

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
  enabled,
  version,
  author,
  status,
  description,
  createBy,
  createOn,
  homePage,
  license,
  system,
  url,

  userPluginSettings,
  adminPluginSettings,

  scopes,

  changePluginStatus,

  withDelete,

  isUserSettings,

  openSettingsDialog,

  uninstallPlugin,

  updatePluginStatus,

  isLast,

  ...rest
}) => {
  const withSettings = scopes.includes(PluginScopes.Settings);

  const pluginSettings = isUserSettings
    ? userPluginSettings
    : adminPluginSettings;

  const showPluginSettingsPage =
    withSettings &&
    pluginSettings &&
    (pluginSettings.type === PluginSettingsType.both ||
      pluginSettings.type === PluginSettingsType.settingsPage);

  const showModalPluginSettings =
    withSettings && pluginSettings?.type && !showPluginSettingsPage;

  return (
    <StyledPlugin>
      <PluginHeader
        id={id}
        name={name}
        enabled={enabled}
        system={system}
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
        status={status}
        description={description}
        createBy={createBy}
        createOn={createOn}
        homePage={homePage}
        url={url}
      />
      {showPluginSettingsPage && (
        <PluginSettings
          {...pluginSettings}
          id={id}
          updatePluginStatus={updatePluginStatus}
        />
      )}
      {!isLast && <div className="plugin-separator"></div>}
    </StyledPlugin>
  );
};

export default Plugin;
