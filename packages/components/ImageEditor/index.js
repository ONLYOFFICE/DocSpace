import React from "react";
import Dropzone from "./Dropzone";
import ImageCropper from "./ImageCropper";

const ImageEditor = ({
  t,
  image,
  onChangeImage,
  Preview,
  setPreview,
  isDisabled,
  classNameWrapperImageCropper,
  className,
}) => {
  const setUploadedFile = (uploadedFile) =>
    onChangeImage({ ...image, uploadedFile });

  const isDefaultAvatar =
    typeof image.uploadedFile === "string" &&
    image.uploadedFile.includes("default_user_photo");

  return (
    <div className={className}>
      {image.uploadedFile && !isDefaultAvatar && (
        <div className={classNameWrapperImageCropper}>
          <ImageCropper
            t={t}
            image={image}
            onChangeImage={onChangeImage}
            uploadedFile={image.uploadedFile}
            setUploadedFile={setUploadedFile}
            setPreviewImage={setPreview}
            isDisabled={isDisabled}
          />
          {Preview}
        </div>
      )}
      <Dropzone
        t={t}
        setUploadedFile={setUploadedFile}
        isDisabled={isDisabled}
      />
    </div>
  );
};

export default ImageEditor;
