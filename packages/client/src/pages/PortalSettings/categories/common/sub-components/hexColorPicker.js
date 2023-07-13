import React from "react";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import Button from "@docspace/components/button";
import { HexColorPicker, HexColorInput } from "react-colorful";

const StyledComponent = styled.div`
  .save-button {
    margin-right: 10px;
  }

  .hex-color-picker .react-colorful {
    width: auto;
    height: 250px;
    padding-bottom: 26px;
  }

  .react-colorful__saturation {
    margin: 16px 0 26px 0;
    border-radius: 3px;
  }

  .hex-color-picker .react-colorful__interactive {
    width: 183px;
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
    width: 30px;
    height: 30px;
    box-shadow: 0px 3px 10px rgba(0, 0, 0, 0.25);
    border: 8px solid #fff;
  }

  .hex-value {
    height: 32px;
    outline: none;
    padding: 6px 8px;
    border: 1px solid ${(props) => (props.theme.isBase ? "#d0d5da" : "#474747")};
    border-radius: 3px;
    width: 100%;
    box-sizing: border-box;
    background: ${(props) => !props.theme.isBase && "#282828"};
    color: ${(props) => !props.theme.isBase && "#5C5C5C"};
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
`;

const HexColorPickerComponent = (props) => {
  const { onCloseHexColorPicker, onAppliedColor, setColor, color } = props;

  const { t } = useTranslation("Common");

  return (
    <StyledComponent>
      <div className="hex-color-picker">
        <div className="hex-value-container">
          <div className="hex-value-label">Hex code:</div>

          <HexColorInput
            className="hex-value"
            prefixed
            color={color.toUpperCase()}
            onChange={setColor}
          />
        </div>

        <HexColorPicker color={color.toUpperCase()} onChange={setColor} />

        <div className="hex-button">
          <Button
            label={t("Common:ApplyButton")}
            size="small"
            className="apply-button"
            primary={true}
            scale={true}
            onClick={onAppliedColor}
          />
          <Button
            label={t("Common:CancelButton")}
            className="cancel-button button"
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
