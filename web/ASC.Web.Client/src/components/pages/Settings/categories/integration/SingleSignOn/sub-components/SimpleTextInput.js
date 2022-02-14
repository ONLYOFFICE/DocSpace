import React from "react";
import { observer } from "mobx-react";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";
import TextInput from "@appserver/components/text-input";

const SimpleTextInput = ({
  FormStore,
  hasError,
  maxWidth,
  name,
  placeholder,
  tabIndex,
}) => {
  return (
    <StyledInputWrapper maxWidth={maxWidth}>
      <TextInput
        className="field-input"
        hasError={hasError}
        name={name}
        onBlur={FormStore.onBlur}
        onChange={FormStore.onTextInputChange}
        placeholder={placeholder}
        scale
        tabIndex={tabIndex}
        value={FormStore[name]}
      />
    </StyledInputWrapper>
  );
};

export default observer(SimpleTextInput);
