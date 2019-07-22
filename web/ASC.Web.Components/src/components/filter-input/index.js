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
            sortDirection: false,
            sortId: this.props.getSortData().length > 0 ? this.props.getSortData()[0].id : "",
            filterValue: []
        };

        this.getSortData = this.getSortData.bind(this);
        this.onClickSortItem = this.onClickSortItem.bind(this);
        this.onSortDirectionClick = this.onSortDirectionClick.bind(this);
        this.onSearch = this.onSearch.bind(this);
        
    }
    getSortData(){
        let _this = this;
        let d = this.props.getSortData();
        d.map(function(item){
            item.key = item.id;
            item.onClick = (e) => _this.onClickSortItem(e, item);
            return item;
        });
        return d;
    }
    onClickSortItem(e, item){
        this.setState({ sortId: item.id});
        this.onFilter(this.state.filterValue, item.id, this.state.sortDirection ? "asc" : "desc");
    }
    onSortDirectionClick(e){
        this.onFilter(this.state.filterValue, this.state.sortId, !this.state.sortDirection ? "asc" : "desc")
        this.setState({ sortDirection: !this.state.sortDirection});
    }
    onSearch(result){
        
        this.setState({ filterValue: result.filterValue});
        this.onFilter(result.filterValue, this.state.sortId, this.state.sortDirection ? "asc" : "desc")
    }

    onFilter(filterValue, sortId, sortDirection){
        let result = {
            inputValue:  this.props.value,
            filterValue: filterValue,
            sortId: sortId,
            sortDirection: sortDirection
        }
        this.props.onFilter(result);
    }

    render(){
        return(
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
                        onSearchClick={(result) => {this.onSearch(result)}}
                        onChangeFilter={(result) => {this.onSearch(result)}}
                        value={this.props.value}
                        onChange={this.props.onChange}
                    />
                </StyledSearchInput>
                
                <StyledComboBox 
                    items={this.getSortData()}
                    isDisabled={this.props.isDisabled}
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
}

export default FilterInput;