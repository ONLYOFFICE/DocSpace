import React, { useState, useRef, useEffect } from "react";

import { useDropzone } from "react-dropzone";
import resizeImage from "resize-image";

import Loader from "../../loader";

import { toastr } from "../../";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import StyledDropzone from "./StyledDropzone";

const ONE_MEGABYTE = 1024 * 1024;
const COMPRESSION_RATIO = 2;
const NO_COMPRESSION_RATIO = 1;

const Dropzone = ({ t, setUploadedFile, isDisabled }) => {
  const [loadingFile, setLoadingFile] = useState(false);
  const mount = useRef(false);
  const timer = useRef(null);

  useEffect(() => {
    mount.current = true;
    return () => {
      mount.current = false;
      timer.current && clearTimeout(timer.current);
    };
  }, []);

  async function resizeRecursiveAsync(
    img,
    canvas,
    compressionRatio = COMPRESSION_RATIO,
    depth = 0
  ) {
    const data = resizeImage.resize(
      canvas,
      img.width / compressionRatio,
      img.height / compressionRatio,
      resizeImage.JPEG
    );

    const file = await fetch(data)
      .then((res) => res.blob())
      .then((blob) => {
        const file = new File([blob], "File name", {
          type: "image/jpg",
        });
        return file;
      });

    const stepMessage = `Step ${depth + 1}`;
    const sizeMessage = `size = ${file.size} bytes`;
    const compressionRatioMessage = `compressionRatio = ${compressionRatio}`;

    console.log(`${stepMessage} ${sizeMessage} ${compressionRatioMessage}`);

    if (file.size < ONE_MEGABYTE) {
      return file;
    }

    if (depth > 5) {
      console.log("start");
      throw new Error("recursion depth exceeded");
    }

    return await resizeRecursiveAsync(
      img,
      canvas,
      compressionRatio + 1,
      depth + 1
    );
  }

  const onDrop = async ([file]) => {
    timer.current = setTimeout(() => {
      setLoadingFile(true);
    }, 50);
    const imageBitMap = await createImageBitmap(file);

    const width = imageBitMap.width;
    const height = imageBitMap.height;
    const canvas = resizeImage.resize2Canvas(imageBitMap, width, height);

    resizeRecursiveAsync(
      { width, height },
      canvas,
      file.size > ONE_MEGABYTE ? COMPRESSION_RATIO : NO_COMPRESSION_RATIO
    )
      .then((file) => {
        if (mount.current) {
          setUploadedFile(file);
        }
      })
      .catch((error) => {
        if (
          error instanceof Error &&
          error.message === "recursion depth exceeded"
        ) {
          toastr.error(t("Common:SizeImageLarge"));
        }
        console.error(error);
      })
      .finally(() => {
        timer.current && clearTimeout(timer.current);
        if (mount.current) {
          setLoadingFile(false);
        }
      });
  };

  const { getRootProps, getInputProps } = useDropzone({
    maxFiles: 0,
    noClick: isDisabled,
    noKeyboard: isDisabled,
    // maxSize: 1000000,
    accept: ["image/png", "image/jpeg"],
    onDrop,
  });

  return (
    <StyledDropzone $isLoading={loadingFile}>
      {loadingFile && (
        <Loader className="dropzone_loader" size="30px" type="track" />
      )}
      <div {...getRootProps({ className: "dropzone" })}>
        <input {...getInputProps()} />
        <div className="dropzone-link">
          <ColorTheme className="dropzone-link-main" themeId={ThemeType.Link}>
            {t("Common:DropzoneTitleLink")}
          </ColorTheme>
          <span className="dropzone-link-secondary">
            {t("Common:DropzoneTitleSecondary")}
          </span>
        </div>
        <div className="dropzone-exsts">{t("Common:DropzoneTitleExsts")}</div>
      </div>
    </StyledDropzone>
  );
};

export default Dropzone;
