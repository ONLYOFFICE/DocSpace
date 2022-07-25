import React from "react";
import equal from "fast-deep-equal/react";
import FieldContainer from "@docspace/components/field-container";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";
import GroupSelector from "../../../../../components/GroupSelector";

class DepartmentField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  onRemoveGroup = (e) => {
    const groupId = e.currentTarget.parentElement.dataset.id;
    this.props.onRemoveGroup(groupId);
  };

  render() {
    const {
      isRequired,
      isDisabled,
      hasError,
      labelText,

      showGroupSelectorButtonTitle,

      onShowGroupSelector,
      onCloseGroupSelector,

      selectorIsVisible,
      selectorSelectedOptions,
      selectorOnSelectGroups,
      maxLabelWidth,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        className="departments-field"
        maxLabelWidth={maxLabelWidth}
      >
        <SelectorAddButton
          isDisabled={isDisabled}
          title={showGroupSelectorButtonTitle}
          onClick={onShowGroupSelector}
          className="department-add-btn"
        />
        {selectorIsVisible && (
          <GroupSelector
            isOpen={selectorIsVisible}
            isMultiSelect={true}
            onSelect={selectorOnSelectGroups}
            onCancel={onCloseGroupSelector}
            onArrowClick={onCloseGroupSelector}
            showCounter
            selectedOptions={selectorSelectedOptions}
          />
        )}
        {selectorSelectedOptions.map((option) => (
          <SelectedItem
            key={`department_${option.key}`}
            text={option.label}
            data-id={option.key}
            onClose={this.onRemoveGroup}
            isInline={true}
            className="department-item"
            isDisabled={isDisabled}
          />
        ))}
      </FieldContainer>
    );
  }
}

export default DepartmentField;
