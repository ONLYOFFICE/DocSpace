import React from "react";

import SelectorAddButton from "@docspace/components/selector-add-button";
import Heading from "@docspace/components/heading";
import ComboBox from "@docspace/components/combobox";
import Checkbox from "@docspace/components/checkbox";

import {
  StyledFilterBlockItem,
  StyledFilterBlockItemHeader,
  StyledFilterBlockItemContent,
  StyledFilterBlockItemSelector,
  StyledFilterBlockItemSelectorText,
  StyledFilterBlockItemTagText,
  StyledFilterBlockItemTagIcon,
  StyledFilterBlockItemToggle,
  StyledFilterBlockItemToggleText,
  StyledFilterBlockItemToggleButton,
  StyledFilterBlockItemCheckboxContainer,
  StyledFilterBlockItemSeparator,
} from "./StyledFilterBlock";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

import XIcon from "PUBLIC_DIR/images/x.react.svg";
import { FilterGroups, FilterKeys } from "../../../constants";

const FilterBlockItem = ({
  group,
  label,
  groupItem,
  isLast,
  withoutHeader,
  withoutSeparator,
  changeFilterValue,
  showSelector,
  isFirst,
  withMultiItems,
}) => {
  const changeFilterValueAction = (
    key,
    isSelected,
    isMultiSelect,
    withOptions
  ) => {
    changeFilterValue &&
      changeFilterValue(
        group,
        key,
        isSelected,
        null,
        isMultiSelect,
        withOptions
      );
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
      <StyledFilterBlockItemSelector
        style={
          item?.displaySelectorType === "button"
            ? {}
            : { height: "0", width: "0" }
        }
        key={item.key}
        onClick={(event) => showSelectorAction(event, isAuthor, item.group, [])}
      >
        {item?.displaySelectorType === "button" && (
          <SelectorAddButton id="filter_add-author" />
        )}
        <StyledFilterBlockItemSelectorText noSelect={true}>
          {item.label}
        </StyledFilterBlockItemSelectorText>
      </StyledFilterBlockItemSelector>
    ) : (
      <ColorTheme
        key={item.key}
        id={item.id}
        isSelected={item.isSelected}
        onClick={(event) =>
          showSelectorAction(
            event,
            isAuthor,
            item.group,
            clearSelectorRef.current
          )
        }
        themeId={ThemeType.FilterBlockItemTag}
      >
        <StyledFilterBlockItemTagText
          className="filter-text"
          noSelect={true}
          isSelected={item.isSelected}
          truncate
        >
          {item?.selectedLabel?.toLowerCase()}
        </StyledFilterBlockItemTagText>
        {item.isSelected && (
          <StyledFilterBlockItemTagIcon ref={clearSelectorRef}>
            <XIcon style={{ marginTop: "2px" }} />
          </StyledFilterBlockItemTagIcon>
        )}
      </ColorTheme>
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

  const getWithOptionsItem = (item) => {
    const selectedOption =
      item.options.find((option) => option.isSelected) || item.options[0];

    return (
      <ComboBox
        id={item.id}
        className={"combo-item"}
        key={item.key}
        onSelect={(data) =>
          changeFilterValueAction(
            data.key,
            data.key === item.options[0].key,
            false,
            item.withOptions
          )
        }
        options={item.options}
        selectedOption={selectedOption}
        displaySelectedOption={true}
        scaled={true}
        scaledOptions={true}
        isDefaultMode={false}
        directionY={"bottom"}
        fixedDirection
      />
    );
  };

  const getCheckboxItem = (item) => {
    return (
      <StyledFilterBlockItemCheckboxContainer key={item.key}>
        <Checkbox
          id={item.id}
          isChecked={item.isSelected}
          label={item.label}
          onChange={() =>
            changeFilterValueAction(item.key, item.isSelected, false)
          }
        />
      </StyledFilterBlockItemCheckboxContainer>
    );
  };

  const getTagItem = (item) => {
    const isAuthor = item.key === FilterKeys.user;

    if (
      item.group === FilterGroups.filterAuthor ||
      item.group === FilterGroups.roomFilterSubject
    ) {
      const [meItem, otherItem, userItem] = groupItem;

      if (
        item.key === otherItem.key &&
        userItem?.isSelected &&
        !meItem?.isSelected
      )
        return;
    }

    return (
      <ColorTheme
        key={item.key}
        isSelected={item.isSelected}
        name={`${item.label}-${item.key}`}
        id={item.id}
        onClick={
          item.key === FilterKeys.other
            ? (event) => showSelectorAction(event, isAuthor, item.group, [])
            : () =>
                changeFilterValueAction(
                  item.key,
                  item.isSelected,
                  item.isMultiSelect
                )
        }
        themeId={ThemeType.FilterBlockItemTag}
      >
        <StyledFilterBlockItemTagText
          className="filter-text"
          noSelect={true}
          isSelected={item.isSelected}
          truncate
        >
          {item.label}
        </StyledFilterBlockItemTagText>
      </ColorTheme>
    );
  };

  return (
    <StyledFilterBlockItem isFirst={isFirst} withoutHeader={withoutHeader}>
      {!withoutHeader && (
        <StyledFilterBlockItemHeader>
          <Heading size="xsmall">{label}</Heading>
        </StyledFilterBlockItemHeader>
      )}

      <StyledFilterBlockItemContent
        withMultiItems={withMultiItems}
        withoutHeader={withoutHeader}
        withoutSeparator={withoutSeparator}
      >
        {groupItem.map((item) => {
          if (item.displaySelectorType) return getSelectorItem(item);
          if (item.isToggle === true) return getToggleItem(item);
          if (item.withOptions === true) return getWithOptionsItem(item);
          if (item.isCheckbox === true) return getCheckboxItem(item);
          return getTagItem(item);
        })}
      </StyledFilterBlockItemContent>
      {!isLast && !withoutSeparator && <StyledFilterBlockItemSeparator />}
    </StyledFilterBlockItem>
  );
};

export default React.memo(FilterBlockItem);
