import React from 'react';
import FilterButton from './FilterButton';
import HideFilter from './HideFilter';
import throttle from 'lodash/throttle';
import { ComboBox } from 'asc-web-components';
import CloseButton from './CloseButton';
import isEqual from 'lodash/isEqual';
import PropTypes from 'prop-types';
import { StyledFilterItem, StyledFilterItemContent, StyledCloseButtonBlock } from '../StyledFilterInput';

class FilterItem extends React.Component {
  constructor(props) {
    super(props);

    const { id } = props;

    this.state = {
      id,
      isOpen: false
    };
  }

  onSelect = (option) => {
    const { group, key, label, inSubgroup } = option;
    this.props.onSelectFilterItem(null, {
      key: group + "_" + key,
      label,
      group,
      inSubgroup: !!inSubgroup
    });
  }
  onClick = () => {
    const { isDisabled, id, onClose } = this.props;
    !isDisabled && onClose(id);
  }

  toggleCombobox= (e, isOpen) => this.setState({ isOpen });

  render() {
    const { id, isOpen } = this.state;
    const { block, opened, isDisabled, groupLabel,
      groupItems, label } = this.props;
    return (
      <StyledFilterItem key={id} id={id} block={block} opened={opened} >
        <StyledFilterItemContent isDisabled={isDisabled} isOpen={isOpen}>
          {groupLabel}:
              {groupItems.length > 1 ?
            <ComboBox
              className='styled-combobox'
              options={groupItems}
              isDisabled={isDisabled}
              onSelect={this.onSelect}
              selectedOption={{
                key: id,
                label
              }}
              size='content'
              scaled={false}
              noBorder={true}
              opened={opened}
              directionX='left'
              toggleAction={this.toggleCombobox}
              dropDownMaxHeight={200}
            ></ComboBox>
            : <span className='styled-filter-name'>{label}</span>
          }
        </StyledFilterItemContent>


        <StyledCloseButtonBlock onClick={this.onClick} isDisabled={isDisabled} isClickable={true}>
          <CloseButton
            isDisabled={isDisabled}
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
  onClose: PropTypes.func,
  onSelectFilterItem: PropTypes.func
}

class FilterBlock extends React.Component {
  constructor(props) {
    super(props);

    const { hideFilterItems, openFilterItems } = props;

    this.state = {
      hideFilterItems: hideFilterItems || [],
      openFilterItems: openFilterItems || []
    };

    this.throttledRender = throttle(this.onRender, 100);

  }

  componentDidUpdate() {
    this.throttledRender();
  }

  shouldComponentUpdate(nextProps, nextState) {

    const { hideFilterItems, openFilterItems } = nextProps;

    if (!isEqual(this.props, nextProps)) {
      if (!isEqual(this.props.hideFilterItems, hideFilterItems) || !isEqual(this.props.openFilterItems, openFilterItems)) {
        this.setState({
          hideFilterItems,
          openFilterItems
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

  onDeleteFilterItem = (key) => {
    this.props.onDeleteFilterItem(key);
  }
  getFilterItems = () => {
    const { openFilterItems, hideFilterItems } = this.state;
    const _this = this;
    let result = [];
    let openItems = [];
    let hideItems = [];
    if (openFilterItems.length > 0) {
      openItems = openFilterItems.map(function (item) {
        const { key, group, groupLabel, label } = item;
        return <FilterItem
          block={false}
          isDisabled={_this.props.isDisabled}
          key={key}
          groupItems={_this.props.getFilterData().filter(function (t) {
            return (t.group == group && t.group != t.key);
          })}
          onSelectFilterItem={_this.props.onClickFilterItem}
          id={key}
          groupLabel={groupLabel}
          label={label}
          opened={key.indexOf('_-1') == -1 ? false : true}
          onClose={_this.onDeleteFilterItem}>
        </FilterItem>
      });
    }
    if (hideFilterItems.length > 0) {
      let open = false;
      let hideFilterItemsList = hideFilterItems.map(function (item) {
        const { key, group, groupLabel, label } = item;
        open = key.indexOf('_-1') == -1 ? false : true
        return <FilterItem
          block={true}
          isDisabled={_this.props.isDisabled}
          key={key}
          groupItems={_this.props.getFilterData().filter(function (t) {
            return (t.group == group && t.group != t.key);
          })}
          onSelectFilterItem={_this.props.onClickFilterItem}
          id={key}
          groupLabel={groupLabel}
          opened={key.indexOf('_-1') == -1 ? false : true}
          label={label}
          onClose={_this.onDeleteFilterItem}>
        </FilterItem>
      })
      hideItems.push(
        <HideFilter key="hide-filter" count={hideFilterItems.length} isDisabled={this.props.isDisabled} open={open}>
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

  onRender = () => {
    this.props.onRender();
  }
  render() {
    const _this = this;
    const filterItems = this.getFilterItems();
    const filterData = this.props.getFilterData();
    const { iconSize, isDisabled } = this.props;
    return (
      <>
        <div className='styled-filter-block' ref={this.filterWrapper} id='filter-items-container'>
          {filterItems}
        </div>
        {filterData.length > 0 && <FilterButton id='filter-button' iconSize={iconSize} getData={_this.getData} isDisabled={isDisabled} />}
      </>
    );
  }
}
FilterBlock.propTypes = {
  getFilterData: PropTypes.func,
  hideFilterItems: PropTypes.array,
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isDisabled: PropTypes.bool,
  isResizeUpdate: PropTypes.bool,
  onDeleteFilterItem: PropTypes.func,
  onRender: PropTypes.func,
  openFilterItems: PropTypes.array,
}

export default FilterBlock;