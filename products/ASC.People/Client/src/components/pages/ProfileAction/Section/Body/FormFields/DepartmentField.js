import React from "react";
import isEqual from "lodash/isEqual";
import {
  FieldContainer,
  SelectorAddButton,
  SelectedItem
} from "asc-web-components";

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
      addButtonTitle,
      departments,
      onAddDepartment,
      onRemoveDepartment
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
          title={addButtonTitle}
          onClick={onAddDepartment}
          className="department-add-btn"
        />
        {departments.map(department => (
            <SelectedItem
              key={`department_${department.id}`}
              text={department.name}
              onClose={() => {
                onRemoveDepartment(department.id);
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
