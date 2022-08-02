import React from "react";
import { observer } from "mobx-react";

import FieldContainer from "@docspace/components/field-container";

import SsoTextInput from "./SsoTextInput";

const MetadataUrlField = ({ labelText, name, placeholder, tooltipContent }) => {
  return (
    <FieldContainer
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      <SsoTextInput isDisabled name={name} placeholder={placeholder} />
    </FieldContainer>
  );
};

export default observer(MetadataUrlField);
