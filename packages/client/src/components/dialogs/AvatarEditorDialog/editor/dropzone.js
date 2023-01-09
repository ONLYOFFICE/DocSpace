import React, { useEffect } from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import { useDropzone } from "react-dropzone";
import resizeImage from "resize-image";

import { Base } from "@docspace/components/themes";

import { hugeMobile } from "@docspace/components/utils/device";

const StyledDropzone = styled.div`
  cursor: pointer;
  box-sizing: border-box;
  width: 100%;
  height: 150px;
  border: 2px dashed
    ${(props) => props.theme.createEditRoomDialog.dropzone.borderColor};
  border-radius: 6px;

  .dropzone {
    height: 100%;
    width: 100%;

    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 4px;

    user-select: none;

    &-link {
      display: flex;
      flex-direction: row;
      gap: 4px;

      font-size: 13px;
      line-height: 20px;
      &-main {
        color: ${(props) =>
          props.theme.createEditRoomDialog.dropzone.linkMainColor};
        font-weight: 600;
        text-decoration: underline;
        text-decoration-style: dashed;
        text-underline-offset: 1px;
      }
      &-secondary {
        font-weight: 400;
        color: ${(props) =>
          props.theme.createEditRoomDialog.dropzone.linkSecondaryColor};
      }

      @media ${hugeMobile} {
        &-secondary {
          display: none;
        }
      }
    }

    &-exsts {
      font-weight: 600;
      font-size: 12px;
      line-height: 16px;
      color: ${(props) => props.theme.createEditRoomDialog.dropzone.exstsColor};
    }
  }
`;

StyledDropzone.defaultProps = { theme: Base };

const Dropzone = ({ setUploadedFile }) => {
  const { t } = useTranslation("CreateEditRoomDialog");

  const { acceptedFiles, getRootProps, getInputProps } = useDropzone({
    maxFiles: 1,
    // maxSize: 1000000,
    accept: ["image/png", "image/jpeg"],
  });

  useEffect(() => {
    if (acceptedFiles.length) {
      const fr = new FileReader();
      fr.readAsDataURL(acceptedFiles[0]);

      fr.onload = () => {
        const img = new Image();
        img.onload = () => {
          const canvas = resizeImage.resize2Canvas(img, img.width, img.height);

          const data = resizeImage.resize(
            canvas,
            img.width / 4,
            img.height / 4,
            resizeImage.JPEG
          );

          fetch(data)
            .then((res) => res.blob())
            .then((blob) => {
              const file = new File([blob], "File name", {
                type: "image/jpg",
              });
              setUploadedFile(file);
            });
        };
        img.src = fr.result;
      };
    }
  }, [acceptedFiles]);

  return (
    <StyledDropzone>
      <div {...getRootProps({ className: "dropzone" })}>
        <input {...getInputProps()} />
        <div className="dropzone-link">
          <span className="dropzone-link-main">{t("DropzoneTitleLink")}</span>
          <span className="dropzone-link-secondary">
            {t("DropzoneTitleSecondary")}
          </span>
        </div>
        <div className="dropzone-exsts">{t("DropzoneTitleExsts")}</div>
      </div>
    </StyledDropzone>
  );
};

export default Dropzone;
