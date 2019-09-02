import React from 'react'
import { FieldContainer, SelectedItem } from 'asc-web-components'

const DepartmentField = React.memo((props) => {
  const {
    isRequired,
    hasError,
    labelText,
    
    departments,
    onRemoveDepartment
  } = props;

  return (
    <FieldContainer
      isRequired={isRequired}
      hasError={hasError}
      labelText={labelText}
    >
      {departments && departments.map((department) => (      
        <SelectedItem
          key={`department_${department.id}`}    
          text={department.name}
          onClose={() => { onRemoveDepartment(department.id) }}
          isInline={true}
          className={"department-item"}
        />
      ))}
    </FieldContainer>
  );
});

export default DepartmentField