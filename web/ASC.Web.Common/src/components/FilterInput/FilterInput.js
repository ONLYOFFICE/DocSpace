import React from 'react';
import PropTypes from 'prop-types';
import { SearchInput } from 'asc-web-components';
import isEqual from 'lodash/isEqual';
import throttle from 'lodash/throttle';
import FilterBlock from './sub-components/FilterBlock';
import SortComboBox from './sub-components/SortComboBox';
import ViewSelector from './sub-components/ViewSelector';
import map from 'lodash/map';
import clone from 'lodash/clone';
import StyledFilterInput from './StyledFilterInput';

const cloneObjectsArray = function (props) {
    return map(props, clone);
}
const convertToInternalData = function (fullDataArray, inputDataArray) {
    const filterItems = [];
    for (let i = 0; i < inputDataArray.length; i++) {
        let filterValue = fullDataArray.find(x => ((x.key === inputDataArray[i].key.replace(inputDataArray[i].group + "_", '')) && x.group === inputDataArray[i].group && !x.inSubgroup));
        if (filterValue) {
            inputDataArray[i].key = inputDataArray[i].group + "_" + inputDataArray[i].key;
            inputDataArray[i].label = filterValue.label;
            inputDataArray[i].groupLabel = !fullDataArray.inSubgroup ? fullDataArray.find(x => (x.group === inputDataArray[i].group)).label : inputDataArray[i].groupLabel;
            filterItems.push(inputDataArray[i]);
        } else {
            filterValue = fullDataArray.find(x => ((x.key === inputDataArray[i].key.replace(inputDataArray[i].group + "_", '')) && x.group === inputDataArray[i].group && x.inSubgroup));
            if (filterValue) {
                inputDataArray[i].key = inputDataArray[i].group + "_" + inputDataArray[i].key;
                inputDataArray[i].label = filterValue.label;
                inputDataArray[i].groupLabel = fullDataArray.find(x => (x.subgroup === inputDataArray[i].group)).label;
                filterItems.push(inputDataArray[i]);
            } else {
                filterValue = fullDataArray.find(x => ((x.subgroup === inputDataArray[i].group)));
                if (filterValue) {
                    const subgroupItems = fullDataArray.filter(t => t.group === filterValue.subgroup);
                    if (subgroupItems.length > 1) {
                        inputDataArray[i].key = inputDataArray[i].group + "_-1";
                        inputDataArray[i].label = filterValue.defaultSelectLabel;
                        inputDataArray[i].groupLabel = fullDataArray.find(x => (x.subgroup === inputDataArray[i].group)).label;
                        filterItems.push(inputDataArray[i]);
                    } else if (subgroupItems.length === 1) {

                        const selectFilterItem = {
                            key: subgroupItems[0].group + "_" + subgroupItems[0].key,
                            group: subgroupItems[0].group,
                            label: subgroupItems[0].label,
                            groupLabel: fullDataArray.find(x => x.subgroup === subgroupItems[0].group).label,
                            inSubgroup: true
                        };
                        filterItems.push(selectFilterItem);
                    }
                }
            }
        }
    }
    return filterItems;
}

