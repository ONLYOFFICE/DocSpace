import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import { HexColorPicker } from "react-colorful";

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
    background: green;
  }

  .save-button {
    margin-right: 10px;
  }

  .hex-color-picker {
    position: fixed;
    bottom: 0;
    width: 100%;
    box-sizing: border-box;
    box-shadow: 0px -4px 60px rgba(4, 15, 27, 0.12);
    border-radius: 6px;
  }

  .react-colorful {
    width: auto;
  }

  .react-colorful__saturation {
    // width: 195px;
    height: 195px;
  }

  .react-colorful__saturation-pointer {
    width: 14px;
    height: 14px;
    border: 0;
  }
`;

const ColorSchemeDialog = (props) => {
  const { visible, onClose, t } = props;

  const [color, setColor] = useState("#b32aa9");
  const [openHexColorPicker, setOpenHexColorPicker] = useState(false);

  const onClick = () => {
    setOpenHexColorPicker(true);
  };

  return (
    <StyledComponent visible={visible} onClose={onClose}>
      <ModalDialog.Header>Edit color scheme</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="flex">
          <div className="text">Accent</div>
          <div className="color-button" onClick={onClick}></div>
        </div>
        <div className="flex">
          <div className="text">Buttons</div>
          <div className="color-button" onClick={onClick}></div>
        </div>

        {openHexColorPicker && (
          <div className="hex-color-picker">
            <HexColorPicker color={color} onChange={setColor} />

            <div className="value" style={{ borderLeftColor: color }}>
              Current color is {color}
            </div>

            <div className="hex-button">
              <Button
                label="Apply"
                size="normal"
                className="save-button"
                primary={true}
                scale={true}
              />
              <Button
                label="Cancel"
                className="button"
                size="normal"
                scale={true}
              />
            </div>
          </div>
        )}
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
