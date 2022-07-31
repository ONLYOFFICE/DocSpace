import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FieldContainer from "@appserver/components/field-container";

import SsoTextInput from "./SsoTextInput";

const SsoFormField = ({
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
      <SsoTextInput
        hasError={errorMessage !== null}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
        value={value}
      />
    </FieldContainer>
  );
};

export default observer(SsoFormField);
