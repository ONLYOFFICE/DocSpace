import React from "react";
import styled, { css } from "styled-components";
import { isMobile } from "react-device-detect";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";
import { tablet } from "@docspace/components/utils/device";

const StyledSelectedItem = styled.div`
  width: auto;
  height: 32px;

  max-width: ${(props) => `calc(${props.sectionWidth}px - 20px)`};

  display: flex;
  align-items: center;
  justify-content: start;

  box-sizing: border-box;

  border-radius: 3px;

  padding: 6px 8px;

  margin-right: 0px;
  margin-bottom: 4px;

  background: ${(props) => props.theme.filterInput.selectedItems.background};

  @media ${tablet} {
    max-width: ${(props) => `calc(${props.sectionWidth}px - 16px)`};
  }

  ${isMobile &&
  css`
    max-width: ${(props) => `calc(${props.sectionWidth}px - 16px)`};
  `}

  .selected-item_label {
    line-height: 20px;
    margin-right: 10px;
  }

  .remove-icon {
    min-width: 12px;
  }
`;

StyledSelectedItem.defaultProps = { theme: Base };

const SelectedItem = ({
  propKey,
  label,
  group,
  removeSelectedItem,
  sectionWidth,
}) => {
  const onRemove = () => {
    removeSelectedItem(propKey, label, group);
  };

  return (
    <StyledSelectedItem onClick={onRemove} sectionWidth={sectionWidth}>
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
