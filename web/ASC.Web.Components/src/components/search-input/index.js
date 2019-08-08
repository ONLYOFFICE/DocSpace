import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';

import FilterButton from './filter-button';
import HideFilter from './hide-filter';
import CloseButton from './close-button';

const StyledSearchInput = styled.div`
  min-width: 200px;
  font-family: Open Sans;
  font-style: normal;
`;
const StyledFilterBlock = styled.div`
  display: flex;
  align-items: center;
`;
const StyledFilterItem = styled.div`
  display:  ${props => props.block ? 'block' : 'inline-block'};
  margin-bottom: ${props => props.block ? '3px' : '0'};
  position relative;
  
  height: 100%;
  padding: 3px 44px 3px 7px;
  margin-right: 2px;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;

  font-weight: 600;
  font-size: 13px;
  line-height: 18px;

  &:last-child{
    margin-bottom: 0;
  }
`;
const StyledCloseButtonBlock = styled.div`
  display: flex;
  align-items: center;
  position: absolute;
  height: 100%;
  width: 25px;
  border-left: 1px solid #ECEEF1;
  right: 0;
  top: 0;
`;

const FilterItem = props => {
  //console.log("FilterItem render");
  const { groupLabel, id, label, block } = props;
  return (
    <StyledFilterItem key={id} id={id} block={block} >
        {groupLabel}: {label}
        <StyledCloseButtonBlock>
          <CloseButton
            isDisabled={props.isDisabled}
            onClick={!props.isDisabled ? ((e) => props.onClose(e, id)) : undefined}
          />
        </StyledCloseButtonBlock>
    </StyledFilterItem>
  );
};

const cloneProperty = function(props){
  var newProps = [];
  props.forEach(item=> {
    let cloneItem = {};
    for (var key in item) {
      cloneItem[key] = item[key];
    }
    newProps.push(cloneItem);
  });
  return newProps;
}

