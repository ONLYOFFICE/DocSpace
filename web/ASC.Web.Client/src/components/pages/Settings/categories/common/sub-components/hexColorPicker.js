import React, { useState, useCallback, useEffect } from "react";

import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@appserver/components/button";
import { HexColorPicker, HexColorInput } from "react-colorful";

const StyledComponent = styled.div`
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

const HexColorPickerComponent = (props) => {
  const { onCloseHexColorPicker, onAppliedColor, setColor, color } = props;

  return (
    <StyledComponent>
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
            onClick={onAppliedColor}
          />
          <Button
            label="Cancel"
            className="button"
            size="small"
            scale={true}
            onClick={onCloseHexColorPicker}
          />
        </div>
      </div>
    </StyledComponent>
  );
};

export default HexColorPickerComponent;
