import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import SearchInput from '../search-input';
import isEqual from 'lodash/isEqual';
import throttle from 'lodash/throttle';
import FilterBlock from './filter-block';
import SortComboBox from './sort-combobox';
import { mobile } from '../../utils/device';
import map from 'lodash/map';
import clone from 'lodash/clone';

const StyledFilterInput = styled.div`
    width: 100%;

    &:after {
        content: " ";
        display: block;
        height: 0;
        clear: both;
        visibility: hidden;
    }
`;
const StyledSearchInput = styled.div`
    display: block;
    float: left;
    width: calc(100% - 140px);
    @media ${mobile} {
        width: calc(100% - 58px);
    }
`;
const StyledFilterBlock = styled.div`
    display: flex;

    .filter-button {
        div:active {
            svg path:first-child { 
                fill: #ECEEF1; 
            }
        }
        div:first-child:hover {
            svg path:first-child { 
                stroke: #adb3b8; 
            }
            svg path:not(:first-child) { 
                stroke: #555F65; 
            }
        }
    }

`;
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

        this.throttledResize = throttle(this.resize, 300);

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

            if ( filterButton !== null && (elementsWidth >= (fullWidth / 3) - filterButton.getBoundingClientRect().width)) {
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

    componentDidMount() {
        window.addEventListener('resize', this.throttledResize);
        if (this.state.filterValues.length > 0) this.updateFilter();
    }
    componentWillUnmount() {
        window.removeEventListener('resize', this.throttledResize);
    }
    shouldComponentUpdate(nextProps, nextState) {
        if (!isEqual(this.props.selectedFilterData, nextProps.selectedFilterData)) {
            let internalFilterData = cloneObjectsArray(this.state.filterValues);
            if (nextProps.selectedFilterData.filterValues) {
                internalFilterData = convertToInternalData(this.props.getFilterData(), cloneObjectsArray(nextProps.selectedFilterData.filterValues));
                this.updateFilter(internalFilterData);
            }
            this.setState(
                {
                    sortDirection: nextProps.selectedFilterData.sortDirection === "desc" ? true : false,
                    sortId: this.props.getSortData().findIndex(x => x.key === nextProps.selectedFilterData.sortId) != -1 ? nextProps.selectedFilterData.sortId : "",
                    filterValues: internalFilterData,
                    searchText: nextProps.selectedFilterData.inputValue || this.props.value
                }
            );
            return true;
        }
        if (this.props.id != nextProps.id ||
            this.props.isDisabled != nextProps.isDisabled ||
            this.props.size != nextProps.size ||
            this.props.placeholder != nextProps.placeholder ||
            this.props.value != nextProps.value)

            return true;
        if (this.isResizeUpdate) {
            return true;
        }
        return !isEqual(this.state, nextState);
    }
    render() {
        //console.log("FilterInput render");
        let iconSize = 33;
        switch (this.props.size) {
            case 'base':
                iconSize = 33;
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
            <StyledFilterInput className={this.props.className} id={this.props.id} style={this.props.style}>
                <StyledSearchInput ref={this.searchWrapper}>
                    <SearchInput
                        id={this.props.id}
                        isDisabled={this.props.isDisabled}
                        size={this.props.size}
                        scale={this.props.scale}
                        isNeedFilter={true}
                        getFilterData={this.props.getFilterData}
                        placeholder={this.props.placeholder}
                        onSearchClick={this.onSearch}
                        onChangeFilter={this.onChangeFilter}
                        value={this.state.searchText}
                        selectedFilterData={this.state.filterValues}
                        showClearButton={this.state.filterValues.length > 0}
                        onClearSearch={this.clearFilter}
                        onChange={this.onSearchChanged}
                    >
                        <StyledFilterBlock ref={this.filterWrapper}>
                            <FilterBlock
                                openFilterItems={this.state.openFilterItems}
                                hideFilterItems={this.state.hideFilterItems}
                                iconSize={iconSize}
                                getFilterData={this.props.getFilterData}
                                onClickFilterItem={this.onClickFilterItem}
                                onDeleteFilterItem={this.onDeleteFilterItem}
                                isResizeUpdate={this.isResizeUpdate}
                                onRender={this.onFilterRender}
                                isDisabled={this.props.isDisabled}
                            />
                        </StyledFilterBlock>

                    </SearchInput>
                </StyledSearchInput>

                <SortComboBox
                    options={this.props.getSortData()}
                    isDisabled={this.props.isDisabled}
                    onChangeSortId={this.onClickSortItem}
                    onChangeSortDirection={this.onChangeSortDirection}
                    selectedOption={this.props.getSortData().length > 0 ? this.props.getSortData().find(x => x.key === this.state.sortId) : {}}
                    onButtonClick={this.onSortDirectionClick}
                    sortDirection={+this.state.sortDirection}
                    directionAscLabel={this.props.directionAscLabel}
                    directionDescLabel={this.props.directionDescLabel}
                />
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
    className: PropTypes.string,
    id: PropTypes.string,
    style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
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
    directionAscLabel: 'A-Z',
    directionDescLabel: 'Z-A'
};

export default FilterInput;