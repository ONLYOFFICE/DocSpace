import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';
import IconButton from '../icon-button';
import { Icons } from '../icons';
import ContextMenuButton from '../context-menu-button';
import {UncontrolledPopover, PopoverBody } from 'reactstrap';

const StyledSearchInput = styled.div`
  min-width: 200px;
`;
const Caret = styled.div`
  width: 7px;
  position: absolute;
  right: 6px;
  transform: ${props => props.isOpen ? 'rotate(180deg)' : 'rotate(0)'};
  top: ${props => props.isOpen ? '2px' : '0'};
`;
const StyledFilterItem = styled.div`
  display:  ${props => props.block ? 'block' : 'inline-block'};
  margin-bottom: ${props => props.block ? '3px' : '0'};
  position relative;
  align-items: center;
  height: 100%;
  padding: 3px;
  padding-right: 18px;
  margin-right: 2px;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;
  &:last-child{
    margin-bottom: 0;
  }
`;
const StyledHideFilterButton = styled.div`
  display: flex;
  position: relative;
  align-items: center;
  font-weight: 600;
  font-size: 16px;
  height: 100%;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;
  padding: 0 20px 0 9px;
  margin-right: 2px;
  cursor: pointer;
  font-family: Open Sans;
  font-style: normal;

  :hover{
    border-color: #A3A9AE;
  }
  :active{
    background-color: #ECEEF1;
  }
`;
const StyledIconButtonBlock = styled.div`
  display: inline-block;
  margin-left: 5px;
  position absolute;
  right: 0;
  right: 3px;
  top: calc(50% - 5px);
`;
const StyledHideFilter = styled.div`
  display: inline-block;
  font-size: 1rem;
  height: 100%;
`;
const StyledPopoverBody = styled(PopoverBody)`
  font-size: 1rem;
  border-radius: 6px;
  box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.13);
`;

const FilterItem = props => {
  const { groupLabel, id, label, block } = props;
  return (
    <StyledFilterItem key={id} id={id} block={block} >
        {groupLabel}: {label}
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
class HideFilter extends React.Component {
  constructor(props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.state = {
      popoverOpen: false
    };
  }

  toggle() {
    this.setState({
      popoverOpen: !this.state.popoverOpen
    });
  }

  render() {
    return (
      <StyledHideFilter>
      <StyledHideFilterButton id="PopoverLegacy" >{this.props.count} <Caret isOpen={this.state.popoverOpen}><Icons.ExpanderDownIcon size='scale' isfill={true} color="#A3A9AE"/></Caret></StyledHideFilterButton>
      
      <UncontrolledPopover isOpen={this.state.popoverOpen} trigger="legacy" placement="bottom-start" target="PopoverLegacy" hideArrow={true} toggle={this.toggle}>
        <StyledPopoverBody>{this.props.children}</StyledPopoverBody>
      </UncontrolledPopover>
    </StyledHideFilter>
    );
  }
}


class SearchInput extends React.Component  {
  constructor(props) {
    super(props);

    this.minWidth = 190;
    this.isUpdateFilter = true;
    this.state = {
      filterItems: [],
      openFilterItems: [],
      hideFilterItems: []
    };

    this.searchWrapper = React.createRef();
    this.filterWrapper = React.createRef();

    this.onClickDropDownItem = this.onClickDropDownItem.bind(this);
    this.getData = this.getData.bind(this);
    this.onSearchClick = this.onSearchClick.bind(this);
    this.onDeleteFilterItem = this.onDeleteFilterItem.bind(this);
    this.getFilterItems = this.getFilterItems.bind(this);
    this.updateFilter = this.updateFilter.bind(this);
    this.resize = this.resize.bind(this);

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
    this.isUpdateFilter = false;
  }

  getData(){
    let _this = this;
    let d = this.props.getFilterData();
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
    this.isUpdateFilter = false;
  }

  getFilterItems(){
    let _this = this;
    let result = [];
    let openItems = [];
    let hideItems = [];
    if(this.state.filterItems.length > 0 && !this.isUpdateFilter){
      openItems = this.state.filterItems.map(function(item) {
            return <FilterItem 
              isDisabled={_this.props.isDisabled} 
              key={item.key}
              id={item.value} 
              groupLabel={item.groupLabel} 
              label={item.label} 
              onClose={_this.onDeleteFilterItem}>
            </FilterItem>
          });
    }
    if(this.state.hideFilterItems.length > 0 && this.isUpdateFilter){
      hideItems.push(
        <HideFilter key="hide-filter" count={this.state.hideFilterItems.length}>
          {
            this.state.hideFilterItems.map(function(item) {
              return <FilterItem 
                block={true}
                isDisabled={_this.props.isDisabled} 
                key={item.key}
                id={item.key} 
                groupLabel={item.groupLabel} 
                label={item.label} 
                onClose={_this.onDeleteFilterItem}>
              </FilterItem>
            })
          }
        </HideFilter>
      ); 
    }
    if(this.state.openFilterItems.length > 0 && this.isUpdateFilter){
      openItems = this.state.openFilterItems.map(function(item) {
            return <FilterItem 
              isDisabled={_this.props.isDisabled} 
              key={item.key}
              id={item.key} 
              groupLabel={item.groupLabel} 
              label={item.label} 
              onClose={_this.onDeleteFilterItem}>
            </FilterItem>
          });
    }
    
    result = hideItems.concat(openItems);
    return result;
  }

  updateFilter(){
    let fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
    let filterWidth = this.filterWrapper.current.getBoundingClientRect().width;

    if(fullWidth <= this.minWidth){
      this.setState({ openFilterItems: []});
      this.setState({ hideFilterItems: this.state.filterItems.slice()});
    }else if(filterWidth > fullWidth/2){
      let newOpenFilterItems = [];
      let newHideFilterItems = [];
      let openFilterWidth = 0;

      let sortArr = Array.from(this.filterWrapper.current.children).sort(function(a,b) {
        return a.getBoundingClientRect().width - b.getBoundingClientRect().width;
      });
      
      sortArr.forEach(element => {
        openFilterWidth = openFilterWidth + element.getBoundingClientRect().width;
        if(openFilterWidth < fullWidth/3){
          newOpenFilterItems.push(this.state.filterItems.find(x => x.value === element.getAttribute('id')));
        }else{
          newHideFilterItems.push(this.state.filterItems.find(x => x.value === element.getAttribute('id')));
        }
      });
      this.setState({ openFilterItems: newOpenFilterItems});
      this.setState({ hideFilterItems: newHideFilterItems});

    }else{
      this.setState({ openFilterItems: this.state.filterItems.slice()});
      this.setState({ hideFilterItems: []});
    }
    this.isUpdateFilter = true;
  }
  resize(){
    this.isUpdateFilter = false;
    this.forceUpdate();
  }
  componentDidUpdate(){
    if(!this.isUpdateFilter) this.updateFilter();
  }
  
  componentDidMount() {
    window.addEventListener('resize', _.throttle(this.resize), 100);
  }
  componentWillUnmount() {
    window.removeEventListener('resize', this.resize());
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
      <StyledSearchInput ref={this.searchWrapper}> 
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
            { this.props.isNeedFilter && 
              <div ref={this.filterWrapper}>
                {this.getFilterItems()}
              </div>
            }
          
          { this.props.isNeedFilter && 
            <ContextMenuButton
              title={'Actions'}
              iconName={'RectangleFilterIcon'}
              color='#A3A9AE'
              size={iconSize}
              getData={_this.getData}
            />
          }
        </InputBlock>
      </StyledSearchInput>
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