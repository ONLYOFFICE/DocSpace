import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import AvatarEditor from "./editor";

const StyledModalDialog = styled(ModalDialog)``;

const AvatarEditorDialog = (props) => {
  const { t } = useTranslation(["Profile", "Common"]);
  const { visible, onClose, profile } = props;
  const [avatar, setAvatar] = useState({
    uploadedFile: profile.avatarMax,
    x: 0.5,
    y: 0.5,
    zoom: 1,
  });
  const [preview, setPreview] = useState(null);

  const onChangeAvatar = (newAvatar) => setAvatar(newAvatar);

  const onSaveClick = () => {
    console.log("onSave");
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
          preview={preview}
          setPreview={setPreview}
          profile={profile}
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

export default AvatarEditorDialog;
