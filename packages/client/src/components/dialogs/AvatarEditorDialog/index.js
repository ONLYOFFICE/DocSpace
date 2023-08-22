import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";
import { loadAvatar, deleteAvatar } from "@docspace/common/api/people";
import { dataUrlToFile } from "@docspace/common/utils/dataUrlToFile";
import ImageEditor from "@docspace/components/ImageEditor";
import AvatarPreview from "@docspace/components/ImageEditor/AvatarPreview";
import DefaultUserAvatarMax from "PUBLIC_DIR/images/default_user_photo_size_200-200.png";

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

  const { visible, onClose, profile, updateCreatedAvatar, setHasAvatar } =
    props;
  const [avatar, setAvatar] = useState({
    uploadedFile: profile.hasAvatar ? profile.avatarMax : DefaultUserAvatarMax,
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
      const res = await deleteAvatar(profile.id);
      updateCreatedAvatar(res);
      setHasAvatar(false);
      onClose();
      return;
    }

    const file = await dataUrlToFile(preview);

    const avatarData = new FormData();
    avatarData.append("file", file);
    avatarData.append("Autosave", true);

    try {
      const res = await loadAvatar(profile.id, avatarData);

      if (res.success) {
        res.data && updateCreatedAvatar(res.data);
        setHasAvatar(true);
        toastr.success(t("Common:ChangesSavedSuccessfully"));
      } else {
        throw new Error(t("Common:ErrorInternalServer"));
      }

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
          className="save"
          key="AvatarEditorSaveBtn"
          label={t("Common:SaveButton")}
          size="normal"
          scale
          primary={true}
          onClick={onSaveClick}
          isLoading={isLoading}
        />
        <Button
          className="cancel-button"
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
    updateCreatedAvatar,
    setHasAvatar,
  } = targetUserStore;

  return {
    profile,
    setHasAvatar,
    updateCreatedAvatar,
  };
})(observer(AvatarEditorDialog));
