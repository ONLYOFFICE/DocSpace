import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import { HexColorPicker, HexColorInput } from "react-colorful";
import InputBlock from "@appserver/components/input-block";

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

  .accent {
    background: ${(props) =>
      props.selectedAccentColorAndButtonsMain.accentColor
        ? props.selectedAccentColorAndButtonsMain.accentColor
        : props.selectedAccentColorAndButtonsMain.addColor};
  }

  .buttons {
    background: ${(props) =>
      props.selectedAccentColorAndButtonsMain.buttonsMain
        ? props.selectedAccentColorAndButtonsMain.buttonsMain
        : props.selectedAccentColorAndButtonsMain.addColor};
  }

  .save-button {
    margin-right: 10px;
  }

  .hex-color-picker {
    position: fixed;
    bottom: 0;
    left: 0;
    width: 100%;
    padding: 0 16px 16px 16px;
    box-sizing: border-box;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  }

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
    selectedAccentColorAndButtonsMain,
    t,
  } = props;

  const [color, setColor] = useState("#FF9933");
  const [openHexColorPicker, setOpenHexColorPicker] = useState(false);

  const onClick = () => {
    setOpenHexColorPicker(true);
  };

  return (
    <StyledComponent
      selectedAccentColorAndButtonsMain={selectedAccentColorAndButtonsMain}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>
          <div>
            <div className="flex">
              <div className="text">Accent</div>
              <div className="color-button accent" onClick={onClick}></div>
            </div>
            <div className="flex">
              <div className="text">Buttons</div>
              <div className="color-button buttons" onClick={onClick}></div>
            </div>
          </div>

          {openHexColorPicker && (
            <div className="hex-color-picker">
              <HexColorInput
                className="hex-value"
                prefixed
                color={color.toUpperCase()}
                onChange={setColor}
              />
              <HexColorPicker color={color} onChange={setColor} />

              <div className="hex-button">
                <Button
                  label="Apply"
                  size="small"
                  className="apply-button"
                  primary={true}
                  scale={true}
                />
                <Button
                  label="Cancel"
                  className="button"
                  size="small"
                  scale={true}
                />
              </div>
            </div>
          )}
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        {!openHexColorPicker && (
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
