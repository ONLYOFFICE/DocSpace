import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import {
  loadAvatar,
  createThumbnailsAvatar,
  deleteAvatar,
} from "@docspace/common/api/people";
import { dataUrlToFile } from "@docspace/common/utils/dataUrlToFile";
import AvatarEditor from "./editor";

const StyledModalDialog = styled(ModalDialog)``;

const AvatarEditorDialog = (props) => {
  const { t } = useTranslation(["Profile", "ProfileAction", "Common"]);
  const {
    visible,
    onClose,
    profile,
    updateProfile,
    updateCreatedAvatar,
  } = props;
  const [avatar, setAvatar] = useState({
    uploadedFile: profile.avatarMax,
    x: 0.5,
    y: 0.5,
    zoom: 1,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [preview, setPreview] = useState(null);

  const onChangeAvatar = (newAvatar) => setAvatar(newAvatar);

  const onSaveClick = async () => {
    setIsLoading(true);

    if (!avatar.uploadedFile) {
      const avatars = await deleteAvatar(profile.id);
      updateCreatedAvatar(avatars);
      updateProfile(profile);
      onClose();
      return;
    }

    const file = await dataUrlToFile(preview);
    let avatarData = new FormData();
    avatarData.append("file", file);
    avatarData.append("Autosave", false);

    try {
      const res = await loadAvatar(profile.id, avatarData);
      const avatars = await createThumbnailsAvatar(profile.id, {
        x: 0,
        y: 0,
        width: 192,
        height: 192,
        tmpFile: res.data,
      });
      updateCreatedAvatar(avatars);
      updateProfile(profile);

      toastr.success(t("ProfileAction:ChangesSavedSuccessfully"));
      onClose();
    } catch (error) {
      console.error(error);
      toastr.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <StyledModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      withFooterBorder
    >
      <ModalDialog.Header>
        <Text fontSize="21px" fontWeight={700}>
          {t("EditPhoto")}
        </Text>
      </ModalDialog.Header>
      <ModalDialog.Body>
        <AvatarEditor
          t={t}
          avatar={avatar}
          onChangeAvatar={onChangeAvatar}
          profile={profile}
          preview={preview}
          setPreview={setPreview}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="AvatarEditorSaveBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary={true}
          onClick={onSaveClick}
          isLoading={isLoading}
        />
        <Button
          key="AvatarEditorCloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const {
    targetUser: profile,
    updateProfile,
    updateCreatedAvatar,
  } = targetUserStore;

  return {
    profile,
    updateProfile,
    updateCreatedAvatar,
  };
})(observer(AvatarEditorDialog));
