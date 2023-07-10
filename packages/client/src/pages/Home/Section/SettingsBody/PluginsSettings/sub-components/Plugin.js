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

  pluginSettings,
  getPluginSettings,

  settingsScope,

  changePluginStatus,

  ...rest
}) => {
  const showPluginSettings =
    settingsScope &&
    pluginSettings &&
    (pluginSettings.type === PluginSettingsType.both ||
      pluginSettings.type === PluginSettingsType.settingsPage);

  return (
    <StyledPlugin>
      <PluginHeader
        id={id}
        name={name}
        isActive={isActive}
        changePluginStatus={changePluginStatus}
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
          getPluginSettings={getPluginSettings}
        />
      )}
    </StyledPlugin>
  );
};

export default Plugin;
