import React, { useState } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";

import ModalDialogContainer from "../ModalDialogContainer";

const ChangeNameDialog = (props) => {
  const { t, tReady, visible, onClose, fName, sName } = props;
  const [firstName, setFirstName] = useState(fName);
  const [lastName, setLastName] = useState(sName);

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("PeopleTranslations:NameChangeButton")}
      </ModalDialog.Header>
      <ModalDialog.Body className="change-name-dialog-body">
        <FieldContainer
          isVertical
          labelText={t("ProfileAction:FirstName")}
          className="field"
        >
          <TextInput
            scale={true}
            isAutoFocussed={true}
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            placeholder={t("ProfileAction:FirstName")}
          />
        </FieldContainer>

        <FieldContainer
          isVertical
          labelText={t("Common:LastName")}
          className="field"
        >
          <TextInput
            scale={true}
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            placeholder={t("Common:LastName")}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="ChangeNameSaveBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary={true}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default ChangeNameDialog;