class FilterInput extends React.Component {
    constructor(props) {
        super(props);

        this.isResizeUpdate = false;
        this.minWidth = 190;

        function getDefaultFilterData() {
            const filterData = props.getFilterData();
            const filterItems = [];
            const selectedFilterData = cloneObjectsArray(props.selectedFilterData.filterValues);
            selectedFilterData.forEach(defaultFilterValue => {
                const filterValue = filterData.find(x => ((x.key === defaultFilterValue.key.replace(defaultFilterValue.group + "_", '')) && x.group === defaultFilterValue.group));
                let groupLabel = '';

                const groupFilterItem = filterData.find(x => (x.key === defaultFilterValue.group));
                if (groupFilterItem != undefined) {
                    groupLabel = groupFilterItem.label;
                } else {
                    const subgroupFilterItem = filterData.find(x => (x.subgroup === defaultFilterValue.group))
                    if (subgroupFilterItem != undefined) {
                        groupLabel = subgroupFilterItem.label;
                    }
                }

                if (filterValue != undefined) {
                    defaultFilterValue.key = defaultFilterValue.group + "_" + defaultFilterValue.key;
                    defaultFilterValue.label = filterValue.label;
                    defaultFilterValue.groupLabel = groupLabel;
                    filterItems.push(defaultFilterValue);
                }
            });
            return filterItems;
        }

        this.state = {
            sortDirection: props.selectedFilterData.sortDirection === "desc" ? true : false,
            sortId: props.getSortData().findIndex(x => x.key === props.selectedFilterData.sortId) != -1 ? props.selectedFilterData.sortId : props.getSortData().length > 0 ? props.getSortData()[0].key : "",
            searchText: props.selectedFilterData.inputValue || props.value,

            filterValues: props.selectedFilterData ? getDefaultFilterData() : [],
            openFilterItems: [],
            hideFilterItems: []
        };

        this.searchWrapper = React.createRef();
        this.filterWrapper = React.createRef();

        this.onClickSortItem = this.onClickSortItem.bind(this);
        this.onSortDirectionClick = this.onSortDirectionClick.bind(this);
        this.onChangeSortDirection = this.onChangeSortDirection.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.onChangeFilter = this.onChangeFilter.bind(this);

        this.onSearchChanged = this.onSearchChanged.bind(this);

        this.getDefaultSelectedIndex = this.getDefaultSelectedIndex.bind(this);

        this.updateFilter = this.updateFilter.bind(this);
        this.onClickFilterItem = this.onClickFilterItem.bind(this);
        this.getFilterData = this.getFilterData.bind(this);
        this.onFilterRender = this.onFilterRender.bind(this);
        this.onDeleteFilterItem = this.onDeleteFilterItem.bind(this);
        this.clearFilter = this.clearFilter.bind(this);

        this.onClickViewSelector = this.onClickViewSelector.bind(this);

        this.throttledResize = throttle(this.resize, 300);

    }

    componentDidMount() {
        window.addEventListener('resize', this.throttledResize);
        if (this.state.filterValues.length > 0) this.updateFilter();
    }
    componentWillUnmount() {
        window.removeEventListener('resize', this.throttledResize);
    }
    componentDidUpdate(prevProps, prevState) {
        if (this.props.needForUpdate && this.props.needForUpdate(prevProps, this.props)) {
            let internalFilterData = convertToInternalData(this.props.getFilterData(), cloneObjectsArray(this.props.selectedFilterData.filterValues));
            this.updateFilter(internalFilterData);
        }
    }
    shouldComponentUpdate(nextProps, nextState) {
        const { selectedFilterData, getFilterData, getSortData, value, id,
            isDisabled, size, placeholder } = this.props;
        if (!isEqual(selectedFilterData, nextProps.selectedFilterData)) {
            let internalFilterData = cloneObjectsArray(this.state.filterValues);
            if (nextProps.selectedFilterData.filterValues) {
                internalFilterData = convertToInternalData(getFilterData(), cloneObjectsArray(nextProps.selectedFilterData.filterValues));
                let internalFilterDataSelectors = this.convertSelectorToInternalData(getFilterData(), cloneObjectsArray(nextProps.selectedFilterData.filterValues));
                internalFilterData = internalFilterData.concat(internalFilterDataSelectors);
                this.updateFilter(internalFilterData);
            }
            this.setState(
                {
                    sortDirection: nextProps.selectedFilterData.sortDirection === "desc" ? true : false,
                    sortId: getSortData().findIndex(x => x.key === nextProps.selectedFilterData.sortId) != -1 ? nextProps.selectedFilterData.sortId : "",
                    filterValues: internalFilterData,
                    searchText: nextProps.selectedFilterData.inputValue || value
                }
            );
            return true;
        }

        if(this.props.viewAs !== nextProps.viewAs){
            return true;
        }

        if (this.props.isReady !== nextProps.isReady) {
            let internalFilterData = cloneObjectsArray(this.state.filterValues);
            internalFilterData = convertToInternalData(getFilterData(), cloneObjectsArray(nextProps.selectedFilterData.filterValues));

            let internalFilterDataSelectors = this.convertSelectorToInternalData(getFilterData(), cloneObjectsArray(nextProps.selectedFilterData.filterValues));
            internalFilterData = internalFilterData.concat(internalFilterDataSelectors);
            this.updateFilter(internalFilterData);

            this.setState(
                {
                    sortDirection: nextProps.selectedFilterData.sortDirection === "desc" ? true : false,
                    sortId: getSortData().findIndex(x => x.key === nextProps.selectedFilterData.sortId) != -1 ? nextProps.selectedFilterData.sortId : "",
                    filterValues: internalFilterData,
                    searchText: nextProps.selectedFilterData.inputValue || value
                }
            );
            // return true;
        }

        if (id != nextProps.id ||
            isDisabled != nextProps.isDisabled ||
            size != nextProps.size ||
            placeholder != nextProps.placeholder ||
            value != nextProps.value)

            return true;
        if (this.isResizeUpdate) {
            return true;
        }
        return !isEqual(this.state, nextState);
    }

