import React from "react";
import ComboBox from "@appserver/components/combobox";
import FieldContainer from "@appserver/components/field-container";
import StyledInputWrapper from "../styled-containers/StyledInputWrapper";
import { addParameter } from "../../../../utils";
import { observer } from "mobx-react";

const SimpleComboBox = ({ FormStore, labelText, name, options, tabIndex }) => {
  const currentOption = options.find(
    (option) => option.key === FormStore[name]
  );

  const onSelect = addParameter(FormStore.onComboBoxChange, name);

  return (
    <FieldContainer isVertical labelText={labelText}>
      <StyledInputWrapper>
        <ComboBox
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
