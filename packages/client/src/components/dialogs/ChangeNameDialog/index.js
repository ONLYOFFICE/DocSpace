import React, { useState } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import ModalDialogContainer from "../ModalDialogContainer";

const ChangeNameDialog = (props) => {
  const { t, tReady, visible, onClose, profile, onSave } = props;
  const [firstName, setFirstName] = useState(profile.firstName);
  const [lastName, setLastName] = useState(profile.lastName);
  const [isSaving, setIsSaving] = useState(false);

  const onSaveClick = async () => {
    const newProfile = profile;
    newProfile.firstName = firstName;
    newProfile.lastName = lastName;

    try {
      setIsSaving(true);
      await onSave(newProfile);
      toastr.success(t("ProfileAction:ChangesSavedSuccessfully"));
    } catch (error) {
      console.error(error);
      toastr.error(error);
    } finally {
      setIsSaving(false);
      onClose();
    }
  };

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
          onClick={onSaveClick}
          isLoading={isSaving}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={isSaving}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default ChangeNameDialog;
