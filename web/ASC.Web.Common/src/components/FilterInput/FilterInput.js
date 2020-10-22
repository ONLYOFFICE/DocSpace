import React from "react";
import PropTypes from "prop-types";
import { SearchInput, utils } from "asc-web-components";
import isEqual from "lodash/isEqual";
import FilterBlock from "./sub-components/FilterBlock";
import SortComboBox from "./sub-components/SortComboBox";
import ViewSelector from "./sub-components/ViewSelector";
import map from "lodash/map";
import clone from "lodash/clone";
import StyledFilterInput from "./StyledFilterInput";

const { smallTablet } = utils.device.size;

const cloneObjectsArray = (props) => {
  return map(props, clone);
};

class FilterInput extends React.Component {
  constructor(props) {
    super(props);

    const { selectedFilterData, getSortData, value } = props;
    const { sortDirection, sortId, inputValue } = selectedFilterData;
    const sortData = getSortData();

    const filterValues = selectedFilterData ? this.getDefaultFilterData() : [];

    this.state = {
      sortDirection: sortDirection === "desc" ? true : false,
      sortId:
        sortData.findIndex((x) => x.key === sortId) != -1
          ? sortId
          : sortData.length > 0
          ? sortData[0].key
          : "",
      searchText: inputValue || value,
      filterValues,
      openFilterItems: [],
      hideFilterItems: [],
      overflowFilter: true,
      filterMaxWidth: 0,
      showHiddenFilter: false,
    };

    this.searchWrapper = React.createRef();
    this.filterWrapper = React.createRef();
  }

  componentDidMount() {
    if (this.state.filterValues.length > 0) {
      this.setState(
        {
          filterValues: cloneObjectsArray(this.state.filterValues),
          openFilterItems: cloneObjectsArray(this.state.filterValues),
          overflowFilter: false,
        },
        () => this.updateFilter(cloneObjectsArray(this.state.filterValues))
      );
    }
  }

  componentDidUpdate(prevProps) {
    const { selectedFilterData, widthProp, getSortData, value } = this.props;
    const { filterValues } = selectedFilterData;

    if (!isEqual(selectedFilterData, prevProps.selectedFilterData)) {
      const newFilterValues = this.getDefaultFilterData();
      const { sortDirection, sortId, inputValue } = selectedFilterData;
      const sortData = getSortData();

      this.setState(
        {
          filterValues: newFilterValues,
          openFilterItems: newFilterValues,
          overflowFilter: false,
          sortDirection: sortDirection === "desc" ? true : false,
          sortId:
            sortData.findIndex((x) => x.key === sortId) != -1
              ? sortId
              : sortData.length > 0
              ? sortData[0].key
              : "",
          searchText: inputValue || value,
        },
        () => this.updateFilter(newFilterValues)
      );
    }

    if (
      selectedFilterData &&
      filterValues.length !==
        prevProps.selectedFilterData.filterValues.length &&
      !filterValues.length
    ) {
      this.clearFilter();
    }

    if (widthProp !== prevProps.widthProp) {
      this.resize();
    }
  }

  shouldComponentUpdate(nextProps, nextState) {
    const {
      selectedFilterData,
      value,
      id,
      isDisabled,
      size,
      placeholder,
    } = this.props;

    if (!isEqual(selectedFilterData, nextProps.selectedFilterData)) {
      return true;
    }

    if (this.props.viewAs !== nextProps.viewAs) {
      return true;
    }

    if (this.props.widthProp !== nextProps.widthProp) {
      return true;
    }

    if (
      id != nextProps.id ||
      isDisabled != nextProps.isDisabled ||
      size != nextProps.size ||
      placeholder != nextProps.placeholder ||
      value != nextProps.value
    ) {
      return true;
    }

    return !isEqual(this.state, nextState);
  }

  resize = () => {
    if (!this.state.filterValues.length) return;
    const filterValues = cloneObjectsArray(this.state.filterValues);
    const fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
    const filterMaxWidth = (fullWidth * 45) / 100 + 50;
    this.setState(
      {
        filterValues,
        openFilterItems: filterValues,
        overflowFilter: false,
        filterMaxWidth,
      },
      () => this.updateFilter()
    );
  };

