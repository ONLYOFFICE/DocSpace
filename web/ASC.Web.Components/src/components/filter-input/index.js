import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import SearchInput from '../search-input';
import ComboBox from '../combobox'
import IconButton from '../icon-button';
import isEqual from 'lodash/isEqual';

const StyledFilterInput = styled.div`
    min-width: 380px;
`;
const StyledIconButton = styled.div`
    transform: ${state => state.sortDirection ? 'scale(1, -1)' : 'scale(1)'};
`;
const StyledSearchInput = styled.div`
  display: block;
  float: left;
  width: calc(80% - 8px);
`;

const StyledComboBox = styled(ComboBox)`
  display: block;
  float: left;
  width: 20%;
  margin-left: 8px;
`;

class SortComboBox extends React.Component {
    constructor(props) {
        super(props);
        this.onSelect = this.onSelect.bind(this);
    }
    onSelect(item) {
        this.props.onSelect(item);
    }
    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.props, nextProps);
    }
    render() {
        return (
            <StyledComboBox
                options={this.props.options}
                isDisabled={this.props.isDisabled}
                onSelect={this.onSelect}
                selectedOption={this.props.selectedOption}
            >
                <StyledIconButton sortDirection={this.props.sortDirection}>
                    <IconButton
                        color={"#D8D8D8"}
                        hoverColor={"#333"}
                        clickColor={"#333"}
                        size={10}
                        iconName={'ZASortingIcon'}
                        isFill={true}
                        isDisabled={this.props.isDisabled}
                        onClick={this.props.onButtonClick}
                    />
                </StyledIconButton>
            </StyledComboBox>
        );
    }
}

class FilterInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            sortDirection: props.selectedFilterData.sortDirection == "asc" ? true : false,
            sortId: props.getSortData().findIndex(x => x.key === props.selectedFilterData.sortId) != -1 ? props.selectedFilterData.sortId : props.getSortData().length > 0 ? props.getSortData()[0].key : "",
            filterValues: props.selectedFilterData.filterValues,
            searchText: props.selectedFilterData.inputValue || props.value
        };

        this.timerId = null;

        this.onClickSortItem = this.onClickSortItem.bind(this);
        this.onSortDirectionClick = this.onSortDirectionClick.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.onChangeFilter = this.onChangeFilter.bind(this);
        this.setFilterTimer = this.setFilterTimer.bind(this);
        this.onSearchChanged = this.onSearchChanged.bind(this);

        this.getDefaultSelectedIndex = this.getDefaultSelectedIndex.bind(this);

    }
    getDefaultSelectedIndex() {
        const sortData = this.props.getSortData();
        if (sortData.length > 0) {
            let defaultIndex = sortData.findIndex(x => x.key === this.state.sortId);
            return defaultIndex != -1 ? defaultIndex : 0;
        }
        return 0;
    }
    onClickSortItem(item) {
        this.setState({ sortId: item.key });
        this.onFilter(this.state.filterValues, item.key, this.state.sortDirection ? "asc" : "desc");
    }
    onSortDirectionClick(e) {
        this.onFilter(this.state.filterValues, this.state.sortId, !this.state.sortDirection ? "asc" : "desc");
        this.setState({ sortDirection: !this.state.sortDirection });
    }
    onChangeFilter(result) {
        this.setState({
            searchText: result.inputValue,
            filterValues: result.filterValues,
        });
        this.onFilter(result.filterValues, this.state.sortId, this.state.sortDirection ? "asc" : "desc", result.inputValue);
    }
    onSearch(result) {
        this.onFilter(result.filterValues, this.state.sortId, this.state.sortDirection ? "asc" : "desc");
    }

    onFilter(filterValues, sortId, sortDirection, searchText) {
        let result = {
            inputValue: searchText != undefined ? searchText : this.state.searchText,
            filterValues: filterValues,
            sortId: sortId,
            sortDirection: sortDirection
        };
        this.props.onFilter(result);
    }

    setFilterTimer() {
        this.timerId && clearTimeout(this.timerId);
        this.timerId = null;
        this.timerId = setTimeout(() => {
            this.onSearch({ filterValues: this.state.filterValues });
            clearTimeout(this.timerId);
            this.timerId = null;
        }, this.props.refreshTimeout);
    }

    onSearchChanged(e) {
        this.setState({ searchText: e.target.value });

        if (this.props.autoRefresh)
            this.setFilterTimer();
    }
    shouldComponentUpdate(nextProps, nextState) {
        if (!isEqual(this.props.selectedFilterData, nextProps.selectedFilterData)) {
            this.setState(
                {
                    sortDirection: nextProps.selectedFilterData.sortDirection === "asc" ? true : false,
                    sortId: this.props.getSortData().findIndex(x => x.key === nextProps.selectedFilterData.sortId) != -1 ? nextProps.selectedFilterData.sortId : "",
                    filterValues: nextProps.selectedFilterData.filterValues || this.state.filterValues,
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

        return !isEqual(this.state, nextState);
    }

    render() {
        //console.log("FilterInput render");
        return (
            <StyledFilterInput>
                <StyledSearchInput>
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
                        onChange={this.onSearchChanged}
                    />
                </StyledSearchInput>

                <SortComboBox
                    options={this.props.getSortData()}
                    isDisabled={this.props.isDisabled}
                    onSelect={this.onClickSortItem}
                    selectedOption={this.props.getSortData().length > 0 ? this.props.getSortData().find(x => x.key === this.state.sortId) : {}}
                    onButtonClick={this.onSortDirectionClick}
                    sortDirection={this.state.sortDirection}
                />
            </StyledFilterInput>

        );
    }
};

FilterInput.protoTypes = {
    autoRefresh: PropTypes.bool,
    refreshTimeout: PropTypes.number,
    selectedFilterData: PropTypes.object,
};

FilterInput.defaultProps = {
    autoRefresh: true,
    refreshTimeout: 1000,
    selectedFilterData: {
        sortDirection: false,
        sortId: '',
        filterValues: [],
        searchText: ''
    }
};

export default FilterInput;