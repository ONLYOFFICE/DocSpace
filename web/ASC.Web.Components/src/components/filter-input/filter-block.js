import React from 'react';
import styled from 'styled-components';
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
  top: 1px;
`;
const StyledComboBox = styled(ComboBox)`
  display: inline-block;
  background: transparent;
  max-width: 185px;
  cursor: pointer;
  vertical-align: middle;
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

class FilterBlock extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      hideFilterItems: this.props.hideFilterItems || [],
      openFilterItems: this.props.openFilterItems || []
    };

    this.getData = this.getData.bind(this);
    this.getFilterItems = this.getFilterItems.bind(this);
    this.onDeleteFilterItem = this.onDeleteFilterItem.bind(this);

  }
  onDeleteFilterItem(e, key) {
    this.props.onDeleteFilterItem(key);
  }
  getFilterItems() {
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
                onSelectFilterItem={_this.props.onClickFilterItem}
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
  getData() {
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
    return (
      <>
        <StyledFilterBlock ref={this.filterWrapper} id='filter-items-container'>
          {this.getFilterItems()}
        </StyledFilterBlock>
        <FilterButton id='filter-button' iconSize={this.props.iconSize} getData={_this.getData} isDisabled={this.props.isDisabled} />
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
  onDeleteFilterItem: PropTypes.func
}

export default FilterBlock;