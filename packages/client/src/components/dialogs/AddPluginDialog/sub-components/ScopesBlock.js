import React from "react";

import styled from "styled-components";

import Block from "./Block";
import ToggleButton from "@docspace/components/toggle-button";

const StyledScopesContainer = styled.div`
  display: grid;

  grid-template-columns: 1fr;
  grid-template-rows: auto;

  gap: 32px;
`;

const ScopesBlock = ({
  startValue,
  onChangeApi,
  onChangeSettings,
  onChangeContextMenu,
  onChangeFilesContextMenu,
  onChangeFoldersContextMenu,
  onChangeRoomsContextMenu,
  onChangeMainButton,
  onChangeProfileMenu,
}) => {
  return (
    <Block headerText={"Scopes"} headerTitle={"Scopes"}>
      <StyledScopesContainer>
        <ToggleButton
          label={"API"}
          isChecked={startValue.api}
          onChange={onChangeApi}
        />
        <ToggleButton
          label={"Plugin settings"}
          isChecked={startValue.settings}
          onChange={onChangeSettings}
        />
        <ToggleButton
          label={"Context menu items"}
          isChecked={startValue.contextMenu}
          onChange={onChangeContextMenu}
        />
        {/* <ToggleButton
          label={"Files context menu"}
          isChecked={startValue.filesContextMenu}
          onChange={onChangeFilesContextMenu}
        /> */}
        <ToggleButton
          label={"Main button items"}
          isChecked={startValue.mainButton}
          onChange={onChangeMainButton}
        />
        {/* <ToggleButton
          label={"Folders context menu"}
          isChecked={startValue.foldersContextMenu}
          onChange={onChangeFoldersContextMenu}
        /> */}
        <ToggleButton
          label={"Profile menu items"}
          isChecked={startValue.profileMenu}
          onChange={onChangeProfileMenu}
        />
        {/* <ToggleButton
          label={"Rooms context menu"}
          isChecked={startValue.roomsContextMenu}
          onChange={onChangeRoomsContextMenu}
        /> */}
      </StyledScopesContainer>
    </Block>
  );
};

export default ScopesBlock;
