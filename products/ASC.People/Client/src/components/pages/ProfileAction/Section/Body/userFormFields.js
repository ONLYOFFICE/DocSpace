import React from 'react'
import styled from 'styled-components';
import { device, FieldContainer, TextInput, DateInput, RadioButtonGroup, SelectedItem } from 'asc-web-components'

const MainContainer = styled.div`
  display: flex;
  flex-direction: row;

  @media ${device.tablet} {
    flex-direction: column;
  }
`;

const AvatarContainer = styled.div`
  margin: 0 32px 32px 0;
  width: 160px;
`;

const MainFieldsContainer = styled.div`
  flex-grow: 1;
`;

const TextField = React.memo((props) => {
  const {isRequired, hasError, labelText, inputName, inputValue, isDisabled, onChange} = props;
  return (
    <FieldContainer isRequired={isRequired} hasError={hasError} labelText={labelText}>
      <TextInput name={inputName} value={inputValue} isDisabled={isDisabled} hasError={hasError} onChange={onChange} className="field-input"/>
    </FieldContainer>
  );
});

const PasswordField = React.memo((props) => {
  const {isRequired, hasError, labelText, radioName, radioValue, radioOptions, radioIsDisabled, radioOnChange, inputName, inputValue, inputIsDisabled, inputOnChange} = props;
  return (
    <FieldContainer isRequired={isRequired} hasError={hasError} labelText={labelText}>
      <RadioButtonGroup name={radioName} selected={radioValue} options={radioOptions} isDisabled={radioIsDisabled} onClick={radioOnChange} className="radio-group"/>
      <TextInput name={inputName} type="password" value={inputValue} isDisabled={inputIsDisabled} hasError={hasError} onChange={inputOnChange} className="field-input"/>
    </FieldContainer>
  );
});

const DateField = React.memo((props) => {
  const {isRequired, hasError, labelText, inputName, inputValue, inputIsDisabled, inputOnChange} = props;
  return (
    <FieldContainer isRequired={isRequired} hasError={hasError} labelText={labelText}>
      <DateInput name={inputName} selected={inputValue} disabled={inputIsDisabled} hasError={hasError} onChange={inputOnChange} className="field-input"/>
    </FieldContainer>
  );
});

const RadioField = React.memo((props) => {
  const {isRequired, hasError, labelText, radioName, radioValue, radioOptions, radioIsDisabled, radioOnChange} = props;
  return (
    <FieldContainer isRequired={isRequired} hasError={hasError} labelText={labelText}>
      <RadioButtonGroup name={radioName} selected={radioValue} options={radioOptions} isDisabled={radioIsDisabled} onClick={radioOnChange} className="radio-group"/>
    </FieldContainer>
  );
});

const DepartmentField = React.memo((props) => {
  const {isRequired, hasError, labelText, departments, onClose} = props;
  return (
    departments && departments.length
    ? <FieldContainer isRequired={isRequired} hasError={hasError} labelText={labelText}>
      {departments.map((department, index) => (      
        <SelectedItem
          key={`user_group_${department.id}`}    
          text={department.name}
          onClose={() => { onClose(department.id) }}
          isInline={true}
          style={{ marginRight: "8px", marginBottom: "8px" }}
        />
      ))}
    </FieldContainer>
    : ""
  );
});

export { MainContainer, AvatarContainer, MainFieldsContainer, TextField, PasswordField, DateField, RadioField, DepartmentField }