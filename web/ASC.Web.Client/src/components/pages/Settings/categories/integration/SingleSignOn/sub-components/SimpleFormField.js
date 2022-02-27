import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FieldContainer from "@appserver/components/field-container";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

import SimpleTextInput from "./SimpleTextInput";

const SimpleFormField = ({
  children,
  labelText,
  name,
  placeholder,
  tabIndex,
  tooltipContent,
}) => {
  const { t } = useTranslation("SingleSignOn");

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
        hasError={FormStore[`${name}errorMessage`]}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
      />
    </FieldContainer>
  );
};

export default observer(SimpleFormField);
