import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';
import IconButton from '../icon-button';
import ContextMenuButton from '../context-menu-button';

const StyledFilterItem = styled.div`
  display: flex;
  align-items: center;
  height: 100%;
  padding: 3px;
  margin-right: 2px;
  border: 1px solid #d4e4ec;
  border-radius: 3px;
  background-color: #edf6fd;
`;
const StyledIconButtonBlock = styled.div`
  display: inline-block;
  margin-left: 5px;
`;
const FilterItem = props => {
  const { groupLabel, id, label } = props;
  return (
    <StyledFilterItem key={id}>
        {groupLabel} {label}
        <StyledIconButtonBlock>
          <IconButton
            color={props.color}
            size={10}
            iconName={"CrossIcon"}
            isFill={true}
            isDisabled={props.isDisabled}
            onClick={!props.isDisabled ? ((e) => props.onClose(e, id)) : undefined}
          />
        </StyledIconButtonBlock>
    </StyledFilterItem>
  );
};

class SearchInput extends React.Component  {
  constructor(props) {
    super(props);

    this.state = {
      filterItems: []
    };

    this.onClickDropDownItem = this.onClickDropDownItem.bind(this);
    this.getData = this.getData.bind(this);
    this.onSearchClick = this.onSearchClick.bind(this);
    this.onDeleteFilterItem = this.onDeleteFilterItem.bind(this);
    this.getFilterItems = this.getFilterItems.bind(this);
    
  }

  onClickDropDownItem(event, filterItem){
    
    let curentFilterItems =  this.state.filterItems.slice();
    let filterItems = this.getData()
    let indexFilterItem = curentFilterItems.findIndex(x => x.key === filterItem.group);
    if(indexFilterItem != -1){
      curentFilterItems.splice(indexFilterItem, 1);
    }
   
    let selectFilterItem = {
      key: filterItem.group,
      value:  filterItem.key,
      label:  filterItem.label,
      groupLabel: filterItems.find(x => x.key === filterItem.group).label
    };
    curentFilterItems.push(selectFilterItem);

    this.setState({ filterItems: curentFilterItems});   
    if(typeof this.props.onChangeFilter === "function")
      this.props.onChangeFilter({
        inputValue: this.props.value,
        filterValue: this.props.isNeedFilter ? curentFilterItems : null
      });
  }

  getData(){
    let _this = this;
    let d= this.props.getFilterData();
    d.map(function(item){
      item.onClick = !item.isSeparator && !item.isHeader && !item.disabled ? ((e) => _this.onClickDropDownItem(e, item)) : undefined;
      return item;
    });
    return d;
  }
  
  onSearchClick(e, value){
    let searchResult = {
      inputValue: value,
      filterValue: this.props.isNeedFilter ? this.state.filterItems : null
    };
    if(typeof this.props.onSearchClick === "function")
      this.props.onSearchClick(searchResult);
  }

  onDeleteFilterItem(e , key){

    let curentFilterItems =  this.state.filterItems.slice();
    let indexFilterItem = curentFilterItems.findIndex(x => x.key === key);
    if(indexFilterItem != -1){
      curentFilterItems.splice(indexFilterItem, 1);
    }
    this.setState({ filterItems: curentFilterItems});
    
    if(typeof this.props.onChangeFilter === "function")
      this.props.onChangeFilter({
        inputValue: this.props.value,
        filterValue: this.props.isNeedFilter ? curentFilterItems : null
      });
  }

  getFilterItems(){
    let _this = this;

    const result = this.state.filterItems.map(function(item) {
      
      return <FilterItem 
        isDisabled={_this.props.isDisabled} 
        key={item.key}
        id={item.key} 
        groupLabel={item.groupLabel} 
        label={item.label} 
        onClose={_this.onDeleteFilterItem}>

      </FilterItem>
    })
    return result;
  }

  render() {
    let _this = this;
    let iconSize = 32;
    switch (this.props.size) {
      case 'base':
        iconSize = 32
        break;
      case 'middle':
      case 'big':
      case 'huge':
        iconSize = 41
        break;
      default:
        break;
    }

    return (
      <InputBlock 
          id={this.props.id}
          isDisabled={this.props.isDisabled}
          iconName={"SearchIcon"}
          isIconFill={true}
          iconColor={"#A3A9AE"}
          onIconClick={this.onSearchClick}
          size={this.props.size}
          scale={true}
          value={this.props.value}
          placeholder={this.props.placeholder}
          onChange={this.props.onChange}
      >
        { this.props.isNeedFilter && this.getFilterItems()}
        {
          this.props.isNeedFilter && 
          <ContextMenuButton
            title={'Actions'}
            iconName={'RectangleFilterIcon'}
            color='#A3A9AE'
            size={iconSize}
            getData={_this.getData}
          />
        }
      </InputBlock>
    );
  }
};

SearchInput.propTypes = {
    id: PropTypes.string,
    size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
    value:PropTypes.string,
    scale: PropTypes.bool,
    placeholder: PropTypes.string,
    onChange: PropTypes.func,
    getFilterData:PropTypes.func,
    isNeedFilter: PropTypes.bool,
    isDisabled: PropTypes.bool
};

SearchInput.defaultProps = {
    size: 'base',
    value: '',
    scale: false,
    isNeedFilter: false,
    isDisabled: false
};

export default SearchInput;