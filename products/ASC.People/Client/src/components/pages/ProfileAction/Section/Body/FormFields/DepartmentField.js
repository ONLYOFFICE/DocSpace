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
      selectorSearchPlaceholder,
      selectorOptions,
      selectorSelectedOptions,
      selectorAddButtonText,
      selectorSelectAllText,
      selectorOnSearchGroups,
      selectorOnSelectGroups
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
        {/* <AdvancedSelector
          displayType="dropdown"
          isOpen={selectorIsVisible}
          maxHeight={336}
          width={379}
          placeholder={selectorSearchPlaceholder}
          onSearchChanged={selectorOnSearchGroups}
          options={selectorOptions}
          selectedOptions={selectorSelectedOptions}
          isMultiSelect={true}
          buttonLabel={selectorAddButtonText}
          selectAllLabel={selectorSelectAllText}
          onSelect={selectorOnSelectGroups}
          onCancel={onCloseGroupSelector}
        /> */}
        <GroupSelector 
          isOpen={selectorIsVisible}
          isMultiSelect={true}
          onSelect={selectorOnSelectGroups}
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