class SearchInput extends React.Component  {
  constructor(props) {
    super(props);

    function getDefaultFilterData(){
      let filterData = props.getFilterData();
      let filterItems = [];
      let selectedFilterData = cloneProperty(props.selectedFilterData);
      selectedFilterData.forEach(defaultfilterValue => {
          let filterValue = filterData.find(x => ((x.key === defaultfilterValue.key.replace(defaultfilterValue.group + "_",'')) && x.group === defaultfilterValue.group));
          if(filterValue != undefined){
              defaultfilterValue.key = defaultfilterValue.group + "_" + defaultfilterValue.key;
              defaultfilterValue.label = filterValue.label; 
              defaultfilterValue.groupLabel = filterData.find(x => (x.key === defaultfilterValue.group)).label;
              filterItems.push(defaultfilterValue);
          }
      });
      return filterItems;
    }

    this.minWidth = 190;
    this.isUpdateFilter = true;
    this.isResizeUpdate = false;
    this.isNewProps = false;
    this.isUpdatedHiddenItems = false;

    this.isNew = true;

    this.state = {
      filterItems: props.selectedFilterData ? getDefaultFilterData() : [],
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
    let curentFilterItems =  cloneProperty(this.state.filterItems);
    let filterItems = this.getData();

    let indexFilterItem = curentFilterItems.findIndex(x => x.group === filterItem.group);
    if(indexFilterItem != -1){
      curentFilterItems.splice(indexFilterItem, 1);
    }

    let selectFilterItem = {
      key:  filterItem.key,
      group: filterItem.group,
      label:  filterItem.label,
      groupLabel: filterItems.find(x => x.key === filterItem.group).label
    };
    curentFilterItems.push(selectFilterItem);
    this.setState({ filterItems: curentFilterItems}); 

    let clone = cloneProperty(curentFilterItems);
    clone.map(function(item){
      item.key = item.key.replace(item.group + "_" ,'');
      return item;
    })
    if(typeof this.props.onChangeFilter === "function")
      this.props.onChangeFilter({
        inputValue: this.props.value,
        filterValue: this.props.isNeedFilter ? 
                      clone : 
                      null
      });
    this.isUpdateFilter = false;
  }

  getData(){
    let _this = this;
    let d = this.props.getFilterData();
    d.map(function(item){
      item.onClick = !item.isSeparator && !item.isHeader && !item.disabled ? ((e) => _this.onClickDropDownItem(e, item)) : undefined;
      item.key = item.group != item.key ? item.group +"_"+ item.key : item.key;
      return item;
    });
    return d;
  }
  
  onSearchClick(e, value){
    let searchResult = {
      inputValue: value,
      filterValue: this.props.isNeedFilter ? this.state.filterItems.map(function(item){
                      item.key = item.key.replace(item.group + "_",'');
                      return item;
                    }) : null
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
        filterValue: this.props.isNeedFilter ? 
                    curentFilterItems.map(function(item){
                      item.key = item.key.replace(item.group + "_",'');
                      return item;
                    }) : null
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
              id={item.key} 
              groupLabel={item.groupLabel} 
              label={item.label} 
              onClose={_this.onDeleteFilterItem}>
            </FilterItem>
          });
    }
    if(this.state.hideFilterItems.length > 0 && this.isUpdateFilter){
      hideItems.push(
        <HideFilter key="hide-filter" count={this.state.hideFilterItems.length} isDisabled={this.props.isDisabled}>
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
          newOpenFilterItems.push(this.state.filterItems.find(x => x.key === element.getAttribute('id')));
        }else{
          newHideFilterItems.push(this.state.filterItems.find(x => x.key === element.getAttribute('id')));
        }
      });
      this.setState({ openFilterItems: newOpenFilterItems});
      this.setState({ hideFilterItems: newHideFilterItems});
    }else{
      this.setState({ openFilterItems: this.state.filterItems.slice()});
      this.setState({ hideFilterItems: []});

    }
    this.isUpdatedHiddenItems = true;
    this.isUpdateFilter = true;
  }

  resize(){
    this.isResizeUpdate = true;
    this.isUpdateFilter = false;
    this.forceUpdate();
  }
  componentDidUpdate(){
    if(!this.isUpdateFilter){
      this.updateFilter();
    }else if(this.isNewProps && this.props.isNeedFilter){
      let filterData = this.props.getFilterData();
      let filterItems = [];

      let curentFilterItems = cloneProperty(this.props.selectedFilterData);

      for(let i=0; i < curentFilterItems.length; i++){
        let filterValue = filterData.find(x => ((x.key === curentFilterItems[i].key.replace(curentFilterItems[i].group + "_",'')) && x.group === curentFilterItems[i].group));
          if(filterValue != undefined){
            curentFilterItems[i].key = curentFilterItems[i].group + "_" + curentFilterItems[i].key;
            curentFilterItems[i].label = filterValue.label; 
            curentFilterItems[i].groupLabel = filterData.find(x => (x.group === curentFilterItems[i].group)).label;
            filterItems.push(curentFilterItems[i]);
          }
      }
      this.isNewProps= false;
      this.isUpdateFilter = false;
      this.setState({ filterItems: filterItems});
    }
  }
  componentDidMount() {
    window.addEventListener('resize', _.throttle(this.resize), 100);
    this.isUpdateFilter = false; this.updateFilter();
  }
  componentWillUnmount() {
    window.removeEventListener('resize', this.resize());
  }
  shouldComponentUpdate(nextProps, nextState){
    if(!this.isNew){
      let isNeedUpdate = false;

      if(nextProps.selectedFilterData && this.props.selectedFilterData && this.props.isNeedFilter){
        if(this.props.selectedFilterData.length != nextProps.selectedFilterData.length){
          this.isNewProps = true;
          isNeedUpdate = true;
        }else{
          let newFilterItems = nextProps.selectedFilterData;
          let oldFilterItems = this.props.selectedFilterData;
  
          for(let i = 0; i < newFilterItems.length; i++){
            if(oldFilterItems.find(x => (x.key === newFilterItems[i].key && x.group === newFilterItems[i].group)) == undefined){
              this.isNewProps = true;
              isNeedUpdate = true;
              break;
            }
          }
        }
      }
      if(!isNeedUpdate){
        for (let propsKey in this.props) {
          if(typeof this.props[propsKey] != "function" && typeof this.props[propsKey] != "object" && this.props[propsKey] != nextProps[propsKey]){
            isNeedUpdate = true;
            break;
          }
        }
      }
      if(!isNeedUpdate){
        for (let key in this.state) {
          if(this.state[key].length != nextState[key].length){
            isNeedUpdate = true;
          }else{
            for(let i = 0; i < this.state[key].length; i++){
              if(nextState[key].find(x => (x.key === this.state[key][i].key && x.group === this.state[key][i].group)) == undefined){
                isNeedUpdate = true;
                break;
              }
            }
          }
        }
      }
      if(!isNeedUpdate && !this.isResizeUpdate){
        if(nextProps.value == this.props.value){
          if(!this.isUpdatedHiddenItems){
            this.isUpdatedHiddenItems = false;
            return false;
          } 
        }
      }else if(this.isResizeUpdate){
        this.isResizeUpdate = false
      }
    }
    this.isNew = false;
    return true;
  }

  render() {
    //console.log("Search input render");
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
              <StyledFilterBlock ref={this.filterWrapper}>
                {this.getFilterItems()}
              </StyledFilterBlock>
            }
          
          { this.props.isNeedFilter && 
            <FilterButton iconSize={iconSize} getData={_this.getData} isDisabled={this.props.isDisabled}/>
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
    isDisabled: PropTypes.bool,
    selectedFilterData: PropTypes.array
};

SearchInput.defaultProps = {
    size: 'base',
    value: '',
    scale: false,
    isNeedFilter: false,
    isDisabled: false,
    selectedFilterData: []
};

export default SearchInput;