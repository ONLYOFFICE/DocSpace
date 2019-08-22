import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';
import ComboBox from '../combobox';

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
  line-height: 15px;

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
const StyledComboBox = styled(ComboBox)`
  display: inline-block;
  background: transparent;
  cursor: pointer;
  vertical-align: middle;
  margin-left: -10px;
`;
const StyledFilterName = styled.span`
  line-height: 18px;
  margin-left: 5px;
`;

class FilterItem extends React.Component {
  constructor(props) {
      super(props);

      this.state={
        id: this.props.id
      };

      this.onSelect = this.onSelect.bind(this);
  }

  onSelect(option){
    this.props.onSelectFilterItem(null, {
      key: option.group + "_" + option.key,
      label: option.label,
      group: option.group,
      inSubgroup: !!option.inSubgroup
    });
  }

  render(){
    return(
      <StyledFilterItem key={this.state.id} id={this.state.id} block={this.props.block} >
        {this.props.groupLabel}: 
          {this.props.groupItems.length > 1 ? 
            <StyledComboBox
              options={this.props.groupItems}
              isDisabled={this.props.isDisabled}
              onSelect={this.onSelect}
              selectedOption={{
                key: this.state.id,
                label: this.props.label
              }}
              size='content'
              scaled={false}
              noBorder={true}
              opened={this.props.opened}
            ></StyledComboBox>
            : <StyledFilterName>{this.props.label}</StyledFilterName> 
          }

        <StyledCloseButtonBlock>
          <CloseButton
            isDisabled={this.props.isDisabled}
            onClick={!this.props.isDisabled ? ((e) => this.props.onClose(e, this.props.id)) : undefined}
          />
        </StyledCloseButtonBlock>
      </StyledFilterItem>
    );
  }
}


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

    this.input = React.createRef();

    function getDefaultFilterData(){
      let filterData = props.getFilterData();
      let filterItems = [];
      let selectedFilterData = cloneProperty(props.selectedFilterData);
      selectedFilterData.forEach(defaultfilterValue => {
          let filterValue = filterData.find(x => ((x.key === defaultfilterValue.key.replace(defaultfilterValue.group + "_",'')) && x.group === defaultfilterValue.group));
          let groupLabel = '';

          let groupFilterItem = filterData.find(x => (x.key === defaultfilterValue.group));
          if(groupFilterItem != undefined){
            groupLabel = groupFilterItem.label;
          }else{
            let subgroupFilterItem = filterData.find(x => (x.subgroup === defaultfilterValue.group))
            if(subgroupFilterItem != undefined){
              groupLabel = subgroupFilterItem.label;
            }
          }

          if(filterValue != undefined){
              defaultfilterValue.key = defaultfilterValue.group + "_" + defaultfilterValue.key;
              defaultfilterValue.label = filterValue.label; 
              defaultfilterValue.groupLabel = groupLabel;
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
      inputValue: props.value,
      filterItems: props.selectedFilterData ? getDefaultFilterData() : [],
      openFilterItems: [],
      hideFilterItems: []
    };
    this.searchWrapper = React.createRef();
    this.filterWrapper = React.createRef();

    this.onClickDropDownItem = this.onClickDropDownItem.bind(this);
    this.getData = this.getData.bind(this);
    this.clearFilter = this.clearFilter.bind(this);
    this.onDeleteFilterItem = this.onDeleteFilterItem.bind(this);
    this.getFilterItems = this.getFilterItems.bind(this);
    this.updateFilter = this.updateFilter.bind(this);
    this.onInputChange = this.onInputChange.bind(this);

    this.resize = this.resize.bind(this);
}

  onClickDropDownItem(event, filterItem){
    let curentFilterItems =  cloneProperty(this.state.filterItems);

    if(filterItem.subgroup != undefined){
      let indexFilterItem = curentFilterItems.findIndex(x => x.group === filterItem.subgroup);
      if(indexFilterItem != -1){
        curentFilterItems.splice(indexFilterItem, 1);
      }
      let subgroupItems = this.props.getFilterData().filter(function(t) {
        return (t.group == filterItem.subgroup);
      });
      if(subgroupItems.length > 1){
        let selectFilterItem = {
          key:  filterItem.subgroup + "_-1",
          group: filterItem.subgroup,
          label:  filterItem.defaultSelectLabel,
          groupLabel: filterItem.label,
          inSubgroup: true
        };
        if(indexFilterItem != -1)
          curentFilterItems.splice(indexFilterItem, 0, selectFilterItem );
        else
          curentFilterItems.push(selectFilterItem);
        this.setState({ filterItems: curentFilterItems}); 
        this.isUpdateFilter = false;
      }else if(subgroupItems.length == 1){
       
        let selectFilterItem = {
          key:  subgroupItems[0].group + "_" + subgroupItems[0].key,
          group: subgroupItems[0].group,
          label:  subgroupItems[0].label,
          groupLabel: this.props.getFilterData().find(x => x.subgroup === subgroupItems[0].group).label,
          inSubgroup: true
        };
        if(indexFilterItem != -1)
          curentFilterItems.splice(indexFilterItem, 0, selectFilterItem );
        else
          curentFilterItems.push(selectFilterItem);
        let clone = cloneProperty(curentFilterItems.filter(function(item) {
          return item.key != '-1';
        }));
        clone.map(function(item){
          item.key = item.key.replace(item.group + "_" ,'');
          return item;
        })
        if(typeof this.props.onChangeFilter === "function")
          this.props.onChangeFilter({
            inputValue: this.state.inputValue,
            filterValue: this.props.isNeedFilter ? 
                          clone.filter(function(item) {
                            return item.key != '-1';
                          }) : 
                          null
          });
        this.setState({ filterItems: curentFilterItems}); 
        this.isUpdateFilter = false;
      }
      

    }else{
      let filterItems = this.getData();

      let indexFilterItem = curentFilterItems.findIndex(x => x.group === filterItem.group);
      if(indexFilterItem != -1){
        curentFilterItems.splice(indexFilterItem, 1);
      }

      let selectFilterItem = {
        key:  filterItem.key,
        group: filterItem.group,
        label:  filterItem.label,
        groupLabel: filterItem.inSubgroup ? filterItems.find(x => x.subgroup === filterItem.group).label : filterItems.find(x => x.key === filterItem.group).label
      };
      if(indexFilterItem != -1)
          curentFilterItems.splice(indexFilterItem, 0, selectFilterItem );
        else
          curentFilterItems.push(selectFilterItem);
      this.setState({ filterItems: curentFilterItems}); 

      let clone = cloneProperty(curentFilterItems.filter(function(item) {
        return item.key != '-1';
      }));
      clone.map(function(item){
        item.key = item.key.replace(item.group + "_" ,'');
        return item;
      })
      if(typeof this.props.onChangeFilter === "function")
        this.props.onChangeFilter({
          inputValue: this.state.inputValue,
          filterValue: this.props.isNeedFilter ? 
                        clone.filter(function(item) {
                          return item.key != '-1';
                        }) : 
                        null
        });
      this.isUpdateFilter = false;
    }
    
  }

  getData(){
    let _this = this;
    let d = this.props.getFilterData();
    let result = [];
    d.forEach(element => {
      if(!element.inSubgroup){
        element.onClick = !element.isSeparator && !element.isHeader && !element.disabled ? ((e) => _this.onClickDropDownItem(e, element)) : undefined;
        element.key = element.group != element.key ? element.group +"_"+ element.key : element.key;
        if(element.subgroup != undefined){
          if(d.findIndex(x => x.group === element.subgroup) == -1) element.disabled = true;
        }
        result.push(element);
      } 
    });
    return result;
  }
  
  clearFilter(){
    if(this.input.current) this.input.current.clearInput();
    this.setState({
      inputValue: '',
      filterItems: [],
      openFilterItems: [],
      hideFilterItems: []
    });
    this.props.onChangeFilter({
      inputValue: '',
      filterValue: []
    });
  }

  onDeleteFilterItem(e , key){

    let curentFilterItems =  this.state.filterItems.slice();
    let indexFilterItem = curentFilterItems.findIndex(x => x.key === key);
    if(indexFilterItem != -1){
      curentFilterItems.splice(indexFilterItem, 1);
    }
    this.setState({ filterItems: curentFilterItems});
    let filterValues = cloneProperty(curentFilterItems);
    filterValues = filterValues.map(function(item){
      item.key = item.key.replace(item.group + "_",'');
      return item;
    })
    if(typeof this.props.onChangeFilter === "function")
      this.props.onChangeFilter({
        inputValue: this.state.inputValue,
        filterValue: this.props.isNeedFilter ? 
                    filterValues.filter(function(item) {
                      return item.key != '-1';
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
              groupItems={_this.props.getFilterData().filter(function(t) {
                return (t.group == item.group && t.group != t.key);
              })}
              onSelectFilterItem={_this.onClickDropDownItem}
              id={item.key} 
              groupLabel={item.groupLabel} 
              label={item.label} 
              opened={item.key.indexOf('_-1') == -1 ? false : true}
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
                groupItems={_this.props.getFilterData().filter(function(t) {
                  return (t.group == item.group && t.group != t.key);
                })}
                onSelectFilterItem={_this.onClickDropDownItem}
                id={item.key} 
                groupLabel={item.groupLabel} 
                opened={false}
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
              groupItems={_this.props.getFilterData().filter(function(t) {
                return (t.group == item.group && t.group != t.key);
              })}
              onSelectFilterItem={_this.onClickDropDownItem}
              id={item.key} 
              groupLabel={item.groupLabel} 
              opened={item.key.indexOf('_-1') == -1 ? false : true}
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
      let newOpenFilterItems = cloneProperty(this.state.filterItems);
      let newHideFilterItems = [];

      let elementsWidth = 0;
      Array.from(this.filterWrapper.current.children).forEach(element => {
        elementsWidth = elementsWidth + element.getBoundingClientRect().width;
      });
      
      if(elementsWidth >= fullWidth/3){
        let filterArr = Array.from(this.filterWrapper.current.children);
        for(let i = 0; i < filterArr.length; i++){
          if(elementsWidth > fullWidth/3){
            elementsWidth = elementsWidth - filterArr[i].getBoundingClientRect().width;
            newHideFilterItems.push(this.state.filterItems.find(x => x.key === filterArr[i].getAttribute('id')));
            newOpenFilterItems.splice(newOpenFilterItems.findIndex(x => x.key === filterArr[i].getAttribute('id')), 1);
          }
        };
      }

      this.setState({ openFilterItems: newOpenFilterItems});
      this.setState({ hideFilterItems: newHideFilterItems});
    }else{
      this.setState({ openFilterItems: this.state.filterItems.slice()});
      this.setState({ hideFilterItems: []});

    }
    this.isUpdatedHiddenItems = true;
    this.isUpdateFilter = true;
  }

  onInputChange(e){
    this.setState({
      inputValue: e.target.value
    });
    this.props.onChange(e)
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
        let filterValue = filterData.find(x => ((x.key === curentFilterItems[i].key.replace(curentFilterItems[i].group + "_",'')) && x.group === curentFilterItems[i].group && !x.inSubgroup));
          if(filterValue){
            curentFilterItems[i].key = curentFilterItems[i].group + "_" + curentFilterItems[i].key;
            curentFilterItems[i].label = filterValue.label; 
            curentFilterItems[i].groupLabel = !filterData.inSubgroup ? filterData.find(x => (x.group === curentFilterItems[i].group)).label : curentFilterItems[i].groupLabel;
            filterItems.push(curentFilterItems[i]);
          }else{
            filterValue = filterData.find(x => ((x.key === curentFilterItems[i].key.replace(curentFilterItems[i].group + "_",'')) && x.group === curentFilterItems[i].group && x.inSubgroup));
            if(filterValue){
              curentFilterItems[i].key = curentFilterItems[i].group + "_" + curentFilterItems[i].key;
              curentFilterItems[i].label = filterValue.label; 
              curentFilterItems[i].groupLabel = filterData.find(x => (x.subgroup === curentFilterItems[i].group)).label;
              filterItems.push(curentFilterItems[i]);
            }else{
              filterValue = filterData.find(x => ((x.subgroup === curentFilterItems[i].group)));
              if(filterValue){
                let subgroupItems = this.props.getFilterData().filter(function(t) {
                  return (t.group == filterValue.subgroup);
                });
                if(subgroupItems.length > 1){
                  curentFilterItems[i].key = curentFilterItems[i].group + "_-1";
                  curentFilterItems[i].label = filterValue.defaultSelectLabel; 
                  curentFilterItems[i].groupLabel = filterData.find(x => (x.subgroup === curentFilterItems[i].group)).label;
                  filterItems.push(curentFilterItems[i]);
                }else if(subgroupItems.length == 1){

                  let selectFilterItem = {
                    key:  subgroupItems[0].group + "_" + subgroupItems[0].key,
                    group: subgroupItems[0].group,
                    label:  subgroupItems[0].label,
                    groupLabel: this.props.getFilterData().find(x => x.subgroup === subgroupItems[0].group).label,
                    inSubgroup: true
                  };
                  curentFilterItems.push(selectFilterItem);
                }
              }
            }
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
      for (let propsKey in this.props) {
        if(typeof this.props[propsKey] != "function" && typeof this.props[propsKey] != "object" && this.props[propsKey] != nextProps[propsKey]){
          if(propsKey=='value') this.setState({inputValue: nextProps[propsKey]})
          isNeedUpdate = true;
          break;
        }
      }

      if(!isNeedUpdate){
        for (let key in this.state) {
          if(this.state[key].length != nextState[key].length){
            isNeedUpdate = true;
          }else{
            for(let i = 0; i < this.state[key].length; i++){
              if(typeof nextState[key] == "object" && nextState[key].find(x => (x.key === this.state[key][i].key && x.group === this.state[key][i].group)) == undefined){
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
    let clearButtonSize = 15;
    switch (this.props.size) {
      case 'base':
        iconSize = 32;
        clearButtonSize = !!this.state.inputValue || this.state.filterItems.length > 0 ? 12 : 15;
        break;
      case 'middle':
        iconSize = 41;
        clearButtonSize = !!this.state.inputValue || this.state.filterItems.length > 0 ? 16 : 18;
        break;
      case 'big':
        iconSize = 41;
        clearButtonSize = !!this.state.inputValue || this.state.filterItems.length > 0 ? 19 : 21;
        break;
      case 'huge':
        iconSize = 41;
        clearButtonSize = !!this.state.inputValue || this.state.filterItems.length > 0 ? 22 : 24;
        break;
      default:
        break;
    }
    return (
      <StyledSearchInput ref={this.searchWrapper}> 
        <InputBlock 
          ref={this.input}
          id={this.props.id}
          isDisabled={this.props.isDisabled}
          iconName={!!this.state.inputValue || this.state.filterItems.length > 0 ? "CrossIcon" : "SearchIcon"}
          isIconFill={true}
          iconSize={clearButtonSize}
          iconColor={"#A3A9AE"}
          onIconClick={!!this.state.inputValue || this.state.filterItems.length > 0 ? this.clearFilter : undefined }
          size={this.props.size}
          scale={true}
          value={this.state.inputValue}
          placeholder={this.props.placeholder}
          onChange={this.onInputChange}
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