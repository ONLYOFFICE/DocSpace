import React from "react";
import { inject, observer } from "mobx-react";

import ComboBox from "@docspace/components/combobox";
import FieldContainer from "@docspace/components/field-container";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";

const SsoComboBox = (props) => {
  const {
    labelText,
    name,
    options,
    tabIndex,
    value,
    setComboBox,
    enableSso,
    isLoadingXml,
    isDisabled,
  } = props;

  const currentOption =
    options.find((option) => option.key === value) || options[0];

  const onSelect = () => {
    setComboBox(currentOption, name);
  };

  return (
    <FieldContainer isVertical labelText={labelText}>
      <StyledInputWrapper>
        <ComboBox
          id={name}
          isDisabled={!enableSso || isLoadingXml || isDisabled}
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

export default inject(({ ssoStore }) => {
  const { setComboBox, enableSso, isLoadingXml } = ssoStore;

  return {
    setComboBox,
    enableSso,
    isLoadingXml,
  };
})(observer(SsoComboBox));
