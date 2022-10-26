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
  isDisabled,
}) => {
  const [previewIcon, setPreviewIcon] = useState(null);

  const setUploadedFile = (uploadedFile) =>
    onChangeIcon({ ...icon, uploadedFile });

  return (
    <StyledIconEditor>
      {icon.uploadedFile && (
        <div className="icon-editor">
          <IconCropper
            t={t}
            icon={icon}
            onChangeIcon={onChangeIcon}
            uploadedFile={icon.uploadedFile}
            setUploadedFile={setUploadedFile}
            setPreviewIcon={setPreviewIcon}
            isDisabled={isDisabled}
          />

          <PreviewTile
            t={t}
            title={title || t("Files:NewRoom")}
            previewIcon={previewIcon}
            tags={tags.map((tag) => tag.name)}
            defaultTagLabel={t(currentRoomTypeData.defaultTag)}
            isDisabled={isDisabled}
          />
        </div>
      )}
      <Dropzone
        t={t}
        setUploadedFile={setUploadedFile}
        isDisabled={isDisabled}
      />
    </StyledIconEditor>
  );
};

export default IconEditor;
