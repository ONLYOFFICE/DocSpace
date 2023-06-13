import React from "react";
import styled from "styled-components";
import CrossReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";

const StyledSelectedItem = styled.div`
  width: fit-content;
  height: 32px;

  display: inline-flex;
  align-items: center;
  justify-content: start;

  box-sizing: border-box;

  border-radius: 3px;

  padding: 6px 8px;

  margin-right: 4px;
  margin-bottom: 4px;

  background: ${(props) => props.theme.filterInput.selectedItems.background};

  :hover {
    background: ${(props) => props.theme.filterInput.selectedItems.hoverBackground};
  }
  .selected-item_label {
    line-height: 20px;
    margin-right: 10px;
    max-width: 23ch;
  }
`;

StyledSelectedItem.defaultProps = { theme: Base };

const SelectedItem = ({ label, removeSelectedItem }) => {
  if (!label) return <></>;

  return (
    <StyledSelectedItem onClick={removeSelectedItem}>
      <Text className={"selected-item_label"} title={label} truncate={true} noSelect>
        {label}
      </Text>
      <IconButton
        className="selected-tag-removed"
        iconName={CrossReactSvgUrl}
        size={12}
        onClick={removeSelectedItem}
        isFill
      />
    </StyledSelectedItem>
  );
};

export default React.memo(SelectedItem);
