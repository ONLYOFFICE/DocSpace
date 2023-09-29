import React from "react";
import styled from "styled-components";

import Heading from "@docspace/components/heading";
import Badge from "@docspace/components/badge";
import ContextMenuButton from "@docspace/components/context-menu-button";

const StyledPluginHeader = styled.div`
  display: flex;

  align-items: center;
  justify-content: space-between;

  height: 24px;

  margin-bottom: 24px;

  .plugin-header-info {
    display: flex;

    align-items: center;
    gap: 8px;
  }

  .plugin-header-badge {
    height: 22px;

    margin-top: 4px;
  }
`;

const PluginHeader = ({
  id,
  name,
  enabled,
  system,
  changePluginStatus,
  withDelete,
  showModalPluginSettings,
  openSettingsDialog,
  isUserSettings,
  uninstallPlugin,
}) => {
  const badgeLabel = enabled ? "enabled" : "disabled";

  const changePluginStatusAction = () => {
    changePluginStatus && changePluginStatus(id, enabled ? "false" : "true");
  };

  const getOptions = () => {
    const enabledOptLabel = !enabled ? "Enable" : "Disable";

    const enabledOpt = {
      key: "enable-plugin",
      label: enabledOptLabel,
      onClick: changePluginStatusAction,
    };

    const deleteOpt = {
      key: "delete-plugin",
      label: "Delete",
      onClick: () => uninstallPlugin && uninstallPlugin(id),
    };

    const settingsOpt = {
      key: "plugin-settings",
      label: "Settings",
      onClick: () => openSettingsDialog && openSettingsDialog(id),
    };

    const opts = [];

    if (!isUserSettings && !system) opts.push(enabledOpt);

    if (showModalPluginSettings) opts.push(settingsOpt);

    if (withDelete && !system && !isUserSettings) opts.push(deleteOpt);

    return opts;
  };

  const options = getOptions();

  return (
    <StyledPluginHeader>
      <div className="plugin-header-info">
        <Heading className={"plugin-header-text"} level={3} truncate>
          {name}
        </Heading>
        {!isUserSettings && (
          <Badge className={"plugin-header-badge"} label={badgeLabel} />
        )}
      </div>
      {options.length > 0 && <ContextMenuButton getData={getOptions} />}
    </StyledPluginHeader>
  );
};

export default PluginHeader;
