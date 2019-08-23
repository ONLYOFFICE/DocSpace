import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';
import ComboBox from '../combobox';

import FilterButton from './filter-button';
import HideFilter from './hide-filter';
import CloseButton from './close-button';

import isEqual from 'lodash/isEqual';
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
  position: relative;
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

    this.state = {
      id: this.props.id
    };

    this.onSelect = this.onSelect.bind(this);
  }

  onSelect(option) {
    this.props.onSelectFilterItem(null, {
      key: option.group + "_" + option.key,
      label: option.label,
      group: option.group,
      inSubgroup: !!option.inSubgroup
    });
  }

  render() {
    return (
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

const cloneObjectsArray = function (props) {
  return _.map(props, _.clone);;
}

const convertToInternalData = function (fullDataArray, inputDataArray) {
  let filterItems = [];
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
          let subgroupItems = fullDataArray.filter(function (t) {
            return (t.group == filterValue.subgroup);
          });
          if (subgroupItems.length > 1) {
            inputDataArray[i].key = inputDataArray[i].group + "_-1";
            inputDataArray[i].label = filterValue.defaultSelectLabel;
            inputDataArray[i].groupLabel = fullDataArray.find(x => (x.subgroup === inputDataArray[i].group)).label;
            filterItems.push(inputDataArray[i]);
          } else if (subgroupItems.length == 1) {

            let selectFilterItem = {
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

class SearchInput extends React.Component {
  constructor(props) {
    super(props);

    this.input = React.createRef();

    function getDefaultFilterData() {
      let filterData = props.getFilterData();
      let filterItems = [];
      let selectedFilterData = cloneObjectsArray(props.selectedFilterData);
      selectedFilterData.forEach(defaultFilterValue => {
        let filterValue = filterData.find(x => ((x.key === defaultFilterValue.key.replace(defaultFilterValue.group + "_", '')) && x.group === defaultFilterValue.group));
        let groupLabel = '';

        let groupFilterItem = filterData.find(x => (x.key === defaultFilterValue.group));
        if (groupFilterItem != undefined) {
          groupLabel = groupFilterItem.label;
        } else {
          let subgroupFilterItem = filterData.find(x => (x.subgroup === defaultFilterValue.group))
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

    this.minWidth = 190;
    this.isResizeUpdate = false;
    this.state = {
      inputValue: props.value,
      filterItems: props.selectedFilterData && props.isNeedFilter ? getDefaultFilterData() : [],
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

    this.throttledResize = _.throttle(this.resize, 300);
  }

  onClickDropDownItem(event, filterItem) {
    let currentFilterItems = cloneObjectsArray(this.state.filterItems);

    if (!!filterItem.subgroup) {
      let indexFilterItem = currentFilterItems.findIndex(x => x.group === filterItem.subgroup);
      if (indexFilterItem != -1) {
        currentFilterItems.splice(indexFilterItem, 1);
      }
      let subgroupItems = this.props.getFilterData().filter(function (t) {
        return (t.group == filterItem.subgroup);
      });
      if (subgroupItems.length > 1) {
        let selectFilterItem = {
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
        this.setState({ filterItems: currentFilterItems });
        this.updateFilter(currentFilterItems);
      } else if (subgroupItems.length == 1) {

        let selectFilterItem = {
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
        let clone = cloneObjectsArray(currentFilterItems.filter(function (item) {
          return item.key != '-1';
        }));
        clone.map(function (item) {
          item.key = item.key.replace(item.group + "_", '');
          return item;
        })
        if (typeof this.props.onChangeFilter === "function")
          this.props.onChangeFilter({
            inputValue: this.state.inputValue,
            filterValues: this.props.isNeedFilter ?
              clone.filter(function (item) {
                return item.key != '-1';
              }) :
              null
          });
        this.setState({ filterItems: currentFilterItems });
        this.updateFilter(currentFilterItems);
      }


    } else {
      let filterItems = this.getData();

      let indexFilterItem = currentFilterItems.findIndex(x => x.group === filterItem.group);
      if (indexFilterItem != -1) {
        currentFilterItems.splice(indexFilterItem, 1);
      }

      let selectFilterItem = {
        key: filterItem.key,
        group: filterItem.group,
        label: filterItem.label,
        groupLabel: filterItem.inSubgroup ? filterItems.find(x => x.subgroup === filterItem.group).label : filterItems.find(x => x.key === filterItem.group).label
      };
      if (indexFilterItem != -1)
        currentFilterItems.splice(indexFilterItem, 0, selectFilterItem);
      else
        currentFilterItems.push(selectFilterItem);
      this.setState({ filterItems: currentFilterItems });
      this.updateFilter(currentFilterItems);

      let clone = cloneObjectsArray(currentFilterItems.filter(function (item) {
        return item.key != '-1';
      }));
      clone.map(function (item) {
        item.key = item.key.replace(item.group + "_", '');
        return item;
      })
      if (typeof this.props.onChangeFilter === "function")
        this.props.onChangeFilter({
          inputValue: this.state.inputValue,
          filterValues: this.props.isNeedFilter ?
            clone.filter(function (item) {
              return item.key != '-1';
            }) :
            null
        });
    }

  }

  getData() {
    let _this = this;
    let d = this.props.getFilterData();
    let result = [];
    d.forEach(element => {
      if (!element.inSubgroup) {
        element.onClick = !element.isSeparator && !element.isHeader && !element.disabled ? ((e) => _this.onClickDropDownItem(e, element)) : undefined;
        element.key = element.group != element.key ? element.group + "_" + element.key : element.key;
        if (element.subgroup != undefined) {
          if (d.findIndex(x => x.group === element.subgroup) == -1) element.disabled = true;
        }
        result.push(element);
      }
    });
    return result;
  }

  clearFilter() {
    if (this.input.current) this.input.current.clearInput();
    this.setState({
      inputValue: '',
      filterItems: [],
      openFilterItems: [],
      hideFilterItems: []
    });
    this.props.onChangeFilter({
      inputValue: '',
      filterValues: []
    });
  }

  onDeleteFilterItem(e, key) {

    let currentFilterItems = this.state.filterItems.slice();
    let indexFilterItem = currentFilterItems.findIndex(x => x.key === key);
    if (indexFilterItem != -1) {
      currentFilterItems.splice(indexFilterItem, 1);
    }
    this.setState({ filterItems: currentFilterItems });
    this.updateFilter(currentFilterItems);
    let filterValues = cloneObjectsArray(currentFilterItems);
    filterValues = filterValues.map(function (item) {
      item.key = item.key.replace(item.group + "_", '');
      return item;
    })
    if (typeof this.props.onChangeFilter === "function")
      this.props.onChangeFilter({
        inputValue: this.state.inputValue,
        filterValues: this.props.isNeedFilter ?
          filterValues.filter(function (item) {
            return item.key != '-1';
          }) : null
      });
  }

  getFilterItems() {
    let _this = this;
    let result = [];
    let openItems = [];
    let hideItems = [];
    if ((this.state.filterItems.length > 0 && this.state.hideFilterItems.length === 0 && this.state.openFilterItems.length === 0) || this.state.openFilterItems.length > 0) {
      const filterItemsArray = this.state.openFilterItems.length > 0 ? this.state.openFilterItems : this.state.filterItems;
      openItems = filterItemsArray.map(function (item) {
        return <FilterItem
          isDisabled={_this.props.isDisabled}
          key={item.key}
          groupItems={_this.props.getFilterData().filter(function (t) {
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
    if (this.state.hideFilterItems.length > 0) {
      hideItems.push(
        <HideFilter key="hide-filter" count={this.state.hideFilterItems.length} isDisabled={this.props.isDisabled}>
          {
            this.state.hideFilterItems.map(function (item) {
              return <FilterItem
                block={true}
                isDisabled={_this.props.isDisabled}
                key={item.key}
                groupItems={_this.props.getFilterData().filter(function (t) {
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
    result = hideItems.concat(openItems);
    return result;
  }

  updateFilter(inputFilterItems) {

    const currentFilterItems = inputFilterItems || cloneObjectsArray(this.state.filterItems);
    let fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
    let filterWidth = this.filterWrapper.current.getBoundingClientRect().width;

    if (fullWidth <= this.minWidth) {
      this.setState({ openFilterItems: [] });
      this.setState({ hideFilterItems: currentFilterItems.slice() });
    } else if (filterWidth > fullWidth / 2) {
      let newOpenFilterItems = cloneObjectsArray(currentFilterItems);
      let newHideFilterItems = [];

      let elementsWidth = 0;
      Array.from(this.filterWrapper.current.children).forEach(element => {
        elementsWidth = elementsWidth + element.getBoundingClientRect().width;
      });

      if (elementsWidth >= fullWidth / 3) {
        let filterArr = Array.from(this.filterWrapper.current.children);
        for (let i = 0; i < filterArr.length; i++) {
          if (elementsWidth > fullWidth / 3) {
            elementsWidth = elementsWidth - filterArr[i].getBoundingClientRect().width;
            newHideFilterItems.push(currentFilterItems.find(x => x.key === filterArr[i].getAttribute('id')));
            newOpenFilterItems.splice(newOpenFilterItems.findIndex(x => x.key === filterArr[i].getAttribute('id')), 1);
          }
        };
      }
      this.setState({ openFilterItems: newOpenFilterItems });
      this.setState({ hideFilterItems: newHideFilterItems });
    } else {
      this.setState({ openFilterItems: currentFilterItems.slice() });
      this.setState({ hideFilterItems: [] });
    }
  }

  onInputChange(e) {
    this.setState({
      inputValue: e.target.value
    });
    this.props.onChange(e)
  }

  resize() {
    this.isResizeUpdate = true;
    //this.forceUpdate();
    this.setState({
      openFilterItems: this.state.filterItems,
      hideFilterItems: []
    })
  }
  componentDidUpdate() {
    if (this.props.isNeedFilter) {
      const fullWidth = this.searchWrapper.current.getBoundingClientRect().width;
      const filterWidth = this.filterWrapper.current.getBoundingClientRect().width;
      if (fullWidth <= this.minWidth || filterWidth > fullWidth / 2) this.updateFilter();
    }
  }
  componentDidMount() {
    window.addEventListener('resize', this.throttledResize);
    if (this.props.isNeedFilter) this.updateFilter();
  }
  componentWillUnmount(){
    window.removeEventListener('resize', this.throttledResize);
  }
  shouldComponentUpdate(nextProps, nextState) {
    if (this.props.value != nextProps.value || !isEqual(this.props.selectedFilterData, nextProps.selectedFilterData)) {
      if (this.props.value != nextProps.value) {
        this.setState({ inputValue: nextProps.value })
      }
      if (!isEqual(this.props.selectedFilterData, nextProps.selectedFilterData) && this.props.isNeedFilter) {
        const internalFilterData = convertToInternalData(this.props.getFilterData(), cloneObjectsArray(nextProps.selectedFilterData));
        this.setState({ filterItems: internalFilterData });
        this.updateFilter(internalFilterData);
      }
      return true;
    }
    if (this.props.id != nextProps.id ||
      this.props.isDisabled != nextProps.isDisabled ||
      this.props.size != nextProps.size ||
      this.props.placeholder != nextProps.placeholder ||
      this.props.isNeedFilter != nextProps.isNeedFilter
    ) {
      return true;
    }
    if (this.isResizeUpdate) {
      this.isResizeUpdate = false;
      return true;
    }
    return !isEqual(this.state, nextState);
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
          onIconClick={!!this.state.inputValue || this.state.filterItems.length > 0 ? this.clearFilter : undefined}
          size={this.props.size}
          scale={true}
          value={this.state.inputValue}
          placeholder={this.props.placeholder}
          onChange={this.onInputChange}
        >
          {this.props.isNeedFilter &&
            <StyledFilterBlock ref={this.filterWrapper}>
              {this.getFilterItems()}
            </StyledFilterBlock>
          }

          {this.props.isNeedFilter &&
            <FilterButton iconSize={iconSize} getData={_this.getData} isDisabled={this.props.isDisabled} />
          }
        </InputBlock>
      </StyledSearchInput>
    );
  }
};

SearchInput.propTypes = {
  id: PropTypes.string,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  value: PropTypes.string,
  scale: PropTypes.bool,
  placeholder: PropTypes.string,
  onChange: PropTypes.func,
  getFilterData: PropTypes.func,
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