  getDefaultFilterData = () => {
    const { getFilterData, selectedFilterData } = this.props;
    const filterData = getFilterData();
    const filterItems = [];
    const filterValues = cloneObjectsArray(selectedFilterData.filterValues);

    for (let item of filterValues) {
      const filterValue = filterData.find(
        (x) => x.key === item.key && x.group === item.group
      );

      if (!filterValue) {
        const isSelector = item.group.includes("filter-author");

        if (isSelector) {
          const typeSelector = item.key.includes("user")
            ? "user"
            : item.key.includes("group")
            ? "group"
            : null;
          const underlined = item.key.indexOf("_") !== -1;
          const key = underlined
            ? item.key.slice(0, item.key.indexOf("_"))
            : item.key;
          const filesFilterValue = filterData.find(
            (x) => x.key === key && x.group === item.group
          );

          if (filesFilterValue) {
            const convertedItem = {
              key: item.group + "_" + item.key,
              label: filesFilterValue.label,
              group: item.group,
              groupLabel: filesFilterValue.label,
              typeSelector,
              groupsCaption: filesFilterValue.groupsCaption,
              defaultOptionLabel: filesFilterValue.defaultOptionLabel,
              defaultOption: filesFilterValue.defaultOption,
              defaultSelectLabel: filesFilterValue.defaultSelectLabel,
              selectedItem: filesFilterValue.selectedItem,
            };
            filterItems.push(convertedItem);
          }
        }
      }

      let groupLabel = "";
      const groupFilterItem = filterData.find((x) => x.key === item.group);
      if (groupFilterItem) {
        groupLabel = groupFilterItem.label;
      } else {
        const subgroupFilterItem = filterData.find(
          (x) => x.subgroup === item.group
        );
        if (subgroupFilterItem) {
          groupLabel = subgroupFilterItem.label;
        }
      }

      if (filterValue) {
        item.key = item.group + "_" + item.key;
        item.label = filterValue.selectedItem
          ? filterValue.selectedItem.label
          : filterValue.label;
        item.groupLabel = groupLabel;
        filterItems.push(item);
      }
    }

    return filterItems;
  };

  onChangeSortDirection = (key) => {
    this.onFilter(
      this.state.filterValues,
      this.state.sortId,
      key ? "desc" : "asc"
    );
    this.setState({ sortDirection: !!key });
  };

  onClickViewSelector = (item) => {
    const itemId = (item.target && item.target.dataset.for) || item;
    const viewAs = itemId.indexOf("row") === -1 ? "tile" : "row";
    this.props.onChangeViewAs(viewAs);
  };

  onClickSortItem = (key) => {
    this.setState({ sortId: key });
    this.onFilter(
      this.state.filterValues,
      key,
      this.state.sortDirection ? "desc" : "asc"
    );
  };

  onSortDirectionClick = () => {
    this.onFilter(
      this.state.filterValues,
      this.state.sortId,
      !this.state.sortDirection ? "desc" : "asc"
    );
    this.setState({ sortDirection: !this.state.sortDirection });
  };

  onSearchChanged = (value) => {
    this.setState({ searchText: value });
    this.onFilter(
      this.state.filterValues,
      this.state.sortId,
      this.state.sortDirection ? "desc" : "asc",
      value
    );
  };

  onSearch = (result) => {
    this.onFilter(
      result.filterValues,
      this.state.sortId,
      this.state.sortDirection ? "desc" : "asc"
    );
  };

  getFilterData = () => {
    const d = this.props.getFilterData();
    const result = [];
    d.forEach((element) => {
      if (!element.inSubgroup) {
        element.onClick =
          !element.isSeparator && !element.isHeader && !element.disabled
            ? () => this.onClickFilterItem(element)
            : undefined;
        element.key =
          element.group != element.key
            ? element.group + "_" + element.key
            : element.key;
        if (element.subgroup != undefined) {
          if (d.findIndex((x) => x.group === element.subgroup) === -1)
            element.disabled = true;
        }
        result.push(element);
      }
    });
    return result;
  };

  clearFilter = () => {
    this.setState({
      searchText: "",
      filterValues: [],
      openFilterItems: [],
      hideFilterItems: [],
    });
    this.onFilter(
      [],
      this.state.sortId,
      this.state.sortDirection ? "desc" : "asc",
      ""
    );
  };

