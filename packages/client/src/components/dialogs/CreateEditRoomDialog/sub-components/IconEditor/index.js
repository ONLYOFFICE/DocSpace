import React, { useState } from "react";
import styled from "styled-components";

import Dropzone from "./Dropzone";

const StyledIconEditor = styled.div`
  .icon-editor {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    justify-content: start;
    gap: 16px;
  }
`;

import IconCropper from "./IconCropper";
import PreviewTile from "./PreviewTile";

const IconEditor = ({
  t,
  isEdit,
  title,
  tags,
  currentRoomTypeData,
  icon,
  onChangeIcon,
}) => {
  const [previewIcon, setPreviewIcon] = useState(null);

  const uploadedFile = icon.uploadedFile;
  const setUploadedFile = (uploadedFile) =>
    onChangeIcon({ ...icon, uploadedFile });

  return (
    <StyledIconEditor>
      {uploadedFile && (
        <div className="icon-editor">
          <IconCropper
            t={t}
            icon={icon}
            onChangeIcon={onChangeIcon}
            uploadedFile={uploadedFile}
            setUploadedFile={setUploadedFile}
            setPreviewIcon={setPreviewIcon}
          />

          <PreviewTile
            t={t}
            title={title || t("Files:NewRoom")}
            previewIcon={previewIcon}
            tags={tags.map((tag) => tag.name)}
            defaultTagLabel={t(currentRoomTypeData.defaultTag)}
          />
        </div>
      )}
      <Dropzone setUploadedFile={setUploadedFile} />
    </StyledIconEditor>
  );
};

export default IconEditor;