    convertSelectorToInternalData = (filterData, filterValues) => {
        const resultValues = [];
        filterValues.forEach((item) => {
            const isSelector = item.group.includes('filter-author');
            if (isSelector) {
                const typeSelector = item.key.includes('user') ? 'user'
                    : item.key.includes('group') ? 'group'
                        : null;
                const hasUnderscore = item.key.indexOf('_') !== -1;
                const key = hasUnderscore ? item.key.slice(0, item.key.indexOf('_')) : item.key;
                const finded = filterData.find(x => x.key === key && x.group === item.group);
                if (!finded) return;
                const convertedItem = {
                    key: item.group + '_' + item.key,
                    label: finded.label,
                    group: item.group,
                    groupLabel: finded.label,
                    typeSelector,
                    groupsCaption: finded.groupsCaption,
                    defaultOptionLabel: finded.defaultOptionLabel,
                    defaultOption: finded.defaultOption,
                    defaultSelectLabel: finded.defaultSelectLabel,
                    selectedItem: finded.selectedItem,
                };
                resultValues.push(convertedItem);
            }
        });
        return resultValues;
    }

    resize = () => {
        this.isResizeUpdate = true;
        this.setState({
            filterValues: this.state.filterValues,
            openFilterItems: this.state.filterValues,
            hideFilterItems: []
        })
    }
    onChangeSortDirection(key) {
        this.onFilter(this.state.filterValues, this.state.sortId, key ? "desc" : "asc");
        this.setState({ sortDirection: !!key });
    }
    onClickViewSelector(item) {
        const itemId = (item.target && item.target.dataset.for) || item;
        const viewAs = itemId.indexOf("row") === -1 ? "tile" : "row"
        this.props.onChangeViewAs(viewAs);
    }
    getDefaultSelectedIndex() {
        const sortData = this.props.getSortData();
        if (sortData.length > 0) {
            const defaultIndex = sortData.findIndex(x => x.key === this.state.sortId);
            return defaultIndex != -1 ? defaultIndex : 0;
        }
        return 0;
    }
    onClickSortItem(key) {
        this.setState({ sortId: key });
        this.onFilter(this.state.filterValues, key, this.state.sortDirection ? "desc" : "asc");
    }
    onSortDirectionClick() {

        this.onFilter(this.state.filterValues, this.state.sortId, !this.state.sortDirection ? "desc" : "asc");
        this.setState({ sortDirection: !this.state.sortDirection });
    }
    onSearchChanged(value) {
        this.setState({ searchText: value });
        this.onFilter(this.state.filterValues, this.state.sortId, this.state.sortDirection ? "desc" : "asc", value);
    }
    onSearch(result) {
        this.onFilter(result.filterValues, this.state.sortId, this.state.sortDirection ? "desc" : "asc");
    }
    getFilterData() {
        const _this = this;
        const d = this.props.getFilterData();
        const result = [];
        d.forEach(element => {
            if (!element.inSubgroup) {
                element.onClick = !element.isSeparator && !element.isHeader && !element.disabled ? ((e) => _this.props.onClickFilterItem(e, element)) : undefined;
                element.key = element.group != element.key ? element.group + "_" + element.key : element.key;
                if (element.subgroup != undefined) {
                    if (d.findIndex(x => x.group === element.subgroup) === -1) element.disabled = true;
                }
                result.push(element);
            }
        });
        return result;
    }
    clearFilter() {
        this.setState({
            searchText: '',
            filterValues: [],
            openFilterItems: [],
            hideFilterItems: []
        });
        this.onFilter([], this.state.sortId, this.state.sortDirection ? "desc" : "asc", '');
    }
    updateFilter(inputFilterItems) {
        const currentFilterItems = inputFilterItems || cloneObjectsArray(this.state.filterValues);
        const fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
        const filterWidth = this.filterWrapper.current.getBoundingClientRect().width;
        const filterArr = Array.from(Array.from(this.filterWrapper.current.children).find(x => x.id === 'filter-items-container').children);
        const searchFilterButton = Array.from(this.filterWrapper.current.children).find(x => x.id != 'filter-items-container');

        const filterButton = searchFilterButton ? Array.from(searchFilterButton.children)[0] : null;

        if (fullWidth <= this.minWidth && fullWidth > 0) {
            this.setState({
                openFilterItems: [],
                hideFilterItems: cloneObjectsArray(currentFilterItems)
            });
        } else if (filterWidth > fullWidth / 2) {
            let newOpenFilterItems = cloneObjectsArray(currentFilterItems);
            let newHideFilterItems = [];

            let elementsWidth = 0;
            Array.from(filterArr).forEach(element => {
                elementsWidth = elementsWidth + element.getBoundingClientRect().width;
            });

            if (filterButton !== null && (elementsWidth >= (fullWidth / 3) - filterButton.getBoundingClientRect().width)) {
                for (let i = 0; i < filterArr.length; i++) {
                    if (elementsWidth > (fullWidth / 3) - filterButton.getBoundingClientRect().width) {
                        elementsWidth = elementsWidth - filterArr[i].getBoundingClientRect().width;
                        const hiddenItem = currentFilterItems.find(x => x.key === filterArr[i].getAttribute('id'));
                        if (hiddenItem) newHideFilterItems.push(hiddenItem);
                        newOpenFilterItems.splice(newOpenFilterItems.findIndex(x => x.key === filterArr[i].getAttribute('id')), 1);
                    }
                }
            }
            this.setState({
                openFilterItems: newOpenFilterItems,
                hideFilterItems: newHideFilterItems
            });

        } else {

            this.setState({
                openFilterItems: currentFilterItems.slice(),
                hideFilterItems: []
            });
        }
    }
    onDeleteFilterItem(key) {
        const currentFilterItems = this.state.filterValues.slice();
        const indexFilterItem = currentFilterItems.findIndex(x => x.key === key);
        if (indexFilterItem != -1) {
            currentFilterItems.splice(indexFilterItem, 1);
        }
        this.setState({
            filterValues: currentFilterItems,
            openFilterItems: currentFilterItems,
            hideFilterItems: []
        });
        let filterValues = cloneObjectsArray(currentFilterItems);
        filterValues = filterValues.map(function (item) {
            item.key = item.key.replace(item.group + "_", '');
            return item;
        })
        this.onFilter(filterValues.filter(item => item.key != '-1'), this.state.sortId, this.state.sortDirection ? "desc" : "asc");
    }
    onFilter(filterValues, sortId, sortDirection, searchText) {
        let cloneFilterValues = cloneObjectsArray(filterValues);
        cloneFilterValues = cloneFilterValues.map(function (item) {
            item.key = item.key.replace(item.group + "_", '');
            return item;
        })
        this.props.onFilter({
            inputValue: searchText != undefined ? searchText : this.state.searchText,
            filterValues: cloneFilterValues.filter(item => item.key != '-1'),
            sortId: sortId,
            sortDirection: sortDirection
        });
    }
    onChangeFilter(result) {
        this.setState({
            searchText: result.inputValue,
            filterValues: result.filterValues,
        });
        this.onFilter(result.filterValues, this.state.sortId, this.state.sortDirection ? "desc" : "asc", result.inputValue);
    }
    onFilterRender() {
        if (this.isResizeUpdate) {
            this.isResizeUpdate = false;
        }

        if (this.searchWrapper.current && this.filterWrapper.current) {
            const fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
            const filterWidth = this.filterWrapper.current.getBoundingClientRect().width;
            if (fullWidth <= this.minWidth || filterWidth > fullWidth / 2) this.updateFilter();
        }
    }
    onClickFilterItem(event, filterItem) {
        const currentFilterItems = cloneObjectsArray(this.state.filterValues);


        if (filterItem.isSelector) {
            const indexFilterItem = currentFilterItems.findIndex(x => x.group === filterItem.group);
            if (indexFilterItem != -1) {
                currentFilterItems.splice(indexFilterItem, 1);
            }
            const typeSelector = filterItem.key.includes('user') ? 'user'
                : filterItem.key.includes('group') ? 'group'
                    : null;
            const itemId = filterItem.key.indexOf('_') !== filterItem.key.lastIndexOf('_') ? filterItem.key.slice(0, filterItem.key.lastIndexOf('_')) : filterItem.key;
            const itemKey = filterItem.selectedItem && filterItem.selectedItem.key && filterItem.typeSelector === typeSelector ? itemId + "_" + filterItem.selectedItem.key : filterItem.key + '_-1';
            const selectedItem = filterItem.typeSelector === typeSelector ? filterItem.selectedItem : {};
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
            this.setState({
                filterValues: currentFilterItems,
                openFilterItems: currentFilterItems,
                hideFilterItems: []
            });

            if (selectFilterItem.selectedItem.key) {
                const clone = cloneObjectsArray(currentFilterItems.filter(item => item.key != '-1'));
                clone.map(function (item) {
                    item.key = item.key.replace(item.group + "_", '');
                    return item;
                })


                this.onFilter(clone.filter(item => item.key != '-1'), this.state.sortId, this.state.sortDirection ? "desc" : "asc");
            }

            return;
        }

        if (filterItem.subgroup) {
            const indexFilterItem = currentFilterItems.findIndex(x => x.group === filterItem.subgroup);
            if (indexFilterItem != -1) {
                currentFilterItems.splice(indexFilterItem, 1);
            }
            const subgroupItems = this.props.getFilterData().filter(t => t.group === filterItem.subgroup);
            if (subgroupItems.length > 1) {
                const selectFilterItem = {
                    key: filterItem.subgroup + "_-1",
                    group: filterItem.subgroup,
                    label: filterItem.defaultSelectLabel,
                    groupLabel: filterItem.label,
                    inSubgroup: true
                };
                if (indexFilterItem != -1)
                    currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
                else
                    currentFilterItems.push(selectFilterItem);
                this.setState({
                    filterValues: currentFilterItems,
                    openFilterItems: currentFilterItems,
                    hideFilterItems: []
                });
            } else if (subgroupItems.length === 1) {

                const selectFilterItem = {
                    key: subgroupItems[0].group + "_" + subgroupItems[0].key,
                    group: subgroupItems[0].group,
                    label: subgroupItems[0].label,
                    groupLabel: this.props.getFilterData().find(x => x.subgroup === subgroupItems[0].group).label,
                    inSubgroup: true
                };
                if (indexFilterItem != -1)
                    currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
                else
                    currentFilterItems.push(selectFilterItem);

                const clone = cloneObjectsArray(currentFilterItems.filter(item => item.key != '-1'));
                clone.map(function (item) {
                    item.key = item.key.replace(item.group + "_", '');
                    return item;
                })
                this.onFilter(clone.filter(item => item.key != '-1'), this.state.sortId, this.state.sortDirection ? "desc" : "asc");
                this.setState({
                    filterValues: currentFilterItems,
                    openFilterItems: currentFilterItems,
                    hideFilterItems: []
                });
            }
        } else {
            const filterItems = this.getFilterData();

            const indexFilterItem = currentFilterItems.findIndex(x => x.group === filterItem.group);
            if (indexFilterItem != -1) {
                currentFilterItems.splice(indexFilterItem, 1);
            }

            const selectFilterItem = {
                key: filterItem.key,
                group: filterItem.group,
                label: filterItem.label,
                groupLabel: filterItem.inSubgroup ? filterItems.find(x => x.subgroup === filterItem.group).label : filterItems.find(x => x.key === filterItem.group).label
            };
            if (indexFilterItem != -1)
                currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
            else
                currentFilterItems.push(selectFilterItem);
            this.setState({
                filterValues: currentFilterItems,
                openFilterItems: currentFilterItems,
                hideFilterItems: []
            });

            const clone = cloneObjectsArray(currentFilterItems.filter(item => item.key != '-1'));
            clone.map(function (item) {
                item.key = item.key.replace(item.group + "_", '');
                return item;
            })
            this.onFilter(clone.filter(item => item.key != '-1'), this.state.sortId, this.state.sortDirection ? "desc" : "asc");
        }

    }

