import React from "react";
import { observer } from "mobx-react";

import FieldContainer from "@appserver/components/field-container";

import SimpleTextInput from "./SimpleTextInput";

const MetadataUrlField = ({ labelText, name, placeholder, tooltipContent }) => {
  return (
    <FieldContainer
      inlineHelpButton
      isVertical
      labelText={labelText}
      place="top"
      tooltipContent={tooltipContent}
    >
      <SimpleTextInput isDisabled name={name} placeholder={placeholder} />
    </FieldContainer>
  );
};

export default observer(MetadataUrlField);
