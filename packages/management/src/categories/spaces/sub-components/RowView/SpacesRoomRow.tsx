import React from "react";
import Row from "@docspace/components/row";
import { RoomContent } from "./RoomContent";
import styled from "styled-components";
import CatalogSettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import DeleteReactSvgUrl from "PUBLIC_DIR/images/delete.react.svg?url";
import ExternalLinkIcon from "PUBLIC_DIR/images/external.link.react.svg?url";
import { ReactSVG } from "react-svg";

import TestIcon from "PUBLIC_DIR/images/logo/dark_lightsmall.svg?url";

const StyledRoomRow = styled(Row)`
  padding: 6px 0;

  .styled-element {
    width: 42px;
    margin-left: 20px;
  }

  .row_context-menu-wrapper {
    margin-right: 8px;
  }
`;

export const SpacesRoomRow = ({ item, deletePortal }) => {
  const element = <ReactSVG id={item.key} src={TestIcon} />; // change icon

  const contextOptionsProps = [
    {
      label: "Open",
      key: "space_open",
      icon: ExternalLinkIcon,
    },
    {
      label: "DocSpace Settings",
      key: "space_settings",
      icon: CatalogSettingsReactSvgUrl,
    },
    {
      key: "separator",
      isSeparator: true,
    },
    {
      label: "Delete",
      key: "space_delete",
      icon: DeleteReactSvgUrl,
      onClick: () => deletePortal(item.portalName, item.tenantId),
    },
  ];

  return (
    <StyledRoomRow
      contextOptions={contextOptionsProps}
      element={element}
      key={item.id}
      data={item}
    >
      <RoomContent item={item} />
    </StyledRoomRow>
  );
};
