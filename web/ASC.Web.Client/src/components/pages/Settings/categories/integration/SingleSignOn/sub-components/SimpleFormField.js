import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FieldContainer from "@appserver/components/field-container";

import SimpleTextInput from "./SimpleTextInput";

const SimpleFormField = ({
  children,
  labelText,
  name,
  placeholder,
  tabIndex,
  tooltipContent,
  value,
  errorMessage,
}) => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <FieldContainer
      errorMessage={t(errorMessage)}
      hasError={errorMessage !== null}
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      {children}
      <SimpleTextInput
        hasError={errorMessage !== null}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
        value={value}
      />
    </FieldContainer>
  );
};

export default observer(SimpleFormField);