  updateFilter = (inputFilterItems) => {
    const currentFilterItems = inputFilterItems
      ? cloneObjectsArray(inputFilterItems)
      : cloneObjectsArray(this.state.filterValues);
    const fullWidth = this.searchWrapper.current.getBoundingClientRect().width;

    const filterArr = Array.from(
      Array.from(this.filterWrapper.current.children).find(
        (x) => x.id === "filter-items-container"
      ).children
    );

    let newOpenFilterItems = currentFilterItems;
    let newHideFilterItems = [];

    let elementsWidth = 0;

    const withHiddenItems = currentFilterItems.length > 0;
    for (let elem of filterArr) {
      if (elem.className !== "styled-hide-filter" && withHiddenItems) {
        elementsWidth += elem.getBoundingClientRect().width;
      }
    }

    for (let i = 0; i < filterArr.length; i++) {
      const elementWidth = filterArr[i].getBoundingClientRect().width;
      if (elementsWidth + 30 >= (fullWidth * 45) / 100) {
        const hiddenItem = currentFilterItems.find(
          (x) => x.key === filterArr[i].getAttribute("id")
        );
        if (hiddenItem) {
          elementsWidth -= elementWidth;
          newHideFilterItems.push(hiddenItem);
          newOpenFilterItems = newOpenFilterItems.filter(
            (x) => x.key !== filterArr[i].getAttribute("id")
          );
        }
      }
    }

    this.setState({
      filterValues: currentFilterItems,
      openFilterItems: newOpenFilterItems,
      hideFilterItems: newHideFilterItems,
      overflowFilter: true,
      showHiddenFilter: false,
    });
  };

  onDeleteFilterItem = (key) => {
    const filterItems = this.state.filterValues.slice();
    const indexFilterItem = filterItems.findIndex((x) => x.key === key);
    if (indexFilterItem != -1) {
      filterItems.splice(indexFilterItem, 1);
    }

    this.setState(
      {
        filterValues: filterItems,
        openFilterItems: filterItems,
        overflowFilter: false,
      },
      () => this.updateFilter(filterItems)
    );

    let filterValues = cloneObjectsArray(filterItems);
    filterValues = filterValues.map((item) => {
      item.key = item.key.replace(item.group + "_", "");
      return item;
    });
    this.onFilter(
      filterValues.filter((item) => item.key != "-1"),
      this.state.sortId,
      this.state.sortDirection ? "desc" : "asc"
    );
  };

  onFilter = (filterValues, sortId, sortDirection, searchText) => {
    let cloneFilterValues = cloneObjectsArray(filterValues);

    cloneFilterValues = cloneFilterValues.map((item) => {
      item.key = item.key.replace(item.group + "_", "");
      return item;
    });

    this.props.onFilter({
      inputValue: searchText != undefined ? searchText : this.state.searchText,
      filterValues: cloneFilterValues,
      sortId,
      sortDirection,
    });
  };

  onChangeFilter = (result) => {
    this.onFilter(
      result.filterValues,
      this.state.sortId,
      this.state.sortDirection ? "desc" : "asc",
      result.inputValue
    );
  };

