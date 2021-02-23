import React from "react";

import { FieldContainer, TextInput } from "ASC.Web.Components";

const TextField = ({
  t,
  hasError,
  value,
  isLoading,
  id,
  type,
  placeholder,
  onChangeValue,
  isAutoFocussed,
}) => {
  const onChangeHandler = (e) => {
    const { value } = e.target;
    const cleanVal = value.trim();
    let isValid = true;
    
    if (!cleanVal) 
      isValid = false;
    
    onChangeValue(cleanVal, isValid);
  };

  return (
    <FieldContainer
      isVertical={true}
      labelVisible={false}
      hasError={hasError}
      errorMessage={t("RequiredFieldMessage")} //TODO: Add wrong login server error
    >
      <TextInput
        id={id}
        name={id}
        type={type ? type : "text"}
        hasError={hasError}
        value={value}
        placeholder={t(`${placeholder}`)}
        size="large"
        scale={true}
        isAutoFocussed={isAutoFocussed}
        tabIndex={1}
        isDisabled={isLoading}
        autoComplete="username"
        onChange={onChangeHandler}
      />
    </FieldContainer>
  );
};

export default TextField;
