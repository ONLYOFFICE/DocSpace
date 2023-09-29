import React from "react";
import styled from "styled-components";

import Heading from "@docspace/components/heading";
import IconButton from "@docspace/components/icon-button";
import ToggleButton from "@docspace/components/toggle-button";
import Badge from "@docspace/components/badge";
import Text from "@docspace/components/text";

import PluginSettingsIconUrl from "PUBLIC_DIR/images/plugin.settings.react.svg?url";
import PluginDefaultLogoUrl from "PUBLIC_DIR/images/plugin.default-logo.png";

import { getPluginUrl } from "SRC_DIR/helpers/plugins/utils";
import { PluginScopes } from "SRC_DIR/helpers/plugins/constants";

const StyledPluginItem = styled.div`
  width: 100%;
  max-width: 500px;

  height: auto;

  display: grid;
  grid-template-rows: 1fr;
  grid-template-columns: 64px 1fr;

  gap: 20px;

  border: 1px solid #d0d5da;
  border-radius: 12px;

  padding: 24px;

  box-sizing: border-box;

  .plugin-logo {
    width: 64px;
    height: 64px;

    border-radius: 4px;
  }

  .plugin-info {
    width: 100%;
    height: auto;

    display: flex;
    flex-direction: column;

    gap: 8px;
  }
`;

const StyledPluginHeader = styled.div`
  width: 100%;
  height: 22px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  .plugin-name {
    margin: 0;
    padding: 0;
  }

  .plugin-controls {
    height: 100%;

    display: flex;
    gap: 16px;

    .plugin-toggle-button {
      position: relative;

      gap: 0;
    }
  }
`;

const PluginItem = ({
  id,

  name,
  version,
  description,

  enabled,
  changePluginStatus,

  scopes,
  openSettingsDialog,

  image,
  url,

  system,

  ...rest
}) => {
  const imgSrc = image ? getPluginUrl(url, `/assets/${image}`) : null;

  const withSettings = scopes.includes(PluginScopes.Settings);

  const onChangeStatus = () => {
    changePluginStatus && changePluginStatus(id, name, !enabled, system);
  };

  const onOpenSettingsDialog = () => {
    openSettingsDialog && openSettingsDialog(id, name, system);
  };

  return (
    <StyledPluginItem>
      <img
        className="plugin-logo"
        src={imgSrc || PluginDefaultLogoUrl}
        alt={"Plugin logo"}
      />
      <div className="plugin-info">
        <StyledPluginHeader>
          <Heading className={"plugin-name"}>{name}</Heading>
          <div className="plugin-controls">
            {withSettings && (
              <IconButton
                iconName={PluginSettingsIconUrl}
                size={16}
                onClick={onOpenSettingsDialog}
              />
            )}
            <ToggleButton
              className="plugin-toggle-button"
              onChange={onChangeStatus}
              isChecked={enabled}
            />
          </div>
        </StyledPluginHeader>

        <Badge
          label={version}
          fontSize={"12px"}
          fontWeight={700}
          noHover={true}
          backgroundColor={"#22C386"}
        />
        {imgSrc && description && (
          <Text fontWeight={400} lineHeight={"20px"}>
            {description}
          </Text>
        )}
      </div>
    </StyledPluginItem>
  );
};

export default PluginItem;
