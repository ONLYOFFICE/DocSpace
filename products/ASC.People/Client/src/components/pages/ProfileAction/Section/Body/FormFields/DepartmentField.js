import React from "react";
import isEqual from "lodash/isEqual";
import {
  FieldContainer,
  SelectorAddButton,
  SelectedItem
} from "asc-web-components";
import { GroupSelector } from "asc-web-common";

class DepartmentField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    console.log("DepartmentField render");

    const {
      isRequired,
      isDisabled,
      hasError,
      labelText,

      showGroupSelectorButtonTitle,

      onShowGroupSelector,
      onCloseGroupSelector,
      onRemoveGroup,

      selectorIsVisible,
      selectorSelectedOptions,
      selectorOnSelectGroups,
      searchPlaceHolderLabel
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        className="departments-field"
      >
        <SelectorAddButton
          isDisabled={isDisabled}
          title={showGroupSelectorButtonTitle}
          onClick={onShowGroupSelector}
          className="department-add-btn"
        />
        <GroupSelector 
          isOpen={selectorIsVisible}
          isMultiSelect={true}
          onSelect={selectorOnSelectGroups}
          onCancel={onCloseGroupSelector}
          searchPlaceHolderLabel={searchPlaceHolderLabel}
        />
        {selectorSelectedOptions.map(option => (
            <SelectedItem
              key={`department_${option.key}`}
              text={option.label}
              onClose={() => {
                onRemoveGroup(option.key);
              }}
              isInline={true}
              className="department-item"
            />
          ))}
      </FieldContainer>
    );
  }
}

export default DepartmentField;
