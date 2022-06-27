import React from "react";
import { inject, observer } from "mobx-react";
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
  hasError,
}) => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <FieldContainer
      //errorMessage={t(FormStore[`${name}ErrorMessage`])}
      //hasError={FormStore[`${name}HasError`]}
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      {children}
      <SimpleTextInput
        //hasError={FormStore[`${name}errorMessage`]}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
        value={value}
      />
    </FieldContainer>
  );
};

export default inject(({ ssoStore }) => {
  const { onHideClick } = ssoStore;

  return {
    onHideClick,
  };
})(observer(SimpleFormField));
