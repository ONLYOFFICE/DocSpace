import React from 'react';
import styled, {css} from 'styled-components';
import FilterButton from './filter-button';
import HideFilter from './hide-filter';
import ComboBox from '../combobox';
import CloseButton from './close-button';
import isEqual from 'lodash/isEqual';
import PropTypes from 'prop-types';

const StyledFilterBlock = styled.div`
  display: flex;
  align-items: center;
`;

const StyledFilterItem = styled.div`
  display:  ${props => props.block ? 'flex' : 'inline-block'};
  margin-bottom: ${props => props.block ? '8px' : '0'};
  position: relative;
  height: 100%;
  margin-right: 2px;
  border: 1px solid #ECEEF1;
  border-radius: 3px;
  background-color: #F8F9F9;
  padding-right: 22px;
  
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  box-sizing: border-box;
  color: #555F65;

  &:last-child{
    margin-bottom: 0;
  }
`;

const StyledFilterItemContent = styled.div`
  display: flex;
  padding: 5px 4px 2px 7px;
  width: 100%;
  ${props =>
    props.isOpen && !props.isDisabled &&
    css`
      background: #ECEEF1;
  `}
  ${props =>
    !props.isDisabled &&
    css`
      &:active{
        background: #ECEEF1;
      }
  `}
`;

const StyledCloseButtonBlock = styled.div`
  display: flex;
  cursor: ${props =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};
  align-items: center;
  position: absolute;
  height: 100%;
  width: 25px;
  border-left: 1px solid #ECEEF1;
  right: 0;
  top: 0;
  background-color: #F8F9F9;
  ${props =>
    !props.isDisabled &&
    css`
      &:active{
        background: #ECEEF1;
        svg path:first-child { 
          fill: #333; 
        }
      }
  `}
`;
const StyledComboBox = styled(ComboBox)`
  display: inline-block;
  background: transparent;
  max-width: 185px;
  cursor: pointer;
  vertical-align: middle;
  margin-top: -1px;
  > div:first-child{
    width: auto;
    padding-left: 4px;
  }
  .combo-button-label {
    color: #555F65;
  }
`;
const StyledFilterName = styled.span`
  line-height: 18px;
  margin-left: 5px;
`;

class FilterItem extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      id: this.props.id,
      isOpen: false
    };
  }

  onSelect = (option) => {
    this.props.onSelectFilterItem(null, {
      key: option.group + "_" + option.key,
      label: option.label,
      group: option.group,
      inSubgroup: !!option.inSubgroup
    });
  }
  onClick = () => {
    !this.props.isDisabled && this.props.onClose(this.props.id);
  }

  render() {
    return (
      <StyledFilterItem key={this.state.id} id={this.state.id} block={this.props.block} opened={this.props.opened} >
        <StyledFilterItemContent isDisabled={this.props.isDisabled} isOpen={this.state.isOpen}>
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
              directionX='left'
              toggleAction={(e,isOpen)=>{
                this.setState({
                  isOpen: isOpen
                })
              }}
            ></StyledComboBox>
            : <StyledFilterName>{this.props.label}</StyledFilterName>
          }
        </StyledFilterItemContent>
        

        <StyledCloseButtonBlock onClick={this.onClick} isDisabled={this.props.isDisabled} isClickable={true}>
          <CloseButton
            isDisabled={this.props.isDisabled}
            onClick={this.onClick}
          />
        </StyledCloseButtonBlock>
      </StyledFilterItem>
    );
  }
}
FilterItem.propTypes = {
  id: PropTypes.string,
  opened: PropTypes.bool,
  isDisabled: PropTypes.bool,
  block: PropTypes.bool,
  groupItems: PropTypes.array,
  label: PropTypes.string,
  groupLabel: PropTypes.string,
  onClose:PropTypes.func,
  onSelectFilterItem:PropTypes.func
}

