import React from "react";
import { observer } from "mobx-react";

import FieldContainer from "@appserver/components/field-container";

import SimpleTextInput from "./SimpleTextInput";

const SimpleFormField = ({
  FormStore,
  children,
  labelText,
  name,
  placeholder,
  t,
  tabIndex,
  tooltipContent,
}) => {
  return (
    <FieldContainer
      errorMessage={t(FormStore[`${name}ErrorMessage`])}
      hasError={FormStore[`${name}HasError`]}
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      {children}
      <SimpleTextInput
        FormStore={FormStore}
        hasError={FormStore[`${name}errorMessage`]}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
      />
    </FieldContainer>
  );
};

export default observer(SimpleFormField);
