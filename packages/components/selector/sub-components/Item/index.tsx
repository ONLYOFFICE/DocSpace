import React from "react";

import Avatar from "../../../avatar";
import Text from "../../../text";
import Checkbox from "../../../checkbox";

import StyledItem from "./StyledItem";

import { ItemProps, Data, Item as ItemType } from "./Item.types";

const compareFunction = (prevProps: ItemProps, nextProps: ItemProps) => {
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

const Item = React.memo(({ index, style, data }: ItemProps) => {
  const { items, onSelect, isMultiSelect, isItemLoaded, rowLoader }: Data =
    data;

  const isLoaded = isItemLoaded(index);

  const renderItem = () => {
    const item: ItemType = items[index];

    if (!item || (item && !item.id))
      return <div style={style}>{rowLoader}</div>;

    const { label, avatar, icon, role, isSelected, isDisabled } = item;

    const currentRole = role ? role : "user";

    const isLogo = !!icon;

    const onChangeAction = () => {
      onSelect && onSelect(item);
    };

    const onClick = (e: React.MouseEvent<HTMLDivElement>) => {
      if (
        ((e.target instanceof HTMLElement || e.target instanceof SVGElement) &&
          !!e.target.closest(".checkbox")) ||
        isDisabled
      )
        return;

      onSelect && onSelect(item);
    };

    return (
      <StyledItem
        isSelected={isSelected}
        isMultiSelect={isMultiSelect}
        style={style}
        onClick={onClick}
        className="test-22"
        isDisabled={isDisabled}
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
