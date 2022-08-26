import React, { useState } from "react";
import styled from "styled-components";

import { IconButton, TextInput } from "@docspace/components";
import { Base } from "@docspace/components/themes";

const StyledFolderInput = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-direction: row;
  gap: 0px;
  width: 100%;
  height: 32px;

  border-radius: 3px;
  transition: all 0.2s ease;

  &,
  .icon-wrapper {
    border: 1px solid
      ${(props) =>
        props.isFocused
          ? props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
              .focusBorderColor
          : props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
              .borderColor};
  }

  &:hover,
  &:hover > .icon-wrapper {
    border: 1px solid
      ${(props) =>
        props.isFocused
          ? props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
              .focusBorderColor
          : props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
              .hoverBorderColor};
  }

  .root_label {
    padding: 5px 2px 5px 7px;
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
    color: ${(props) =>
      props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
        .rootLabelColor};
  }

  .text_input {
    padding-left: 0;
    border: none;
    border-radius: 0px;
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
  }

  .icon-wrapper {
    cursor: pointer;
    height: 100%;
    box-sizing: border-box;
    width: 31px;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s ease;
    border-top: none !important;
    border-right: none !important;
    border-bottom: none !important;

    &:hover {
      path {
        fill: ${(props) =>
          props.theme.createEditRoomDialog.thirdpartyStorage.folderInput
            .iconFill};
      }
    }
  }
`;
StyledFolderInput.defaultProps = { theme: Base };

const FolderInput = ({ value, onChangeFolderPath }) => {
  const [isFocused, setIsFocused] = useState();

  const onFocus = () => setIsFocused(true);
  const onBlur = () => setIsFocused(false);

  return (
    <StyledFolderInput isFocused={isFocused}>
      <span className="root_label">ROOT/</span>
      <TextInput
        className="text_input"
        value={value}
        onChange={onChangeFolderPath}
        onFocus={onFocus}
        onBlur={onBlur}
      />
      <div className="icon-wrapper">
        <IconButton
          size={16}
          iconName="images/folder.react.svg"
          isClickable
          onClick={() => {}}
        />
      </div>
    </StyledFolderInput>
  );
};

export default FolderInput;