  onClickFilterItem = (filterItem) => {
    const { filterValues, sortId, sortDirection } = this.state;
    const currentFilterItems = cloneObjectsArray(filterValues);

    if (filterItem.isSelector) {
      const indexFilterItem = currentFilterItems.findIndex(
        (x) => x.group === filterItem.group
      );
      if (indexFilterItem != -1) {
        currentFilterItems.splice(indexFilterItem, 1);
      }
      const typeSelector = filterItem.key.includes("user")
        ? "user"
        : filterItem.key.includes("group")
        ? "group"
        : null;
      const itemId =
        filterItem.key.indexOf("_") !== filterItem.key.lastIndexOf("_")
          ? filterItem.key.slice(0, filterItem.key.lastIndexOf("_"))
          : filterItem.key;
      const itemKey =
        filterItem.selectedItem &&
        filterItem.selectedItem.key &&
        filterItem.typeSelector === typeSelector
          ? itemId + "_" + filterItem.selectedItem.key
          : filterItem.key + "_-1";
      const selectedItem =
        filterItem.typeSelector === typeSelector ? filterItem.selectedItem : {};
      const selectFilterItem = {
        key: itemKey,
        group: filterItem.group,
        label: filterItem.label,
        groupLabel: filterItem.label,
        typeSelector,
        defaultOption: filterItem.defaultOption,
        groupsCaption: filterItem.groupsCaption,
        defaultOptionLabel: filterItem.defaultOptionLabel,
        defaultSelectLabel: filterItem.defaultSelectLabel,
        selectedItem,
      };

      currentFilterItems.push(selectFilterItem);
      this.setState(
        {
          filterValues: currentFilterItems,
          openFilterItems: currentFilterItems,
          overflowFilter: false,
        },
        () => this.updateFilter(currentFilterItems)
      );

      if (selectFilterItem.selectedItem.key) {
        const clone = cloneObjectsArray(
          currentFilterItems.filter((item) => item.key != "-1")
        );
        clone.map((item) => {
          item.key = item.key.replace(item.group + "_", "");
          return item;
        });

        this.onFilter(clone, sortId, sortDirection ? "desc" : "asc");
      }

      return;
    } else if (filterItem.subgroup) {
      const indexFilterItem = currentFilterItems.findIndex(
        (x) => x.group === filterItem.subgroup
      );
      if (indexFilterItem != -1) {
        currentFilterItems.splice(indexFilterItem, 1);
      }
      const subgroupItems = this.props
        .getFilterData()
        .filter((t) => t.group === filterItem.subgroup);
      if (subgroupItems.length > 1) {
        const selectFilterItem = {
          key: filterItem.subgroup + "_-1",
          group: filterItem.subgroup,
          label: filterItem.defaultSelectLabel,
          groupLabel: filterItem.label,
          inSubgroup: true,
        };
        if (indexFilterItem != -1) {
          currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
        } else {
          currentFilterItems.push(selectFilterItem);
          this.setState(
            {
              filterValues: currentFilterItems,
              openFilterItems: currentFilterItems,
              overflowFilter: false,
            },
            () => this.updateFilter(currentFilterItems)
          );
        }
      } else if (subgroupItems.length === 1) {
        const selectFilterItem = {
          key: subgroupItems[0].group + "_" + subgroupItems[0].key,
          group: subgroupItems[0].group,
          label: subgroupItems[0].label,
          groupLabel: this.props
            .getFilterData()
            .find((x) => x.subgroup === subgroupItems[0].group).label,
          inSubgroup: true,
        };
        if (indexFilterItem != -1) {
          currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
        } else {
          currentFilterItems.push(selectFilterItem);
        }

        const clone = cloneObjectsArray(
          currentFilterItems.filter((item) => item.key != "-1")
        );
        clone.map((item) => {
          item.key = item.key.replace(item.group + "_", "");
          return item;
        });
        this.onFilter(clone, sortId, sortDirection ? "desc" : "asc");
      }

      this.setState(
        {
          filterValues: currentFilterItems,
          openFilterItems: currentFilterItems,
          overflowFilter: false,
        },
        () => this.updateFilter(currentFilterItems)
      );
    } else {
      const filterItems = this.getFilterData();

      const indexFilterItem = currentFilterItems.findIndex(
        (x) => x.group === filterItem.group
      );
      if (indexFilterItem != -1) {
        currentFilterItems.splice(indexFilterItem, 1);
      }

      const selectFilterItem = {
        key: filterItem.key,
        group: filterItem.group,
        label: filterItem.label,
        groupLabel: filterItem.inSubgroup
          ? filterItems.find((x) => x.subgroup === filterItem.group).label
          : filterItems.find((x) => x.key === filterItem.group).label,
      };
      if (indexFilterItem != -1)
        currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
      else {
        currentFilterItems.push(selectFilterItem);
      }

      const clone = cloneObjectsArray(
        currentFilterItems.filter((item) => item.key != "-1")
      );
      clone.map((item) => {
        item.key = item.key.replace(item.group + "_", "");
        return item;
      });

      this.onFilter(clone, sortId, sortDirection ? "desc" : "asc");
    }
  };

  setShowHiddenFilter = (showHiddenFilter) => {
    this.setState({ showHiddenFilter });
  };

