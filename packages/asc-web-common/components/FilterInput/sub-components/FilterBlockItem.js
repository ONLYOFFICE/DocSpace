import React from "react";

import SelectorAddButton from "@appserver/components/selector-add-button";
import Heading from "@appserver/components/heading";

import {
  StyledFilterBlockItem,
  StyledFilterBlockItemHeader,
  StyledFilterBlockItemContent,
  StyledFilterBlockItemSelector,
  StyledFilterBlockItemSelectorText,
  StyledFilterBlockItemTag,
  StyledFilterBlockItemTagText,
  StyledFilterBlockItemTagIcon,
  StyledFilterBlockItemToggle,
  StyledFilterBlockItemToggleText,
  StyledFilterBlockItemToggleButton,
  StyledFilterBlockItemSeparator,
} from "./StyledFilterBlock";

import XIcon from "../svg/x.react.svg";

const FilterBlockItem = ({
  group,
  label,
  groupItem,
  isLast,
  withoutHeader,
  changeFilterValue,
  showSelector,
}) => {
  const changeFilterValueAction = (key, isSelected, isMultiSelect) => {
    changeFilterValue &&
      changeFilterValue(group, key, isSelected, null, isMultiSelect);
  };

  const showSelectorAction = (event, isAuthor, group, ref) => {
    let target = event.target;

    while (!!target.parentNode) {
      target = target.parentNode;

      if (target === ref) {
        changeFilterValue && changeFilterValue(group, [], true);
        return;
      }
    }

    showSelector && showSelector(isAuthor, group);
  };

  const getSelectorItem = (item) => {
    const clearSelectorRef = React.useRef(null);

    const isAuthor = item.key === "user";

    return !item.isSelected ||
      item.selectedKey === "me" ||
      item.selectedKey === "other" ? (
      <StyledFilterBlockItemSelector key={item.key}>
        <SelectorAddButton
          onClick={(event) =>
            showSelectorAction(event, isAuthor, item.group, [])
          }
        />
        <StyledFilterBlockItemSelectorText noSelect={true}>
          {item.label}
        </StyledFilterBlockItemSelectorText>
      </StyledFilterBlockItemSelector>
    ) : (
      <StyledFilterBlockItemTag
        key={item.key}
        isSelected={item.isSelected}
        onClick={(event) =>
          showSelectorAction(
            event,
            isAuthor,
            item.group,
            clearSelectorRef.current
          )
        }
      >
        <StyledFilterBlockItemTagText
          noSelect={true}
          isSelected={item.isSelected}
        >
          {item?.selectedLabel?.toLowerCase()}
        </StyledFilterBlockItemTagText>
        {item.isSelected && (
          <StyledFilterBlockItemTagIcon ref={clearSelectorRef}>
            <XIcon style={{ marginTop: "2px" }} />
          </StyledFilterBlockItemTagIcon>
        )}
      </StyledFilterBlockItemTag>
    );
  };

  const getToggleItem = (item) => {
    return (
      <StyledFilterBlockItemToggle key={item.key}>
        <StyledFilterBlockItemToggleText noSelect={true}>
          {item.label}
        </StyledFilterBlockItemToggleText>
        <StyledFilterBlockItemToggleButton
          isChecked={item.isSelected}
          onChange={() => changeFilterValueAction(item.key, item.isSelected)}
        />
      </StyledFilterBlockItemToggle>
    );
  };

  const getTagItem = (item) => {
    return (
      <StyledFilterBlockItemTag
        key={item.key}
        isSelected={item.isSelected}
        name={`${item.label}-${item.key}`}
        onClick={() =>
          changeFilterValueAction(item.key, item.isSelected, item.isMultiSelect)
        }
      >
        <StyledFilterBlockItemTagText
          noSelect={true}
          isSelected={item.isSelected}
        >
          {item.label}
        </StyledFilterBlockItemTagText>
      </StyledFilterBlockItemTag>
    );
  };

  return (
    <StyledFilterBlockItem withoutHeader={withoutHeader}>
      {!withoutHeader && (
        <StyledFilterBlockItemHeader>
          <Heading size="xsmall">{label}</Heading>
        </StyledFilterBlockItemHeader>
      )}

      <StyledFilterBlockItemContent withoutHeader={withoutHeader}>
        {groupItem.map((item) => {
          if (item.isSelector === true) return getSelectorItem(item);
          if (item.isToggle === true) return getToggleItem(item);
          return getTagItem(item);
        })}
      </StyledFilterBlockItemContent>
      {!isLast && <StyledFilterBlockItemSeparator />}
    </StyledFilterBlockItem>
  );
};

export default React.memo(FilterBlockItem);
