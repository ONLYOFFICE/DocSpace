import React, { useEffect } from "react";
import { ReactSVG } from "react-svg";
import throttle from "lodash/throttle";
import AvatarEditor from "react-avatar-editor";

import Slider from "../../slider";
import IconButton from "../../icon-button";
import StyledImageCropper from "./StyledImageCropper";

const ImageCropper = ({
  t,
  image,
  onChangeImage,
  uploadedFile,
  setUploadedFile,
  setPreviewImage,
  isDisabled,
}) => {
  let editorRef = null;
  const setEditorRef = (editor) => (editorRef = editor);

  const handlePositionChange = (position) => {
    if (isDisabled) return;

    onChangeImage({ ...image, x: position.x, y: position.y });
  };

  const handleSliderChange = (e, newZoom = null) => {
    if (isDisabled) return;

    onChangeImage({ ...image, zoom: newZoom ? newZoom : +e.target.value });
  };

  const handleZoomInClick = () => {
    if (isDisabled) return;

    handleSliderChange({}, image.zoom <= 4.5 ? image.zoom + 0.5 : 5);
  };

  const handleZoomOutClick = () => {
    if (isDisabled) return;

    handleSliderChange({}, image.zoom >= 1.5 ? image.zoom - 0.5 : 1);
  };

  const handleDeleteImage = () => {
    if (isDisabled) return;
    setUploadedFile(null);
  };

  const handleImageChange = throttle(() => {
    try {
      if (!editorRef) return;
      const newPreveiwImage = editorRef.getImageScaledToCanvas()?.toDataURL();
      setPreviewImage(newPreveiwImage);
    } catch (e) {
      console.error(e);
    }
  }, 300);

  useEffect(() => {
    handleImageChange();
    return () => {
      setPreviewImage("");
    };
  }, [image]);

  return (
    <StyledImageCropper className="icon_cropper">
      <div className="icon_cropper-crop_area">
        <ReactSVG
          className="icon_cropper-grid"
          src="images/icon-cropper-grid.svg"
        />
        <AvatarEditor
          ref={setEditorRef}
          image={uploadedFile}
          width={216}
          height={216}
          position={{ x: image.x, y: image.y }}
          scale={image.zoom}
          color={[6, 22, 38, 0.2]}
          border={0}
          rotate={0}
          borderRadius={108}
          onPositionChange={handlePositionChange}
          onImageReady={handleImageChange}
          disableHiDPIScaling
          crossOrigin="anonymous"
        />
      </div>
      <div
        className="icon_cropper-delete_button"
        onClick={handleDeleteImage}
        title={t("Common:Delete")}
      >
        <ReactSVG src={"images/trash.react.svg"} />
        <div className="icon_cropper-delete_button-text">
          {t("Common:Delete")}
        </div>
      </div>

      <div className="icon_cropper-zoom-container">
        <IconButton
          className="icon_cropper-zoom-container-button"
          size="16"
          onClick={handleZoomOutClick}
          iconName={"/static/images/zoom-minus.react.svg"}
          isFill={true}
          isClickable={false}
          isDisabled={isDisabled}
        />
        <Slider
          className="icon_cropper-zoom-container-slider"
          max={5}
          min={1}
          onChange={handleSliderChange}
          step={0.01}
          value={image.zoom}
          isDisabled={isDisabled}
        />
        <IconButton
          className="icon_cropper-zoom-container-button"
          size="16"
          onClick={handleZoomInClick}
          iconName={"/static/images/zoom-plus.react.svg"}
          isFill={true}
          isClickable={false}
          isDisabled={isDisabled}
        />
      </div>
    </StyledImageCropper>
  );
};

export default ImageCropper;
