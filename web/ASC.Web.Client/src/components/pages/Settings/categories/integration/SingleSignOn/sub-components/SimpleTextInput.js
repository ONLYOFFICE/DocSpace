import React from "react";
import StyledInputWrapper from "../styled-containers/StyledInputWrapper";
import TextInput from "@appserver/components/text-input";
import { observer } from "mobx-react";

const SimpleTextInput = ({
  FormStore,
  maxWidth,
  name,
  placeholder,
  tabIndex,
}) => {
  return (
    <StyledInputWrapper maxWidth={maxWidth}>
      <TextInput
        className="field-input"
        name={name}
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
