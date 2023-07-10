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

const PluginHeader = ({ id, name, isActive, changePluginStatus }) => {
  const badgeLabel = isActive ? "active" : "disabled";

  const changePluginStatusAction = () => {
    changePluginStatus && changePluginStatus(id, isActive ? "false" : "true");
  };

  const getOptions = () => {
    const activateOptLabel = !isActive ? "Activate" : "Disable";

    return [
      {
        key: "activate-plugin",
        label: activateOptLabel,
        onClick: changePluginStatusAction,
      },
      { key: "delete-plugin", label: "Delete" },
    ];
  };

  return (
    <StyledPluginHeader>
      <div className="plugin-header-info">
        <Heading className={"plugin-header-text"} level={3} truncate>
          {name}
        </Heading>
        <Badge className={"plugin-header-badge"} label={badgeLabel} />
      </div>
      <ContextMenuButton getData={getOptions} />
    </StyledPluginHeader>
  );
};

export default PluginHeader;
