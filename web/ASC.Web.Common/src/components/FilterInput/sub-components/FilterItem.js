import React from "react";
import { ComboBox } from "asc-web-components";
import CloseButton from "./CloseButton";
import PropTypes from "prop-types";
import {
  StyledFilterItem,
  StyledFilterItemContent,
  StyledCloseButtonBlock,
} from "../StyledFilterInput";
import GroupSelector from "../../GroupSelector";
import PeopleSelector from "../../PeopleSelector";
import isEqual from "lodash/isEqual";

class FilterItem extends React.Component {
  constructor(props) {
    super(props);
    const { id, selectedItem, typeSelector } = props;

    const selectedOption =
      selectedItem &&
      selectedItem.key &&
      (typeSelector === selectedItem.type || id.includes(typeSelector))
        ? {
            key: selectedItem.key,
            label: selectedItem.label,
          }
        : {
            key: null,
            label: this.props.defaultSelectLabel,
            default: true,
          };

    const isOpenSelector = Boolean(selectedOption.key);

    this.state = {
      id,
      isOpen: false,
      isOpenSelector: !isOpenSelector,
      selectedOption,
    };
  }

  componentDidUpdate(prevProps, prevState) {
    const { selectedItem, defaultSelectLabel } = this.props;

    if (
      selectedItem &&
      selectedItem.key !== this.state.selectedOption.key &&
      selectedItem.key !== this.state.selectedOption.key &&
      selectedItem.key !== prevProps.selectedItem.key
    ) {
      const selectedOption = selectedItem.key
        ? {
            key: selectedItem.key,
            label: selectedItem.label,
          }
        : {
            key: null,
            label: defaultSelectLabel,
            default: true,
          };
      const isOpenSelector = Boolean(selectedOption.key);
      this.setState({
        isOpenSelector: !isOpenSelector,
        selectedOption,
      });
    }
  }

  shouldComponentUpdate(nextProps, nextState) {
    if (!isEqual(this.props, nextProps) || !isEqual(this.state, nextState)) {
      return true;
    }
    return false;
  }

  onSelect = (option) => {
    const { group, key, label, inSubgroup } = option;
    const filterItem = {
      key: group + "_" + key,
      label,
      group,
      inSubgroup: !!inSubgroup,
    };
    this.props.setShowHiddenFilter && this.props.setShowHiddenFilter(false);
    this.props.onSelectFilterItem(filterItem);
  };

  onClick = () => {
    const { isDisabled, id, onClose } = this.props;
    !isDisabled && onClose(id);
  };

  toggleCombobox = (e, isOpen) => this.setState({ isOpen });

  onCancelSelector = (e) => {
    if (
      this.state.isOpenSelector &&
      (e.target.id === "filter-selector_button" ||
        e.target.closest("#filter-selector_button"))
    ) {
      // Skip double set of isOpen property
      return;
    }
    this.setState({ isOpenSelector: false });
  };

  onSelectGroup = (selected) => {
    const { key, label } = selected[0];
    const selectedOption = {
      key,
      label,
    };
    this.setState({
      selectedOption,
      isOpenSelector: false,
    });

    const {
      onSelectFilterItem,
      id,
      groupItems,
      typeSelector,
      defaultOption,
      groupsCaption,
      defaultOptionLabel,
      defaultSelectLabel,
    } = this.props;

    onSelectFilterItem({
      isSelector: true,
      key: id,
      group: groupItems[0].group, //hack, replace it: this.props.id.slice(0, this.props.id.indexOf('_'));
      label: this.props.label,
      typeSelector,
      defaultOption,
      groupsCaption,
      defaultOptionLabel,
      defaultSelectLabel,
      selectedItem: selected[0],
    });
  };

  onPeopleSelectorClick = () => {
    this.setState({ isOpenSelector: !this.state.isOpenSelector });
  };

  render() {
    //console.log("FilterItem render");

    const { id, isOpen, isOpenSelector, selectedOption } = this.state;

    const {
      block,
      opened,
      isDisabled,
      groupLabel,
      groupItems,
      label,
      typeSelector,
      defaultOptionLabel,
      groupsCaption,
      defaultOption,
    } = this.props;

    return (
      <StyledFilterItem key={id} id={id} block={block}>
        <StyledFilterItemContent isDisabled={isDisabled} isOpen={isOpen}>
          {typeSelector === "group" && (
            <>
              {groupLabel}:
              <ComboBox
                id="filter-selector_button"
                className="styled-combobox"
                options={[]}
                opened={isOpenSelector}
                selectedOption={selectedOption}
                size="content"
                toggleAction={this.onPeopleSelectorClick}
                displayType="toggle"
                scaled={false}
                noBorder={true}
                directionX="left"
                dropDownMaxHeight={200}
              ></ComboBox>
              <GroupSelector
                isOpen={isOpenSelector}
                isMultiSelect={false}
                onCancel={this.onCancelSelector}
                onSelect={this.onSelectGroup}
              />
            </>
          )}
          {typeSelector === "user" && (
            <>
              {groupLabel}:
              <ComboBox
                id="filter-selector_button"
                className="styled-combobox"
                options={[]}
                opened={isOpenSelector}
                selectedOption={selectedOption}
                size="content"
                toggleAction={this.onPeopleSelectorClick}
                displayType="toggle"
                scaled={false}
                noBorder={true}
                directionX="left"
                dropDownMaxHeight={200}
              ></ComboBox>
              <PeopleSelector
                isOpen={isOpenSelector}
                groupsCaption={groupsCaption}
                defaultOption={defaultOption}
                defaultOptionLabel={defaultOptionLabel}
                onCancel={this.onCancelSelector}
                onSelect={this.onSelectGroup}
              />
            </>
          )}
          {!typeSelector && (
            <>
              {groupLabel}:
              {groupItems.length > 1 ? (
                <ComboBox
                  className="styled-combobox"
                  options={groupItems}
                  isDisabled={isDisabled}
                  onSelect={this.onSelect}
                  selectedOption={{
                    key: id,
                    label,
                  }}
                  size="content"
                  scaled={false}
                  noBorder={true}
                  opened={opened}
                  directionX="left"
                  toggleAction={this.toggleCombobox}
                  dropDownMaxHeight={200}
                ></ComboBox>
              ) : (
                <span className="styled-filter-name">{label}</span>
              )}
            </>
          )}
        </StyledFilterItemContent>

        <StyledCloseButtonBlock
          onClick={this.onClick}
          isDisabled={isDisabled}
          isClickable={true}
        >
          <CloseButton isDisabled={isDisabled} onClick={this.onClick} />
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
  onSelectFilterItem: PropTypes.func,
  typeSelector: PropTypes.string,
  groupsCaption: PropTypes.string,
  defaultOptionLabel: PropTypes.string,
  defaultOption: PropTypes.object,
  selectedItem: PropTypes.object,
  defaultSelectLabel: PropTypes.string,
};

export default FilterItem;
