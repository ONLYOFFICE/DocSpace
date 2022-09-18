import React, { useState } from "react";
import styled from "styled-components";

import AvatarCropper from "./avatar-cropper";
import AvatarPreview from "./preview";
import Dropzone from "./dropzone";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 24px;

  .avatar-editor {
    display: flex;
    gap: 16px;
    align-items: center;
  }
`;

const AvatarEditor = ({ t, profile, avatar, onChangeAvatar }) => {
  const [preview, setPreview] = useState(null);

  const setUploadedFile = (file) =>
    onChangeAvatar({ ...avatar, uploadedFile: file });

  const isDefaultAvatar =
    typeof avatar.uploadedFile === "string" &&
    avatar.uploadedFile.includes("default_user_photo");

  return (
    <StyledWrapper>
      {avatar.uploadedFile && !isDefaultAvatar && (
        <div className="avatar-editor">
          <AvatarCropper
            t={t}
            avatar={avatar}
            onChangeAvatar={onChangeAvatar}
            uploadedFile={avatar.uploadedFile}
            setUploadedFile={setUploadedFile}
            setPreviewAvatar={setPreview}
          />
          <AvatarPreview avatar={preview} userName={profile.displayName} />
        </div>
      )}
      <Dropzone t={t} setUploadedFile={setUploadedFile} />
    </StyledWrapper>
  );
};

export default AvatarEditor;
