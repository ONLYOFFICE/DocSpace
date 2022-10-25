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
  title,
  tags,
  defaultTag,
  isPrivate,
  icon,
  onChangeIcon,
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
          />

          <PreviewTile
            t={t}
            title={title || t("Files:NewRoom")}
            isPrivate={isPrivate}
            previewIcon={previewIcon}
            tags={tags.map((tag) => tag.name)}
            defaultTagLabel={t(defaultTag)}
          />
        </div>
      )}
      <Dropzone t={t} setUploadedFile={setUploadedFile} />
    </StyledIconEditor>
  );
};

export default IconEditor;
