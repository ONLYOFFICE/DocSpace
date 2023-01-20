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
import ImageEditor from "@docspace/components/ImageEditor";
import AvatarPreview from "@docspace/components/ImageEditor/AvatarPreview";

const StyledModalDialog = styled(ModalDialog)`
  .wrapper-image-editor {
    width: 100%;
    display: flex;
    flex-direction: column;
    gap: 24px;
    .avatar-editor {
      display: flex;
      gap: 16px;
      align-items: center;
    }
  }
`;

const AvatarEditorDialog = (props) => {
  const { t } = useTranslation([
    "Profile",
    "PeopleTranslations",
    "ProfileAction",
    "Common",
    "CreateEditRoomDialog",
  ]);

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

      toastr.success(t("Common:ChangesSavedSuccessfully"));
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
        <ImageEditor
          t={t}
          className="wrapper-image-editor"
          classNameWrapperImageCropper="avatar-editor"
          image={avatar}
          setPreview={setPreview}
          onChangeImage={onChangeAvatar}
          Preview={
            <AvatarPreview avatar={preview} userName={profile.displayName} />
          }
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
