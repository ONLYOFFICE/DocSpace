import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import { isMobileOnly } from "react-device-detect";

const StyledComponent = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    width: 100%;
    display: flex;
    padding-right: 32px;
  }

  .flex {
    display: flex;
    justify-content: space-between;

    :not(:last-child) {
      padding-bottom: 20px;
    }
  }

  .text {
    font-weight: 700;
    font-size: 18px;
    line-height: 24px;
  }

  .color-button {
    width: 46px;
    height: 46px;
  }

  .relative {
    position: relative;
  }

  .save-button {
    margin-right: 10px;
  }

  /* .hex-color-picker {
    position: fixed;
    bottom: 0;
    left: 0;
    width: 100%;
    padding: 0 16px 16px 16px;
    box-sizing: border-box;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  } */

  .hex-color-picker .react-colorful {
    width: auto;
    height: 195px;
    padding-bottom: 16px;
  }

  .react-colorful__saturation {
    margin: 16px 0;
  }

  .hex-color-picker .react-colorful__saturation-pointer {
    width: 14px;
    height: 14px;
    transform: none !important;
  }

  .hex-color-picker .react-colorful__hue {
    border-radius: 6px;
    height: 12px;
  }

  .hex-color-picker .react-colorful__hue-pointer {
    width: 24px;
    height: 24px;
    box-shadow: 0px 3px 10px rgba(0, 0, 0, 0.25);
    border: 6px solid #fff;
  }

  .hex-value {
    outline: none;
    padding: 6px 8px;
    border: 1px solid #d0d5da;
    border-radius: 3px;
    margin-top: 16px;
    width: 100%;
    box-sizing: border-box;
  }

  .hex-button {
    display: flex;

    .apply-button {
      margin-right: 8px;
    }
  }

  @media (min-width: 428px) {
  }
`;

const ColorSchemeDialog = (props) => {
  const {
    visible,
    onClose,
    header,

    nodeButtonsColor,
    nodeAccentColor,
    nodeHexColorPickerAccent,
    nodeHexColorPickerButtons,
    t,
  } = props;

  console.log(!isMobileOnly, !window.innerWidth <= 428);
  return (
    <StyledComponent visible={visible} onClose={onClose}>
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>
          <div className="flex relative">
            <div className="text">Accent</div>
            {nodeAccentColor}

            {!(isMobileOnly || window.innerWidth <= 428) &&
              nodeHexColorPickerAccent}
          </div>

          <div className="flex relative">
            <div className="text">Buttons</div>
            {nodeButtonsColor}

            {!(isMobileOnly || window.innerWidth <= 428) &&
              nodeHexColorPickerButtons}
          </div>
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        {(isMobileOnly || window.innerWidth <= 428) && nodeHexColorPickerAccent}

        {(isMobileOnly || window.innerWidth <= 428) &&
          nodeHexColorPickerButtons}

        {!(nodeHexColorPickerButtons || nodeHexColorPickerAccent) && (
          <>
            <Button
              label="Save"
              size="normal"
              className="save-button"
              primary={true}
              scale={true}
            />
            <Button
              label="Restore to default"
              className="button"
              size="normal"
              scale={true}
            />
          </>
        )}
      </ModalDialog.Footer>
    </StyledComponent>
  );
};

export default ColorSchemeDialog;
