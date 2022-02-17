import React from "react";
import { observer } from "mobx-react";

import ComboBox from "@appserver/components/combobox";
import FieldContainer from "@appserver/components/field-container";

import StyledInputWrapper from "../styled-containers/StyledInputWrapper";

const ModalComboBox = ({ FormStore, t }) => {
  const certificateOptions = [
    { key: "signing", label: t("Signing") },
    { key: "encryption", label: t("Encryption") },
    { key: "signingAndEncryption", label: t("SigningAndEncryption") },
  ];

  const currentOption = certificateOptions.find(
    (option) => option.key === FormStore.newSpCertificateUsedFor
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
