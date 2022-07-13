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
    left: 0;
    width: 100%;
    padding: 0 16px 16px 16px;
    box-sizing: border-box;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  }

  .react-colorful {
    width: auto;
    height: 239px;
  }

  .react-colorful__pointer {
    width: 14px;
    height: 14px;
  }

  .react-colorful__hue-pointer {
    width: 24px;
    height: 24px;
    border: 6px solid #fff;
  }

  .react-colorful__last-control {
    border-radius: 6px;
    height: 12px;
    margin: 16px 0;
  }

  .hex-value {
    padding: 6px 8px;
    border: 1px solid #d0d5da;
    border-radius: 3px;
    margin: 16px 0;
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
            <div className="hex-value">{color.toUpperCase()}</div>
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
