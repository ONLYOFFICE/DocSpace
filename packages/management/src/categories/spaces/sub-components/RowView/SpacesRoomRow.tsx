import React from "react";
import Row from "@docspace/components/row";
import { RoomContent } from "./RoomContent";
import { observer } from "mobx-react";
import styled from "styled-components";
import CatalogSettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import DeleteReactSvgUrl from "PUBLIC_DIR/images/delete.react.svg?url";
import ExternalLinkIcon from "PUBLIC_DIR/images/external.link.react.svg?url";
import DefaultLogoUrl from "PUBLIC_DIR/images/logo/dark_lightsmall.svg?url";
import { useTranslation } from "react-i18next";

import { ReactSVG } from "react-svg";
import { useStore } from "SRC_DIR/store";

const StyledRoomRow = styled(Row)`
  padding: 4px 0;

  .styled-element {
    width: 32px;
    margin-left: 20px;
  }

  .row_context-menu-wrapper {
    margin-right: 8px;
  }
`;
const SpacesRoomRow = ({ item }) => {
  const { spacesStore } = useStore();

  const { deletePortal, faviconLogo } = spacesStore;

  const { t } = useTranslation(["Common", "Files", "Settings"]);

  const logoElement = faviconLogo ? (
    <img id={item.key} width={"32px"} height={"32px"} src={faviconLogo} />
  ) : (
    <ReactSVG id={item.key} src={DefaultLogoUrl} />
  );

  const contextOptionsProps = [
    {
      label: t("Files:Open"),
      key: "space_open",
      icon: ExternalLinkIcon,
    },
    {
      label: t("Common:SettingsDocSpace"),
      key: "space_settings",
      icon: CatalogSettingsReactSvgUrl,
    },
    {
      key: "separator",
      isSeparator: true,
    },
    {
      label: t("Common:Delete"),
      key: "space_delete",
      icon: DeleteReactSvgUrl,
      onClick: () => deletePortal(item.portalName, item.tenantId),
    },
  ];

  return (
    <StyledRoomRow
      contextOptions={contextOptionsProps}
      element={logoElement}
      key={item.id}
      data={item}
    >
      <RoomContent item={item} />
    </StyledRoomRow>
  );
};

export default observer(SpacesRoomRow);
