import React, { useState, useCallback, useEffect } from "react";

import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import { HexColorPicker, HexColorInput } from "react-colorful";
import { isMobileOnly } from "react-device-detect";

const StyledComponent = styled.div`
  .save-button {
    margin-right: 10px;
  }

  .hex-color-picker .react-colorful {
    width: auto;
    height: 239px;
    padding-bottom: 16px;
  }

  .react-colorful__saturation {
    margin: 16px 0;
    border-radius: 3px;
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
    height: 32px;
    outline: none;
    padding: 6px 8px;
    border: 1px solid #d0d5da;
    border-radius: 3px;

    width: 100%;
    box-sizing: border-box;
  }

  .hex-value-label {
    line-height: 20px;
  }

  .hex-button {
    display: flex;

    .apply-button {
      margin-right: 8px;
    }
  }

  @media (min-width: 428px) {
    ${!isMobileOnly &&
    css`
      .hex-color-picker {
        display: flex;
        flex-direction: column;
        padding-bottom: 16px;
        width: 195px;
      }

      .hex-value-container {
        order: 2;
        padding-bottom: 16px;
      }

      .hex-color-picker .react-colorful {
        order: 1;
      }

      .hex-button {
        order: 3;
      }
    `}
  }
`;

const HexColorPickerComponent = (props) => {
  const {
    onCloseHexColorPicker,
    onAppliedColor,
    setColor,
    color,
    viewMobile,
  } = props;

  return (
    <StyledComponent viewMobile={viewMobile}>
      <div className="hex-color-picker">
        <div className="hex-value-container">
          {!viewMobile && <div className="hex-value-label">Hex code:</div>}

          <HexColorInput
            className="hex-value"
            prefixed
            color={color.toUpperCase()}
            onChange={setColor}
          />
        </div>

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
