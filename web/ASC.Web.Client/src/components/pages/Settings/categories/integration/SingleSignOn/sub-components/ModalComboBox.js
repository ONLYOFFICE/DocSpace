import React from "react";
import { observer } from "mobx-react";

import ComboBox from "@appserver/components/combobox";
import FieldContainer from "@appserver/components/field-container";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";

const ModalComboBox = ({ t }) => {
  const certificateOptions = [
    { key: "signing", label: t("Signing") },
    { key: "encrypt", label: t("Encryption") },
    { key: "signing and encrypt", label: t("SigningAndEncryption") },
  ];

  const currentOption = certificateOptions.find(
    (option) => option.key === FormStore.sp_action
  );

  return (
    <FieldContainer isVertical labelText={t("UsedFor")}>
      <StyledInputWrapper>
        <ComboBox
          onSelect={FormStore.onModalComboBoxChange}
          options={certificateOptions}
          scaled
          scaledOptions
          selectedOption={currentOption}
          showDisabledItems
        />
      </StyledInputWrapper>
    </FieldContainer>
  );
};

export default observer(ModalComboBox);