  render() {
    //console.log("FilterInput render");
    const {
      className,
      id,
      style,
      size,
      isDisabled,
      scale,
      getFilterData,
      placeholder,
      getSortData,
      directionAscLabel,
      directionDescLabel,
      filterColumnCount,
      viewAs,
      contextMenuHeader,
      isMobile,
      widthProp,
    } = this.props;
    const {
      searchText,
      filterValues,
      openFilterItems,
      hideFilterItems,
      sortId,
      sortDirection,
      overflowFilter,
      filterMaxWidth,
      showHiddenFilter,
    } = this.state;

    let iconSize = 30;
    switch (size) {
      case "base":
        iconSize = 30;
        break;
      case "middle":
      case "big":
      case "huge":
        iconSize = 41;
        break;
      default:
        break;
    }

    const isMinimized = widthProp <= smallTablet;

    return (
      <StyledFilterInput
        filterMaxWidth={filterMaxWidth}
        overflowFilter={overflowFilter}
        isMobile={isMobile || isMinimized}
        viewAs={viewAs}
        className={className}
        id={id}
        style={style}
      >
        <div className="styled-search-input" ref={this.searchWrapper}>
          <SearchInput
            id={id}
            isDisabled={isDisabled}
            size={size}
            scale={scale}
            isNeedFilter={true}
            getFilterData={getFilterData}
            placeholder={placeholder}
            onSearchClick={this.onSearch}
            onChangeFilter={this.onChangeFilter}
            value={searchText}
            selectedFilterData={filterValues}
            showClearButton={filterValues.length > 0}
            onClearSearch={this.clearFilter}
            onChange={this.onSearchChanged}
          >
            <div className="styled-filter-block" ref={this.filterWrapper}>
              <FilterBlock
                contextMenuHeader={contextMenuHeader}
                openFilterItems={openFilterItems}
                hideFilterItems={hideFilterItems}
                iconSize={iconSize}
                getFilterData={getFilterData}
                onClickFilterItem={this.onClickFilterItem}
                onDeleteFilterItem={this.onDeleteFilterItem}
                isDisabled={isDisabled}
                columnCount={filterColumnCount}
                showHiddenFilter={showHiddenFilter}
                setShowHiddenFilter={this.setShowHiddenFilter}
                isMinimized={isMinimized}
              />
            </div>
          </SearchInput>
        </div>

        <SortComboBox
          sortId={sortId}
          getSortData={getSortData}
          isDisabled={isDisabled}
          onChangeSortId={this.onClickSortItem}
          onChangeView={this.onClickViewSelector}
          onChangeSortDirection={this.onChangeSortDirection}
          onButtonClick={this.onSortDirectionClick}
          viewAs={viewAs}
          sortDirection={+sortDirection}
          directionAscLabel={directionAscLabel}
          directionDescLabel={directionDescLabel}
        />

        {viewAs && (
          <ViewSelector
            isDisabled={isDisabled}
            onClickViewSelector={this.onClickViewSelector}
            viewAs={viewAs}
          />
        )}
      </StyledFilterInput>
    );
  }
}

FilterInput.propTypes = {
  size: PropTypes.oneOf(["base", "middle", "big", "huge"]),
  selectedFilterData: PropTypes.object,
  directionAscLabel: PropTypes.string,
  directionDescLabel: PropTypes.string,
  viewAs: PropTypes.bool, // TODO: include viewSelector after adding method getThumbnail - PropTypes.string
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  filterColumnCount: PropTypes.number,
  onChangeViewAs: PropTypes.func,
  contextMenuHeader: PropTypes.string,
  getSortData: PropTypes.func,
  getFilterData: PropTypes.func,
  isDisabled: PropTypes.bool,
  scale: PropTypes.bool,
  placeholder: PropTypes.string,
  isMobile: PropTypes.bool,
  value: PropTypes.string,
  widthProp: PropTypes.number,
  onFilter: PropTypes.func,
};

FilterInput.defaultProps = {
  selectedFilterData: {
    sortDirection: false,
    sortId: "",
    filterValues: [],
    searchText: "",
  },
  size: "base",
  directionAscLabel: "A-Z",
  directionDescLabel: "Z-A",
};

export default FilterInput;
