import React from "react";

import Aside from "@appserver/components/aside";
import Backdrop from "@appserver/components/backdrop";
import Button from "@appserver/components/button";
import Heading from "@appserver/components/heading";
import IconButton from "@appserver/components/icon-button";

import FilterBlockItem from "./FilterBlockItem";

// import Selector from "./Selector";

import PeopleSelector from "people/PeopleSelector"; //TODO: Move out PeopleSelector  of FilterItem

import {
  StyledFilterBlock,
  StyledFilterBlockHeader,
  StyledFilterBlockFooter,
} from "./StyledFilterBlock";

//TODO: fix translate
const FilterBlock = ({
  selectedFilterData,
  contextMenuHeader,
  getFilterData,
  hideFilterBlock,
  onFilter,
  addUserHeader,
}) => {
  const [showSelector, setShowSelector] = React.useState({
    show: false,
    isAuthor: false,
    group: "",
    selectedItems: [],
  });

  const [filterData, setFilterData] = React.useState([]);
  const [filterValues, setFilterValues] = React.useState([]);

  const changeShowSelector = (isAuthor, group, selectedItems) => {
    setShowSelector((val) => {
      return {
        show: !val.show,
        isAuthor: isAuthor,
        group: group,
        selectedItems: selectedItems,
      };
    });
  };

  const changeSelectedItems = (filter) => {
    const items = filterData.slice();

    items.forEach((item) => {
      if (filter.find((value) => value.group === item.group)) {
        const key = filter.filter((value) => value.group === item.group)[0].key;
        item.groupItem.forEach((groupItem) => {
          groupItem.isSelected = false;
          if (groupItem.key === key) {
            groupItem.isSelected = true;
          }
          if (groupItem.isSelector) {
            groupItem.isSelected = true;
            groupItem.selectedItem = key;
          }
        });
      } else {
        item.groupItem.forEach((groupItem) => {
          groupItem.isSelected = false;
          if (groupItem.isSelector) {
            groupItem.selectedItem = [];
          }
        });
      }
    });

    setFilterData(items);
  };

  const clearFilter = () => {
    changeSelectedItems([]);
    setFilterValues([]);
  };

  const changeFilterValue = (group, key, isSelected) => {
    let value = filterValues.filter((item) => item.key !== key);

    if (isSelected) {
      value = filterValues.filter((item) => item.group !== group);

      setFilterValues(value);
      changeSelectedItems(value);
      return;
    }

    if (value.find((item) => item.group === group)) {
      value.forEach((item) => {
        if (item.group === group) {
          item.key = key;
        }
      });
    } else {
      value.push({ group, key });
    }

    setFilterValues(value);
    changeSelectedItems(value);
  };

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

    if (selectedFilterData.filterValues) {
      selectedFilterData.filterValues.forEach((value) => {
        items.forEach((item) => {
          if (item.group === value.group) {
            item.groupItem.forEach((groupItem) => {
              if (
                groupItem.key === value.key ||
                (groupItem.selectedItem &&
                  Object.keys(groupItem.selectedItem).length > 0)
              ) {
                groupItem.isSelected = true;
              }
            });
          }
        });
      });
    }
    console.log();
    setFilterData(items);
    setFilterValues(selectedFilterData.filterValues);
  }, [selectedFilterData, getFilterData]);

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
      selectedItems: [...items],
    }));

    changeFilterValue(showSelector.group, items[0].key, false);
  };

  return (
    <>
      {showSelector.show ? (
        <>
          <StyledFilterBlock>
            <PeopleSelector
              className="people-selector"
              isOpen={showSelector.show}
              withoutAside={true}
              isMultiSelect={false}
              onSelect={selectOption}
              onArrowClick={onArrowClick}
              headerLabel={addUserHeader}
            />
          </StyledFilterBlock>
        </>
      ) : (
        <StyledFilterBlock>
          <StyledFilterBlockHeader>
            <Heading size="medium">{contextMenuHeader}</Heading>
            <IconButton
              iconName="/static/images/clear.react.svg"
              isFill={true}
              onClick={clearFilter}
              size={17}
            />
          </StyledFilterBlockHeader>
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
          <StyledFilterBlockFooter>
            <Button
              size="large"
              primary={true}
              label="Apply filters"
              scale={true}
              onClick={onFilterAction}
            />
          </StyledFilterBlockFooter>
        </StyledFilterBlock>
      )}

      <Backdrop
        visible={true}
        withBackground={true}
        onClick={hideFilterBlock}
      />
    </>
  );
};

export default React.memo(FilterBlock);