class FilterBlock extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      hideFilterItems: this.props.hideFilterItems || [],
      openFilterItems: this.props.openFilterItems || []
    };

  }
  onDeleteFilterItem = (key) => {
    this.props.onDeleteFilterItem(key);
  }
  getFilterItems = () => {
    const _this = this;
    let result = [];
    let openItems = [];
    let hideItems = [];
    if (this.state.openFilterItems.length > 0) {
      openItems = this.state.openFilterItems.map(function (item) {
        return <FilterItem
          block={false}
          isDisabled={_this.props.isDisabled}
          key={item.key}
          groupItems={_this.props.getFilterData().filter(function (t) {
            return (t.group == item.group && t.group != t.key);
          })}
          onSelectFilterItem={_this.props.onClickFilterItem}
          id={item.key}
          groupLabel={item.groupLabel}
          label={item.label}
          opened={item.key.indexOf('_-1') == -1 ? false : true}
          onClose={_this.onDeleteFilterItem}>
        </FilterItem>
      });
    }
    if (this.state.hideFilterItems.length > 0) {
      var open = false;
      var hideFilterItemsList = this.state.hideFilterItems.map(function (item) {
        open = item.key.indexOf('_-1') == -1 ? false : true
        return <FilterItem
          block={true}
          isDisabled={_this.props.isDisabled}
          key={item.key}
          groupItems={_this.props.getFilterData().filter(function (t) {
            return (t.group == item.group && t.group != t.key);
          })}
          onSelectFilterItem={_this.props.onClickFilterItem}
          id={item.key}
          groupLabel={item.groupLabel}
          opened={item.key.indexOf('_-1') == -1 ? false : true}
          label={item.label}
          onClose={_this.onDeleteFilterItem}>
        </FilterItem>
      })
      hideItems.push(
        <HideFilter key="hide-filter" count={this.state.hideFilterItems.length} isDisabled={this.props.isDisabled} open={open}>
          {
            hideFilterItemsList
          }
        </HideFilter>
      );
    }
    result = hideItems.concat(openItems);
    return result;
  }
  getData = () => {
    const _this = this;
    const d = this.props.getFilterData();
    let result = [];
    d.forEach(element => {
      if (!element.inSubgroup) {
        element.onClick = !element.isSeparator && !element.isHeader && !element.disabled ? ((e) => _this.props.onClickFilterItem(e, element)) : undefined;
        element.key = element.group != element.key ? element.group + "_" + element.key : element.key;
        if (element.subgroup != undefined) {
          if (d.findIndex(x => x.group === element.subgroup) == -1) element.disabled = true;
        }
        result.push(element);
      }
    });
    return result;
  }
  componentDidUpdate() {
    this.props.onRender();
  }
  shouldComponentUpdate(nextProps, nextState) {

    if (!isEqual(this.props, nextProps)) {
      if (!isEqual(this.props.hideFilterItems, nextProps.hideFilterItems) || !isEqual(this.props.openFilterItems, nextProps.openFilterItems)) {
        this.setState({
          hideFilterItems: nextProps.hideFilterItems,
          openFilterItems: nextProps.openFilterItems
        });
        return false;
      }
      return true;
    }
    if (this.props.isResizeUpdate) {
      return true;
    }
    return !isEqual(this.state, nextState);
  }
  render() {
    const _this = this;
    const filterItems = this.getFilterItems();
    const filterData = this.props.getFilterData();
    return (
      <>
        <StyledFilterBlock ref={this.filterWrapper} id='filter-items-container'>
          {filterItems}
        </StyledFilterBlock>
        {filterData.length > 0 && <FilterButton id='filter-button' iconSize={this.props.iconSize} getData={_this.getData} isDisabled={this.props.isDisabled} />}
      </>
    );
  }
}
FilterBlock.propTypes = {
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isDisabled: PropTypes.bool,
  isResizeUpdate: PropTypes.bool,
  hideFilterItems: PropTypes.array,
  openFilterItems: PropTypes.array,
  onRender: PropTypes.func,
  onDeleteFilterItem: PropTypes.func,
  getFilterData: PropTypes.func
}

export default FilterBlock;