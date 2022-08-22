import React from "react";
import styled from "styled-components";

import { IconButton, TextInput } from "@docspace/components";

const StyledFolderInput = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-direction: row;
  gap: 0px;
  width: 100%;
  height: 32px;

  border: 1px solid #d0d5da;
  border-radius: 3px;

  .root_label {
    padding: 5px 2px 5px 7px;
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
    color: #a3a9ae;
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
    border-left: 1px solid #d0d5da;

    &:hover {
      path {
        fill: #657177;
      }
    }
  }
`;

const FolderInput = ({ value, onChangeFolderPath }) => {
  return (
    <StyledFolderInput>
      <span className="root_label">ROOT/</span>
      <TextInput
        className="text_input"
        value={value}
        onChange={onChangeFolderPath}
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
