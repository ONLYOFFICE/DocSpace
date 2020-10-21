import React from "react";
import FilterButton from "./FilterButton";
import HideFilter from "./HideFilter";
import FilterItem from "./FilterItem";
import PropTypes from "prop-types";
import isEqual from "lodash/isEqual";

class FilterBlock extends React.Component {
  getFilterItems = () => {
    const {
      openFilterItems,
      hideFilterItems,
      onClickFilterItem,
      isDisabled,
      getFilterData,
      onDeleteFilterItem,
      setShowHiddenFilter,
    } = this.props;

    let result = [];
    let openItems = [];
    let hideItems = [];

    const filterData = getFilterData();

    if (openFilterItems.length > 0) {
      openItems = openFilterItems.map((item) => {
        const {
          key,
          group,
          groupLabel,
          label,
          typeSelector,
          groupsCaption,
          defaultOptionLabel,
          defaultOption,
          defaultSelectLabel,
          selectedItem,
        } = item;

        return (
          <FilterItem
            block={false}
            isDisabled={isDisabled}
            key={key}
            groupItems={filterData.filter(
              (t) => t.group == group && t.group != t.key
            )}
            onSelectFilterItem={onClickFilterItem}
            id={key}
            groupLabel={groupLabel}
            label={label}
            onClose={onDeleteFilterItem}
            typeSelector={typeSelector}
            groupsCaption={groupsCaption}
            defaultOptionLabel={defaultOptionLabel}
            defaultOption={defaultOption}
            defaultSelectLabel={defaultSelectLabel}
            selectedItem={selectedItem}
            opened={key.indexOf("_-1") == -1 ? false : true}
          ></FilterItem>
        );
      });
    }

    if (hideFilterItems.length > 0) {
      let open = false;
      const hideFilterItemsList = hideFilterItems.map((item) => {
        const {
          key,
          group,
          groupLabel,
          label,
          typeSelector,
          groupsCaption,
          defaultOptionLabel,
          defaultOption,
          defaultSelectLabel,
          selectedItem,
        } = item;

        open = key.indexOf("_-1") == -1 ? false : true;

        return (
          <FilterItem
            block={true}
            isDisabled={isDisabled}
            key={key}
            groupItems={filterData.filter(
              (t) => t.group == group && t.group != t.key
            )}
            onSelectFilterItem={onClickFilterItem}
            id={key}
            groupLabel={groupLabel}
            label={label}
            onClose={onDeleteFilterItem}
            typeSelector={typeSelector}
            groupsCaption={groupsCaption}
            defaultOptionLabel={defaultOptionLabel}
            defaultOption={defaultOption}
            defaultSelectLabel={defaultSelectLabel}
            selectedItem={selectedItem}
            opened={open}
            setShowHiddenFilter={setShowHiddenFilter}
          ></FilterItem>
        );
      });

      hideItems.push(
        <HideFilter
          key="hide-filter"
          count={hideFilterItems.length}
          isDisabled={isDisabled}
          open={open ? open : this.props.showHiddenFilter}
          setShowHiddenFilter={setShowHiddenFilter}
        >
          {hideFilterItemsList}
        </HideFilter>
      );
    }

    result = hideItems.concat(openItems);
    return result;
  };

  getData = () => {
    const { getFilterData, onClickFilterItem } = this.props;
    const d = getFilterData();
    let result = [];
    d.forEach((element) => {
      if (!element.inSubgroup) {
        element.onClick =
          !element.isSeparator && !element.isHeader && !element.disabled
            ? () => onClickFilterItem(element)
            : undefined;
        element.key =
          element.group != element.key
            ? element.group + "_" + element.key
            : element.key;
        if (element.subgroup != undefined) {
          if (d.findIndex((x) => x.group === element.subgroup) == -1)
            element.disabled = true;
        }
        result.push(element);
      }
    });
    return result;
  };

  shouldComponentUpdate(nextProps) {
    if (!isEqual(this.props, nextProps)) {
      return true;
    }
    return false;
  }

  render() {
    //console.log("FilterBlock render");
    const {
      iconSize,
      isDisabled,
      contextMenuHeader,
      getFilterData,
      columnCount,
    } = this.props;
    const filterItems = this.getFilterItems();
    const filterData = getFilterData();
    return (
      <>
        <div className="styled-filter-block" id="filter-items-container">
          {filterItems}
        </div>
        {filterData.length > 0 && (
          <FilterButton
            columnCount={columnCount}
            id="filter-button"
            iconSize={iconSize}
            getData={this.getData}
            isDisabled={isDisabled}
            asideHeader={contextMenuHeader}
          />
        )}
      </>
    );
  }
}

FilterBlock.propTypes = {
  getFilterData: PropTypes.func,
  hideFilterItems: PropTypes.array,
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isDisabled: PropTypes.bool,
  onDeleteFilterItem: PropTypes.func,
  openFilterItems: PropTypes.array,
  columnCount: PropTypes.number,
  contextMenuHeader: PropTypes.string,
  onClickFilterItem: PropTypes.func,
  setShowHiddenFilter: PropTypes.func,
};

export default FilterBlock;
