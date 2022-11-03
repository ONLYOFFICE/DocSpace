import React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";

const StyledSelectedItem = styled.div`
  width: auto;
  height: 32px;

  max-width: ${!isMobileOnly ? "500px" : "100%"};

  display: flex;
  align-items: center;
  justify-content: start;

  box-sizing: border-box;

  border-radius: 3px;

  padding: 6px 8px;

  margin-right: 4px;
  margin-bottom: 4px;

  background: ${(props) => props.theme.filterInput.selectedItems.background};

  .selected-item_label {
    line-height: 20px;
    margin-right: 10px;
  }

  .remove-icon {
    min-width: 12px;
  }
`;

StyledSelectedItem.defaultProps = { theme: Base };

const SelectedItem = ({ propKey, label, group, removeSelectedItem }) => {
  const onRemove = () => {
    removeSelectedItem(propKey, label, group);
  };

  return (
    <StyledSelectedItem onClick={onRemove}>
      <Text className={"selected-item_label"} title={label} noSelect truncate>
        {label}
      </Text>
      <IconButton
        className={"remove-icon"}
        iconName={"/static/images/cross.react.svg"}
        size={12}
        onClick={onRemove}
        isFill
      />
    </StyledSelectedItem>
  );
};

export default React.memo(SelectedItem);
