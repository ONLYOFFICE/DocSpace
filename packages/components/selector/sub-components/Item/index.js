import React from "react";
import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import Avatar from "../../../avatar";
import Text from "../../../text";
import Checkbox from "../../../checkbox";
import { Base } from "../../../themes";

const selectedCss = css`
  background: ${(props) =>
    props.theme.selector.item.selectedBackground} !important;
`;

const StyledItem = styled.div`
  display: flex;
  align-items: center;

  padding: 0 16px;

  box-sizing: border-box;

  :hover {
    cursor: pointer;
    background: ${(props) => props.theme.selector.item.hoverBackground};
  }

  ${(props) => props.isSelected && !props.isMultiSelect && selectedCss}

  .room-logo,
  .user-avatar {
    min-width: 32px;
  }

  .room-logo {
    height: 32px;

    border-radius: 6px;
  }

  .label {
    width: 100%;
    max-width: 100%;

    line-height: 16px;

    margin-left: 8px;
  }

  .checkbox {
    svg {
      margin-right: 0px;
    }
  }
`;

StyledItem.defaultProps = { theme: Base };

const compareFunction = (prevProps, nextProps) => {
  const prevData = prevProps.data;
  const prevItems = prevData.items;
  const prevIndex = prevProps.index;

  const nextData = nextProps.data;
  const nextItems = nextData.items;
  const nextIndex = nextProps.index;

  const prevItem = prevItems[prevIndex];
  const nextItem = nextItems[nextIndex];

  return (
    prevItem?.id === nextItem?.id &&
    prevItem?.isSelected === nextItem?.isSelected
  );
};

const Item = React.memo(({ index, style, data }) => {
  const { items, onSelect, isMultiSelect, isItemLoaded, rowLoader } = data;

  const isLoaded = isItemLoaded(index);

  const renderItem = () => {
    const item = items[index];

    if (!item && !item?.id) return <div style={style}>{rowLoader}</div>;

    const { label, avatar, icon, role, isSelected } = item;

    const currentRole = role ? role : "user";

    const isLogo = !!icon;

    const onChangeAction = () => {
      onSelect && onSelect(item);
    };

    const onClick = () => {
      !isMultiSelect && onSelect && onSelect(item);
    };

    return (
      <StyledItem
        isSelected={isSelected}
        isMultiSelect={isMultiSelect}
        style={style}
        onClick={onClick}
      >
        {!isLogo ? (
          <Avatar
            className="user-avatar"
            source={avatar}
            role={currentRole}
            size={"min"}
          />
        ) : (
          <img className="room-logo" src={icon} alt="room logo" />
        )}

        <Text
          className="label"
          fontWeight={600}
          fontSize={"14px"}
          noSelect
          truncate
        >
          {label}
        </Text>

        {isMultiSelect && (
          <Checkbox
            className="checkbox"
            isChecked={isSelected}
            onChange={onChangeAction}
          />
        )}
      </StyledItem>
    );
  };

  return isLoaded ? renderItem() : <div style={style}>{rowLoader}</div>;
}, compareFunction);

export default Item;
