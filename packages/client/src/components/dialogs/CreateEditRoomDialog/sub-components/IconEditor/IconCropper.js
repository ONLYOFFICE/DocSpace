import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import throttle from "lodash/throttle";
import AvatarEditor from "react-avatar-editor";

import Slider from "@docspace/components/slider";
import IconButton from "@docspace/components/icon-button";

const StyledIconCropper = styled.div`
  max-width: 216px;

  .icon_cropper-crop_area {
    width: 216px;
    height: 216px;
    margin-bottom: 4px;
    position: relative;
    .icon_cropper-grid {
      pointer-events: none;
      position: absolute;
      width: 216px;
      height: 216px;
      top: 0;
      bottom: 0;
      left: 0;
      right: 0;
      svg {
        opacity: 0.2;
        path {
          fill: #333333;
        }
      }
    }
  }

  .icon_cropper-delete_button {
    cursor: pointer;
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    gap: 8px;
    width: 100%;
    padding: 6px 0;
    background: #f8f9f9;
    border-radius: 3px;
    margin-bottom: 12px;

    &-text {
      font-weight: 600;
      line-height: 20px;
      color: #555f65;
    }

    svg {
      path {
        fill: #657077;
      }
    }
  }

  .icon_cropper-zoom-container {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    gap: 12px;
    margin-bottom: 20px;

    &-slider {
      margin: 0;
    }

    &-button {
      user-select: none;
    }
  }
`;

const IconCropper = ({
  t,
  icon,
  onChangeIcon,
  uploadedFile,
  setUploadedFile,
  setPreviewIcon,
}) => {
  let editorRef = null;
  const setEditorRef = (editor) => (editorRef = editor);

  const handlePositionChange = (position) =>
    onChangeIcon({ ...icon, x: position.x, y: position.y });

  const handleSliderChange = (e, newZoom = null) =>
    onChangeIcon({ ...icon, zoom: newZoom ? newZoom : +e.target.value });

  const handleZoomInClick = () =>
    handleSliderChange({}, icon.zoom <= 4.5 ? icon.zoom + 0.5 : 5);

  const handleZoomOutClick = () =>
    handleSliderChange({}, icon.zoom >= 1.5 ? icon.zoom - 0.5 : 1);

  const handleDeleteImage = () => setUploadedFile(null);

  const handleImageChange = throttle(() => {
    if (editorRef) {
      const newPreveiwImage = editorRef.getImageScaledToCanvas()?.toDataURL();
      setPreviewIcon(newPreveiwImage);
    }
  }, 300);

  useEffect(() => {
    handleImageChange();
    return () => {
      setPreviewIcon("");
    };
  }, [icon]);

  return (
    <StyledIconCropper className="icon_cropper">
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
          position={{ x: icon.x, y: icon.y }}
          scale={icon.zoom}
          color={[6, 22, 38, 0.2]}
          border={0}
          rotate={0}
          borderRadius={108}
          onPositionChange={handlePositionChange}
          onImageReady={handleImageChange}
          disableHiDPIScaling
        />
      </div>

      <div className="icon_cropper-delete_button" onClick={handleDeleteImage}>
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
        />
        <Slider
          className="icon_cropper-zoom-container-slider"
          max={5}
          min={1}
          onChange={handleSliderChange}
          step={0.01}
          value={icon.zoom}
        />
        <IconButton
          className="icon_cropper-zoom-container-button"
          size="16"
          onClick={handleZoomInClick}
          iconName={"/static/images/zoom-plus.react.svg"}
          isFill={true}
          isClickable={false}
        />
      </div>
    </StyledIconCropper>
  );
};

export default IconCropper;
