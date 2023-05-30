import React from "react";
import { withTranslation } from "react-i18next";
import { isMobileOnly } from "react-device-detect";

import ClearReactSvgUrl from "PUBLIC_DIR/images/clear.react.svg?url";

import Loaders from "../../Loaders";

import Backdrop from "@docspace/components/backdrop";
import Button from "@docspace/components/button";
import Heading from "@docspace/components/heading";
import IconButton from "@docspace/components/icon-button";
import Scrollbar from "@docspace/components/scrollbar";
import Portal from "@docspace/components/portal";

import FilterBlockItem from "./FilterBlockItem";

import PeopleSelector from "client/PeopleSelector";
import RoomSelector from "@docspace/client/src/components/RoomSelector";

import {
  StyledFilterBlock,
  StyledFilterBlockHeader,
  StyledFilterBlockFooter,
  StyledControlContainer,
  StyledCrossIcon,
} from "./StyledFilterBlock";
import { FilterSelectorTypes } from "../../../constants";

//TODO: fix translate
const FilterBlock = ({
  t,
  selectedFilterValue,
  filterHeader,
  getFilterData,
  hideFilterBlock,
  onFilter,
  selectorLabel,
  isPersonalRoom,
  isRooms,
  isAccounts,
}) => {
  const [showSelector, setShowSelector] = React.useState({
    show: false,
    type: null,
    group: "",
  });

  const [filterData, setFilterData] = React.useState([]);
  const [filterValues, setFilterValues] = React.useState([]);
  const [isLoading, setIsLoading] = React.useState(false);

  const changeShowSelector = React.useCallback((selectorType, group) => {
    setShowSelector((val) => ({
      show: !val.show,
      type: selectorType,
      group: group,
    }));
  }, []);

  const changeSelectedItems = React.useCallback(
    (filter) => {
      const data = filterData.map((item) => ({ ...item }));

      data.forEach((item) => {
        if (filter.find((value) => value.group === item.group)) {
          const currentFilter = filter.find(
            (value) => value.group === item.group
          );

          item.groupItem.forEach((groupItem) => {
            groupItem.isSelected = false;
            if (groupItem.key === currentFilter.key) {
              groupItem.isSelected = true;
            }
            if (groupItem.displaySelectorType) {
              groupItem.isSelected = true;
              groupItem.selectedKey = currentFilter.key;
              groupItem.selectedLabel = currentFilter.label;
            }
            if (groupItem.isMultiSelect) {
              groupItem.isSelected = currentFilter.key.includes(groupItem.key);
            }
            if (groupItem.withOptions) {
              groupItem.isSelected = currentFilter.key.includes(groupItem.key);
            }
          });
        } else {
          item.groupItem.forEach((groupItem, idx) => {
            groupItem.isSelected = false;
            if (groupItem.displaySelectorType) {
              groupItem.selectedKey = null;
              groupItem.selectedLabel = null;
            }
            if (groupItem.withOptions) {
              item.groupItem[idx].options.forEach((x, index) => {
                item.groupItem[idx].options[index].isSelected = false;
              });
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

    selectedFilterValue &&
      selectedFilterValue.length > 0 &&
      onFilter &&
      onFilter([]);
  }, [changeSelectedItems, selectedFilterValue?.length]);

  const changeFilterValue = React.useCallback(
    (group, key, isSelected, label, isMultiSelect, withOptions) => {
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

          if (value[groupIdx].key.length === 0)
            value = value.filter((item) => item.group !== group);
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

  const getDefaultFilterData = React.useCallback(async () => {
    setIsLoading(true);
    const data = await getFilterData();

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
              if (
                groupItem.key === selectedValue.key ||
                groupItem.displaySelectorType
              ) {
                groupItem.isSelected = true;
                if (groupItem.displaySelectorType) {
                  groupItem.selectedLabel = selectedValue.label;
                  groupItem.selectedKey = selectedValue.key;
                }
              }

              if (groupItem.isMultiSelect) {
                groupItem.isSelected = selectedValue.key.includes(
                  groupItem.key
                );
              }

              if (groupItem.withOptions) {
                groupItem.options.forEach(
                  (option) =>
                    (option.isSelected = option.key === selectedValue.key)
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

    setTimeout(() => {
      setIsLoading(false);
    }, 500);
  }, []);

  React.useEffect(() => {
    getDefaultFilterData();
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

      changeFilterValue(showSelector.group, items[0].id, false, items[0].label);
    },
    [showSelector.group, changeFilterValue]
  );

  const isEqualFilter = () => {
    let isEqual = true;

    if (
      filterValues.length === 0 ||
      selectedFilterValue.length > filterValues.length
    )
      return !isEqual;

    if (
      (selectedFilterValue.length === 0 && filterValues.length > 0) ||
      selectedFilterValue.length !== filterValues.length
    ) {
      isEqual = false;

      return !isEqual;
    }

    filterValues.forEach((value) => {
      const oldValue = selectedFilterValue.find(
        (item) => item.group === value.group
      );

      let isMultiSelectEqual = false;
      let withOptionsEqual = false;

      if (typeof value.key === "object") {
        isMultiSelectEqual = true;
        value.key.forEach(
          (item) =>
            (isMultiSelectEqual =
              isMultiSelectEqual && oldValue.key.includes(item))
        );
      }

      if (value.options) {
        withOptionsEqual = true;
        value.options.forEach(
          (option) =>
            (withOptionsEqual =
              isMultiSelectEqual && option.key === oldValue.key)
        );
      }

      isEqual =
        isEqual &&
        (oldValue?.key === value.key || isMultiSelectEqual || withOptionsEqual);
    });

    return !isEqual;
  };

  const showFooter = isEqualFilter();

  const filterBlockComponent = (
    <>
      {showSelector.show ? (
        <>
          <StyledFilterBlock>
            {showSelector.type === FilterSelectorTypes.people ? (
              <PeopleSelector
                withOutCurrentAuthorizedUser
                className="people-selector"
                isMultiSelect={false}
                onAccept={selectOption}
                onBackClick={onArrowClick}
                headerLabel={selectorLabel}
              />
            ) : (
              <RoomSelector
                className="people-selector"
                isMultiSelect={false}
                onAccept={selectOption}
                onBackClick={onArrowClick}
                headerLabel={selectorLabel}
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
            <Heading size="medium">{filterHeader}</Heading>
            {showFooter && (
              <IconButton
                id="filter_search-options-clear"
                iconName={ClearReactSvgUrl}
                isFill={true}
                onClick={onClearFilter}
                size={17}
              />
            )}
          </StyledFilterBlockHeader>
          <div className="filter-body">
            {isLoading ? (
              <Loaders.FilterBlock isRooms={isRooms} isAccounts={isAccounts} />
            ) : (
              <Scrollbar className="filter-body__scrollbar" stype="mediumBlack">
                {filterData.map((item, index) => {
                  return (
                    <FilterBlockItem
                      key={item.key}
                      label={item.label}
                      keyProp={item.key}
                      group={item.group}
                      groupItem={item.groupItem}
                      isLast={item.isLast}
                      isFirst={index === 0}
                      withoutHeader={item.withoutHeader}
                      withoutSeparator={item.withoutSeparator}
                      changeFilterValue={changeFilterValue}
                      showSelector={changeShowSelector}
                      withMultiItems={item.withMultiItems}
                    />
                  );
                })}
              </Scrollbar>
            )}
          </div>
          {showFooter && (
            <StyledFilterBlockFooter>
              <Button
                id="filter_apply-button"
                size="normal"
                primary={true}
                label={t("Common:ApplyButton")}
                scale={true}
                onClick={onFilterAction}
              />
              <Button
                id="filter_cancel-button"
                size="normal"
                label={t("Common:CancelButton")}
                scale={true}
                onClick={hideFilterBlock}
              />
            </StyledFilterBlockFooter>
          )}

          <StyledControlContainer id="filter_close" onClick={hideFilterBlock}>
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

  const renderPortalFilterBlock = () => {
    const rootElement = document.getElementById("root");

    return (
      <Portal
        element={filterBlockComponent}
        appendTo={rootElement}
        visible={true}
      />
    );
  };

  return isMobileOnly ? renderPortalFilterBlock() : filterBlockComponent;
};

export default React.memo(withTranslation("Common")(FilterBlock));
