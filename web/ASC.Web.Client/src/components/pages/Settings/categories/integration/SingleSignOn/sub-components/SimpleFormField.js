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
  tabIndex,
  tooltipContent,
}) => {
  return (
    <FieldContainer
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      {children}
      <SimpleTextInput
        FormStore={FormStore}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
      />
    </FieldContainer>
  );
};

export default observer(SimpleFormField);
