import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FieldContainer from "@docspace/components/field-container";

import SsoTextInput from "./SsoTextInput";

const SsoFormField = ({
  children,
  labelText,
  name,
  placeholder,
  tabIndex,
  tooltipContent,
  value,
  hasError,
  tooltipClass,
}) => {
  const { t } = useTranslation("Common");

  return (
    <FieldContainer
      errorMessage={t("Common:EmptyFieldError")}
      hasError={hasError}
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
      tooltipClass={tooltipClass}
    >
      {children}
      <SsoTextInput
        hasError={hasError}
        name={name}
        placeholder={placeholder}
        tabIndex={tabIndex}
        value={value}
      />
    </FieldContainer>
  );
};

export default observer(SsoFormField);
