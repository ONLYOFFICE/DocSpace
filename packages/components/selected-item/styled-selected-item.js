import React from "react";
import { Base } from "@docspace/components/themes";
import styled, { css } from "styled-components";

const StyledSelectedItem = styled.div`
  width: ${(props) => (props.isInline ? "fit-content" : "100%")};
  height: 32px;

  display: inline-flex;
  align-items: center;
  justify-content: space-between;

  box-sizing: border-box;

  border-radius: 3px;

  padding: 6px 8px;

  margin-right: 4px;
  margin-bottom: 4px;

  background: ${(props) => props.theme.filterInput.selectedItems.background};

  ${(props) =>
    !props.isDisabled &&
    css`
      :hover {
        background: ${(props) =>
          props.theme.filterInput.selectedItems.hoverBackground};
      }
    `}

  .selected-item_label {
    line-height: 20px;
    margin-right: 10px;
    max-width: 23ch;
    color: ${(props) => props.isDisabled && props.theme.text.disableColor};
  }
`;

StyledSelectedItem.defaultProps = { theme: Base };

export { StyledSelectedItem };
