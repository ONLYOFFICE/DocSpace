import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import ComboBox from "@docspace/components/combobox";
import FieldContainer from "@docspace/components/field-container";

const ModalComboBox = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const { isDisabled, spAction, setComboBoxOption, className } = props;

  const certificateOptions = [
    { key: "signing", label: t("Signing") },
    { key: "encrypt", label: t("Encryption") },
    { key: "signing and encrypt", label: t("SigningAndEncryption") },
  ];

  const currentOption = certificateOptions.find(
    (option) => option.key === spAction
  );

  return (
    <FieldContainer isVertical labelText={t("UsedFor")} className={className}>
      <ComboBox
        onSelect={setComboBoxOption}
        options={certificateOptions}
        scaled
        scaledOptions
        selectedOption={currentOption}
        showDisabledItems
        isDisabled={isDisabled}
        directionY="both"
      />
    </FieldContainer>
  );
};

export default inject(({ ssoStore }) => {
  const { spAction, setComboBoxOption } = ssoStore;

  return { spAction, setComboBoxOption };
})(observer(ModalComboBox));