    render() {
        /* eslint-disable react/prop-types */
        const { className, id, style, size,
            isDisabled, scale, getFilterData, placeholder,
            getSortData, directionAscLabel, directionDescLabel,
            filterColumnCount, viewAs } = this.props;
        /* eslint-enable react/prop-types */

        const { searchText, filterValues, openFilterItems,
            hideFilterItems, sortId, sortDirection } = this.state;

        // console.log("filter input render, openFilterItems", openFilterItems, 'hideFilterItems', hideFilterItems);
        let iconSize = 30;
        switch (size) {
            case 'base':
                iconSize = 30;
                break;
            case 'middle':
            case 'big':
            case 'huge':
                iconSize = 41;
                break;
            default:
                break;
        }
        return (
            <StyledFilterInput viewAs={viewAs} className={className} id={id} style={style}>
                <div className='styled-search-input' ref={this.searchWrapper}>
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
                        <div className='styled-filter-block' ref={this.filterWrapper}>
                            <FilterBlock
                                openFilterItems={openFilterItems}
                                hideFilterItems={hideFilterItems}
                                iconSize={iconSize}
                                getFilterData={getFilterData}
                                onClickFilterItem={this.onClickFilterItem}
                                onDeleteFilterItem={this.onDeleteFilterItem}
                                isResizeUpdate={this.isResizeUpdate}
                                onRender={this.onFilterRender}
                                isDisabled={isDisabled}
                                columnCount={filterColumnCount}
                            />
                        </div>

                    </SearchInput>
                </div>

                <SortComboBox
                    options={getSortData()}
                    isDisabled={isDisabled}
                    onChangeSortId={this.onClickSortItem}
                    onChangeView={this.onClickViewSelector}
                    onChangeSortDirection={this.onChangeSortDirection}
                    selectedOption={getSortData().length > 0 ? getSortData().find(x => x.key === sortId) : {}}
                    onButtonClick={this.onSortDirectionClick}
                    viewAs={viewAs}
                    sortDirection={+sortDirection}
                    directionAscLabel={directionAscLabel}
                    directionDescLabel={directionDescLabel}
                />
                {viewAs &&
                    <ViewSelector
                        isDisabled={isDisabled}
                        onClickViewSelector={this.onClickViewSelector}
                        viewAs={viewAs}
                    />
                }
            </StyledFilterInput>

        );
    }
}

FilterInput.protoTypes = {
    size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
    autoRefresh: PropTypes.bool,
    selectedFilterData: PropTypes.object,
    directionAscLabel: PropTypes.string,
    directionDescLabel: PropTypes.string,
    viewAs: PropTypes.bool,  // TODO: include viewSelector after adding method getThumbnail - PropTypes.string 
    className: PropTypes.string,
    id: PropTypes.string,
    style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
    needForUpdate: PropTypes.bool,
    filterColumnCount: PropTypes.number,
    onChangeViewAs: PropTypes.func
};

FilterInput.defaultProps = {
    autoRefresh: true,
    selectedFilterData: {
        sortDirection: false,
        sortId: '',
        filterValues: [],
        searchText: ''
    },
    size: 'base',
    needForUpdate: false,
    directionAscLabel: 'A-Z',
    directionDescLabel: 'Z-A'
};

export default FilterInput;