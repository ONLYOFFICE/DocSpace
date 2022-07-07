import React from "react";

import Backdrop from "@appserver/components/backdrop";
import Button from "@appserver/components/button";
import Heading from "@appserver/components/heading";
import IconButton from "@appserver/components/icon-button";

import FilterBlockItem from "./FilterBlockItem";

import PeopleSelector from "people/PeopleSelector";
import GroupSelector from "people/GroupSelector";

import {
  StyledFilterBlock,
  StyledFilterBlockHeader,
  StyledFilterBlockFooter,
  StyledControlContainer,
  StyledCrossIcon,
} from "./StyledFilterBlock";
import { withTranslation } from "react-i18next";
import Scrollbar from "@appserver/components/scrollbar";

//TODO: fix translate
const FilterBlock = ({
  t,
  selectedFilterValue,
  contextMenuHeader,
  getFilterData,
  hideFilterBlock,
  onFilter,
  headerLabel,
}) => {
  const [showSelector, setShowSelector] = React.useState({
    show: false,
    isAuthor: false,
    group: "",
  });
  const [filterData, setFilterData] = React.useState([]);
  const [filterValues, setFilterValues] = React.useState([]);

  const changeShowSelector = React.useCallback((isAuthor, group) => {
    setShowSelector((val) => {
      return {
        show: !val.show,
        isAuthor: isAuthor,
        group: group,
      };
    });
  }, []);

  const changeSelectedItems = React.useCallback(
    (filter) => {
      const data = filterData.map((item) => ({ ...item }));

      data.forEach((item) => {
        if (filter.find((value) => value.group === item.group)) {
          const currentFilter = filter.filter(
            (value) => value.group === item.group
          )[0];

          item.groupItem.forEach((groupItem) => {
            groupItem.isSelected = false;
            if (groupItem.key === currentFilter.key) {
              groupItem.isSelected = true;
            }
            if (groupItem.isSelector) {
              groupItem.isSelected = true;
              groupItem.selectedKey = currentFilter.key;
              groupItem.selectedLabel = currentFilter.label;
            }
            if (groupItem.isMultiSelect) {
              groupItem.isSelected = currentFilter.key.includes(groupItem.key);
            }
          });
        } else {
          item.groupItem.forEach((groupItem) => {
            groupItem.isSelected = false;
            if (groupItem.isSelector) {
              groupItem.selectedKey = null;
              groupItem.selectedLabel = null;
            }
          });
        }
      });

      setFilterData(data);
    },
    [filterData]
  );

  const onClearFilter = React.useCallback(() => {
    changeSelectedItems([]);
    setFilterValues([]);

    selectedFilterValue.length > 0 && onFilter && onFilter([]);
  }, [selectedFilterValue.length]);

  const changeFilterValue = React.useCallback(
    (group, key, isSelected, label, isMultiSelect) => {
      let value = filterValues.map((value) => {
        if (typeof value.key === "object") {
          const newKey = [...value.key];
          value.key = newKey;
        }

        return {
          ...value,
        };
      });

      if (isSelected) {
        if (isMultiSelect) {
          const groupIdx = value.findIndex((item) => item.group === group);

          const itemIdx = value[groupIdx].key.findIndex((item) => item === key);

          value[groupIdx].key.splice(itemIdx, 1);
        } else {
          value = value.filter((item) => item.group !== group);
        }

        setFilterValues(value);
        changeSelectedItems(value);

        const idx = selectedFilterValue.findIndex(
          (item) => item.group === group
        );

        if (idx > -1) {
          if (isMultiSelect) {
            const itemIdx = selectedFilterValue[idx].key.findIndex(
              (item) => item === key
            );

            if (itemIdx === -1) return;

            selectedFilterValue[idx].key.splice(itemIdx, 1);

            return onFilter(selectedFilterValue);
          }

          onFilter(value);
        }

        return;
      }

      if (value.find((item) => item.group === group)) {
        value.forEach((item) => {
          if (item.group === group) {
            if (isMultiSelect) {
              item.key.push(key);
            } else {
              item.key = key;
              if (label) {
                item.label = label;
              }
            }
          }
        });
      } else {
        if (label) {
          value.push({ group, key, label });
        } else if (isMultiSelect) {
          value.push({ group, key: [key] });
        } else {
          value.push({ group, key });
        }
      }

      setFilterValues(value);
      changeSelectedItems(value);
    },
    [selectedFilterValue, filterValues, changeSelectedItems]
  );

  React.useEffect(() => {
    const data = getFilterData();

    const items = data.filter((item) => item.isHeader === true);

    items.forEach((item) => {
      const groupItem = data.filter(
        (val) => val.group === item.group && val.isHeader !== true
      );

      groupItem.forEach((item) => (item.isSelected = false));

      item.groupItem = groupItem;
    });

    if (selectedFilterValue) {
      selectedFilterValue.forEach((selectedValue) => {
        items.forEach((item) => {
          if (item.group === selectedValue.group) {
            item.groupItem.forEach((groupItem) => {
              if (groupItem.key === selectedValue.key || groupItem.isSelector) {
                groupItem.isSelected = true;
                if (groupItem.isSelector) {
                  groupItem.selectedLabel = selectedValue.label;
                  groupItem.selectedKey = selectedValue.key;
                }
              }

              if (groupItem.isMultiSelect) {
                groupItem.isSelected = selectedValue.key.includes(
                  groupItem.key
                );
              }
            });
          }
        });
      });
    }

    const newFilterValues = selectedFilterValue.map((value) => {
      if (typeof value.key === "object") {
        const newKey = [...value.key];
        value.key = newKey;
      }

      return {
        ...value,
      };
    });

    setFilterData(items);
    setFilterValues(newFilterValues);
  }, []);

  const onFilterAction = React.useCallback(() => {
    onFilter && onFilter(filterValues);
    hideFilterBlock();
  }, [onFilter, hideFilterBlock, filterValues]);

  const onArrowClick = React.useCallback(() => {
    setShowSelector((val) => ({ ...val, show: false }));
  }, []);

  const selectOption = React.useCallback(
    (items) => {
      setShowSelector((val) => ({
        ...val,
        show: false,
      }));

      changeFilterValue(
        showSelector.group,
        items[0].key,
        false,
        items[0].label
      );
    },
    [showSelector.group, changeFilterValue]
  );

  const isEqualFilter = () => {
    const selectedFilterValues = selectedFilterValue;

    let isEqual = true;

    if (
      filterValues.length === 0 ||
      selectedFilterValues.length > filterValues.length
    )
      return !isEqual;

    if (
      (selectedFilterValues.length === 0 && filterValues.length > 0) ||
      selectedFilterValues.length !== filterValues.length
    ) {
      isEqual = false;

      return !isEqual;
    }

    filterValues.forEach((value) => {
      const oldValue = selectedFilterValues.find(
        (item) => item.group === value.group
      );

      let isMultiSelectEqual = true;

      if (typeof value.key === "object") {
        value.key.forEach(
          (item) =>
            (isMultiSelectEqual =
              isMultiSelectEqual && oldValue.key.includes(item))
        );
      }

      isEqual = isEqual && (oldValue?.key === value.key || isMultiSelectEqual);
    });

    return !isEqual;
  };

  const showFooter = isEqualFilter();

  return (
    <>
      {showSelector.show ? (
        <>
          <StyledFilterBlock>
            {showSelector.isAuthor ? (
              <PeopleSelector
                className="people-selector"
                isOpen={showSelector.show}
                withoutAside={true}
                isMultiSelect={false}
                onSelect={selectOption}
                onArrowClick={onArrowClick}
                headerLabel={headerLabel}
              />
            ) : (
              <GroupSelector
                className="people-selector"
                isOpen={showSelector.show}
                withoutAside={true}
                isMultiSelect={false}
                onSelect={selectOption}
                onArrowClick={onArrowClick}
                headerLabel={headerLabel}
              />
            )}

            <StyledControlContainer onClick={hideFilterBlock}>
              <StyledCrossIcon />
            </StyledControlContainer>
          </StyledFilterBlock>
        </>
      ) : (
        <StyledFilterBlock showFooter={showFooter}>
          <StyledFilterBlockHeader>
            <Heading size="medium">{contextMenuHeader}</Heading>
            <IconButton
              iconName="/static/images/clear.react.svg"
              isFill={true}
              onClick={onClearFilter}
              size={17}
            />
          </StyledFilterBlockHeader>
          <div className="filter-body">
            <Scrollbar className="filter-body__scrollbar" stype="mediumBlack">
              {filterData.map((item) => {
                return (
                  <FilterBlockItem
                    key={item.key}
                    label={item.label}
                    keyProp={item.key}
                    group={item.group}
                    groupItem={item.groupItem}
                    isLast={item.isLast}
                    withoutHeader={item.withoutHeader}
                    changeFilterValue={changeFilterValue}
                    showSelector={changeShowSelector}
                  />
                );
              })}
            </Scrollbar>
          </div>
          {showFooter && (
            <StyledFilterBlockFooter>
              <Button
                size="normal"
                primary={true}
                label={t("AddFilter")}
                scale={true}
                onClick={onFilterAction}
              />
            </StyledFilterBlockFooter>
          )}

          <StyledControlContainer onClick={hideFilterBlock}>
            <StyledCrossIcon />
          </StyledControlContainer>
        </StyledFilterBlock>
      )}

      <Backdrop
        visible={true}
        withBackground={true}
        onClick={hideFilterBlock}
        zIndex={215}
      />
    </>
  );
};

export default React.memo(withTranslation("Common")(FilterBlock));
