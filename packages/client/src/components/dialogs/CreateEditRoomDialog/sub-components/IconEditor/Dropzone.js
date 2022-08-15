import React, { useEffect } from "react";
import styled from "styled-components";
import { useDropzone } from "react-dropzone";

const StyledDropzone = styled.div`
  cursor: pointer;
  width: 100%;
  height: 150px;
  border: 2px dashed #eceef1;
  border-radius: 6px;

  .dropzone {
    height: 100%;
    width: 100%;

    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 4px;

    &-link {
      display: flex;
      flex-direction: row;
      gap: 4px;

      font-size: 13px;
      line-height: 20px;
      &-main {
        color: #316daa;
        font-weight: 600;
        text-decoration: underline;
        text-decoration-style: dashed;
        text-underline-offset: 1px;
      }
      &-secondary {
        font-weight: 400;
      }
    }

    &-exsts {
      font-weight: 600;
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
    }
  }
`;

const Dropzone = ({ setUploadedFile }) => {
  const { acceptedFiles, getRootProps, getInputProps } = useDropzone({
    maxFiles: 1,
    maxSize: 1000000,
    // accept: {
    //   "image/*": [".jpeg", ".png"],
    // },
  });

  useEffect(() => {
    if (acceptedFiles.length) setUploadedFile(acceptedFiles[0]);
  }, [acceptedFiles]);

  return (
    <StyledDropzone>
      <div {...getRootProps({ className: "dropzone" })}>
        <input {...getInputProps()} />
        <div className="dropzone-link">
          <span className="dropzone-link-main">Select new image</span>
          <span className="dropzone-link-secondary">or drop file here</span>
        </div>
        <div className="dropzone-exsts">(JPG or PNG, max 1 MB)</div>
      </div>
    </StyledDropzone>
  );
};

export default Dropzone;
