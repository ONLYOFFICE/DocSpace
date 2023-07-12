import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import ModalDialogContainer from "../ModalDialogContainer";

const ChangeNameDialog = (props) => {
  const { t, ready } = useTranslation([
    "ProfileAction",
    "PeopleTranslations",
    "Common",
  ]);
  const {
    visible,
    onClose,
    profile,
    updateProfile,
    updateProfileInUsers,
    fromList,
  } = props;
  const [firstName, setFirstName] = useState(profile.firstName);
  const [lastName, setLastName] = useState(profile.lastName);
  const [isSaving, setIsSaving] = useState(false);

  const onCloseAction = () => {
    if (!isSaving) {
      onClose();
    }
  };

  const onKeyDown = (e) => {
    if (e.keyCode === 13 || e.which === 13) onSaveClick();
  };

  const onSaveClick = async () => {
    const newProfile = profile;
    newProfile.firstName = firstName;
    newProfile.lastName = lastName;

    try {
      setIsSaving(true);
      const currentProfile = await updateProfile(newProfile);
      fromList && (await updateProfileInUsers(currentProfile));
      toastr.success(t("Common:ChangesSavedSuccessfully"));
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
      isLoading={!ready}
      visible={visible}
      onClose={onCloseAction}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("PeopleTranslations:NameChangeButton")}
      </ModalDialog.Header>
      <ModalDialog.Body className="change-name-dialog-body">
        <FieldContainer
          isVertical
          labelText={t("Common:FirstName")}
          className="field"
        >
          <TextInput
            className="first-name"
            scale={true}
            isAutoFocussed={true}
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            placeholder={t("Common:FirstName")}
            isDisabled={isSaving}
            onKeyDown={onKeyDown}
            tabIndex={1}
          />
        </FieldContainer>

        <FieldContainer
          isVertical
          labelText={t("Common:LastName")}
          className="field"
        >
          <TextInput
            className="last-name"
            scale={true}
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            placeholder={t("Common:LastName")}
            isDisabled={isSaving}
            onKeyDown={onKeyDown}
            tabIndex={2}
          />
        </FieldContainer>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="save"
          key="ChangeNameSaveBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary={true}
          onClick={onSaveClick}
          isLoading={isSaving}
          tabIndex={3}
        />
        <Button
          className="cancel-button"
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={isSaving}
          tabIndex={4}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ peopleStore }) => {
  const { updateProfile } = peopleStore.targetUserStore;

  const { updateProfileInUsers } = peopleStore.usersStore;

  return { updateProfile, updateProfileInUsers };
})(observer(ChangeNameDialog));
