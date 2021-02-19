import React, { useState } from "react";

import { FieldContainer, TextInput } from "ASC.Web.Components";

const TextField = ({
  t,
  hasError,
  value,
  errorText,
  placeholder,
  id,
  type,
  isLoading,
  onChangeValue,
}) => {
  const [isValid, setIsValid] = useState(true);

  const onChangeHandler = (e) => {
    const { value } = e.target;
    const cleanVal = value.trim();
    let isValid = true;
    if (!isValid) {
      isValid = false;
    }
    setIsValid(isValid);
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
        isAutoFocussed={true}
        tabIndex={1}
        isDisabled={isLoading}
        autoComplete="username"
        onChange={onChangeHandler}
      />
    </FieldContainer>
  );
};

export default TextField;
