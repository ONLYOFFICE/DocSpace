import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import SearchInput from '../search-input';
import ComboBox from '../combobox'
import IconButton from '../icon-button';

const StyledFilterInput = styled.div`
    min-width: 380px;
`;
const StyledIconButton = styled.div`
    transform: ${state => state.sortDirection ? 'scale(1, -1)' : 'scale(1)'};
`;
const StyledSearchInput = styled.div`
  display: inline-block;
  float: left;
  width: calc(80% - 8px);
`;
const StyledComboBox = styled(ComboBox)`
  display: inline-block;
  float left;
  width: 20%;
  margin-left: 8px;
`;

class FilterInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            sortDirection: props.selectedFilterData ? props.selectedFilterData.sortDirection == "asc" ? true : false : false,
            sortId: props.selectedFilterData ? 
                        this.props.getSortData().findIndex(x => x.key === props.selectedFilterData.sortId) != -1 ? props.selectedFilterData.sortId : "" : 
                        this.props.getSortData().length > 0 ? this.props.getSortData()[0].key : "",

            filterValue: props.selectedFilterData ? props.selectedFilterData.filterValue :  [],
            searchText: props.selectedFilterData ? props.selectedFilterData.inputValue : this.props.value
        };

        this.timerId = null;
        this.isNew = true;
        this.isNeedUpdate = false;

        this.updatedProps = {
            sortDirection: false,
            sortId:  false,
            filterValue: false,
            searchText: false
        };

        this.onClickSortItem = this.onClickSortItem.bind(this);
        this.onSortDirectionClick = this.onSortDirectionClick.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.setFilterTimer = this.setFilterTimer.bind(this);
        this.onSearchChanged = this.onSearchChanged.bind(this);
        
        this.getDefaultSelectedIndex = this.getDefaultSelectedIndex.bind(this);

    }
    getDefaultSelectedIndex(){
        const sortData = this.props.getSortData();
        if(sortData.length > 0){
            let defaultIndex = sortData.findIndex(x => x.key === this.state.sortId);
            return defaultIndex != -1 ? defaultIndex : 0;
        }
        return 0;
    }
    onClickSortItem(item) {
        this.setState({ sortId: item.key });
        this.onFilter(this.state.filterValue, item.key, this.state.sortDirection ? "asc" : "desc");
    }
    onSortDirectionClick(e) {
        this.isNeedUpdate = true;
        this.onFilter(this.state.filterValue, this.state.sortId, !this.state.sortDirection ? "asc" : "desc");
        this.setState({ sortDirection: !this.state.sortDirection });
    }
    onSearch(result) {
        this.setState({ filterValue: result.filterValue });
        this.onFilter(result.filterValue, this.state.sortId, this.state.sortDirection ? "asc" : "desc");
    }

    onFilter(filterValue, sortId, sortDirection) {
        let result = {
            inputValue: this.state.searchText,
            filterValue: filterValue,
            sortId: sortId,
            sortDirection: sortDirection
        };
        this.props.onFilter(result);
    }

    setFilterTimer() {
        this.timerId && clearTimeout(this.timerId);
        this.timerId = null;
        this.timerId = setTimeout(() => {
            this.onSearch({ filterValue: this.state.filterValue });
            clearTimeout(this.timerId);
            this.timerId = null;
        }, this.props.refreshTimeout);
    }

    onSearchChanged(e) {
        this.isNeedUpdate = true;
        this.setState({ searchText: e.target.value });

        if (this.props.autoRefresh)
            this.setFilterTimer();
    }
    componentDidUpdate(){
        if(this.isNeedUpdate){
            this.setState(
                {
                    sortDirection: this.updatedProps.sortDirection ? this.props.selectedFilterData.sortDirection == "asc" ? true : false : this.state.sortDirection,
                    sortId: this.updatedProps.sortId ? this.props.getSortData().findIndex(x => x.key === this.props.selectedFilterData.sortId) != -1 ? this.props.selectedFilterData.sortId : "" : this.state.sortId,
                    filterValue: this.updatedProps.filterValue ? this.props.selectedFilterData.filterValue ? this.props.selectedFilterData.filterValue : [] : this.state.filterValue,
                    searchText: this.updatedProps.searchText ? this.props.selectedFilterData.inputValue ? this.props.selectedFilterData.inputValue : this.props.value : this.state.searchText
                }
            );
            this.updatedProps={
                sortDirection: false,
                sortId:  false,
                filterValue: false,
                searchText: false
            }
        }
    }
    shouldComponentUpdate(nextProps, nextState){
        if(!this.isNeedUpdate){
            for (let propsKey in this.props) {
                if(typeof this.props[propsKey] != "function" && typeof this.props[propsKey] != "object" && this.props[propsKey] != nextProps[propsKey]){
                  this.isNeedUpdate = true;
                  break;
                }
            }
            if(nextProps.selectedFilterData && this.props.selectedFilterData){
                if(!this.props.selectedFilterData.filterValue || !nextProps.selectedFilterData.filterValue){
                    this.updatedProps.filterValue = true;
                    this.isNeedUpdate = true;
                }else{
                    if(this.props.selectedFilterData.filterValue.length != nextProps.selectedFilterData.filterValue.length){
                        this.updatedProps.filterValue = true;
                        this.isNeedUpdate = true;
                    }else{
                      let newFilterItems = nextProps.selectedFilterData.filterValue;
                      let oldFilterItems = this.props.selectedFilterData.filterValue;
              
                      for(let i = 0; i < newFilterItems.length; i++){
                        if(oldFilterItems.find(x => (x.key === newFilterItems[i].key && x.group === newFilterItems[i].group)) == undefined){
                            this.updatedProps.filterValue = true;
                            this.isNeedUpdate = true;
                            break;
                        }
                      }
                    }
                }
              }
            if(nextProps.selectedFilterData.inputValue != this.props.selectedFilterData.inputValue){
                this.updatedProps.searchText = true;
                this.isNeedUpdate = true;
            }
            if(nextProps.selectedFilterData.sortDirection != this.props.selectedFilterData.sortDirection){
                this.updatedProps.sortDirection = true;
                this.isNeedUpdate = true;
            }
            if(nextProps.selectedFilterData.sortId != this.props.selectedFilterData.sortId){
                this.updatedProps.sortId = true;
                this.isNeedUpdate = true;
            }
            if(!this.isNeedUpdate) return false;
            else return true;
        }
        this.isNeedUpdate = false;
        return true;
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
                        onChangeFilter={this.onSearch}
                        value={this.state.searchText}
                        selectedFilterData={this.state.filterValue}
                        onChange={this.onSearchChanged}
                    />
                </StyledSearchInput>

                <StyledComboBox
                    options={this.props.getSortData()}
                    isDisabled={this.props.isDisabled}
                    onSelect={this.onClickSortItem}
                    selectedOption={this.state.sortId}
                >
                    <StyledIconButton {...this.state}>
                        <IconButton
                            color={"#D8D8D8"}
                            hoverColor={"#333"}
                            clickColor={"#333"}
                            size={10}
                            iconName={'ZASortingIcon'}
                            isFill={true}
                            isDisabled={this.props.isDisabled}
                            onClick={this.onSortDirectionClick}
                        />
                    </StyledIconButton>

                </StyledComboBox>
            </StyledFilterInput>

        );
    }
};

FilterInput.protoTypes = {
    autoRefresh: PropTypes.bool,
    refreshTimeout: PropTypes.number,
    selectedFilterData:  PropTypes.object,
};

FilterInput.defaultProps = {
    autoRefresh: true,
    refreshTimeout: 1000,
    selectedFilterData: {}
};

export default FilterInput;