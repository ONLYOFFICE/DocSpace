import React from 'react'
import { FieldContainer, SelectorAddButton, SelectedItem } from 'asc-web-components'

const DepartmentField = React.memo((props) => {
  const {
    isRequired,
    isDisabled,
    hasError,
    labelText,
    addButtonTitle,
    departments,
    onAddDepartment,
    onRemoveDepartment
  } = props;

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
      {departments && departments.map((department) => (
        <SelectedItem
          key={`department_${department.id}`}
          text={department.name}
          onClose={() => { onRemoveDepartment(department.id) }}
          isInline={true}
          className="department-item"
        />
      ))}
    </FieldContainer>
  );
});

export default DepartmentField