import React from "react";
import { observer } from "mobx-react";

import FormStore from "@appserver/studio/src/store/SsoFormStore";
import TextInput from "@appserver/components/text-input";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";

const SimpleTextInput = ({
  hasError,
  isDisabled,
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
        isDisabled={isDisabled ?? !FormStore.enableSso}
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
