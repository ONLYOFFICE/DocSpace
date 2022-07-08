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

const FilterBlock = ({
  t,
  selectedFilterData,
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
  const [clearFilter, setClearFilter] = React.useState(false);

  const changeShowSelector = (isAuthor, group) => {
    setShowSelector((val) => {
      return {
        show: !val.show,
        isAuthor: isAuthor,
        group: group,
      };
    });
  };

  const changeSelectedItems = (filter) => {
    const items = filterData.slice();

    items.forEach((item) => {
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

    setFilterData(items);
  };

  const onClearFilter = () => {
    changeSelectedItems([]);
    setFilterValues([]);
    setClearFilter(true);
    selectedFilterData.filterValues.length > 0 && onFilter && onFilter([]);
  };

  const changeFilterValue = (group, key, isSelected, label) => {
    let value = filterValues.concat();

    if (isSelected) {
      value = filterValues.filter((item) => item.group !== group);

      setFilterValues(value);
      changeSelectedItems(value);

      const idx = selectedFilterData.filterValues.findIndex(
        (item) => item.group === group
      );

      if (idx > -1) {
        setClearFilter(true);
        onFilter(value);
      }

      return;
    }

    if (value.find((item) => item.group === group)) {
      value.forEach((item) => {
        if (item.group === group) {
          item.key = key;
          if (label) {
            item.label = label;
          }
        }
      });
    } else {
      if (label) {
        value.push({ group, key, label });
      } else {
        value.push({ group, key });
      }
    }

    setFilterValues(value);
    changeSelectedItems(value);
  };

  React.useEffect(() => {
    if (!clearFilter) {
      const data = getFilterData();

      const items = data.filter((item) => item.isHeader === true);

      items.forEach((item) => {
        const groupItem = data.filter(
          (val) => val.group === item.group && val.isHeader !== true
        );

        groupItem.forEach((item) => (item.isSelected = false));

        item.groupItem = groupItem;
      });

      if (selectedFilterData.filterValues) {
        selectedFilterData.filterValues.forEach((value) => {
          items.forEach((item) => {
            if (item.group === value.group) {
              item.groupItem.forEach((groupItem) => {
                if (groupItem.key === value.key || groupItem.isSelector) {
                  groupItem.isSelected = true;
                  if (groupItem.isSelector) {
                    groupItem.selectedLabel = value.label;
                    groupItem.selectedKey = value.key;
                  }
                }
              });
            }
          });
        });
      }

      const newFilterValues = selectedFilterData.filterValues.map((value) => ({
        ...value,
      }));

      setFilterData(items);
      setFilterValues(newFilterValues);
    }
  }, [selectedFilterData, getFilterData, clearFilter]);

  const onFilterAction = () => {
    onFilter && onFilter(filterValues);
    hideFilterBlock();
  };

  const onArrowClick = () => {
    setShowSelector((val) => ({ ...val, show: false }));
  };

  const selectOption = (items) => {
    setShowSelector((val) => ({
      ...val,
      show: false,
    }));

    changeFilterValue(showSelector.group, items[0].key, false, items[0].label);
  };

  const isEqualFilter = () => {
    const selectedFilterValues = selectedFilterData.filterValues;

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

      isEqual = isEqual && oldValue?.key === value.key;
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
