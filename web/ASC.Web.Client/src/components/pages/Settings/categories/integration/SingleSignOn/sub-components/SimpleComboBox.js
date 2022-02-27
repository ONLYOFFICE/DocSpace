import React from "react";
import { observer } from "mobx-react";

import ComboBox from "@appserver/components/combobox";
import FieldContainer from "@appserver/components/field-container";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";
import { addArguments } from "../../../../utils/addArguments";

const SimpleComboBox = ({ labelText, name, options, tabIndex }) => {
  const currentOption = options.find(
    (option) => option.key === FormStore[name]
  );

  const onSelect = addArguments(FormStore.onComboBoxChange, name);

  return (
    <FieldContainer isVertical labelText={labelText}>
      <StyledInputWrapper>
        <ComboBox
          isDisabled={!FormStore.enableSso}
          onSelect={onSelect}
          options={options}
          scaled
          scaledOptions
          selectedOption={currentOption}
          showDisabledItems
          tabIndex={tabIndex}
        />
      </StyledInputWrapper>
    </FieldContainer>
  );
};

export default observer(SimpleComboBox);
