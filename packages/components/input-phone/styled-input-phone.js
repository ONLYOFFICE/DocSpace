import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import TextInput from "@docspace/components/text-input";
import styled from "styled-components";

export const StyledBox = styled(Box)`
  position: relative;
  box-sizing: border-box;
  max-width: 320px;
  border: 1px solid ${(props) => (props.hasError ? "#f21c0e" : "#d0d5da")};
  border-radius: 3px;
  :focus-within {
    border-color: ${(props) => (props.hasError ? "#f21c0e" : "#2da7db;")};
  }
`;

export const StyledComboBox = styled(ComboBox)`
  width: 57px;

  .combo-button {
    width: 100%;
    height: 100%;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    cursor: pointer;
    padding-left: 0;

    .invalid-flag {
      width: 26px;
      height: 20px;
      margin-left: 6px;
      margin-top: 9px;
    }

    .forceColor {
      width: 36px;
      height: 36px;
      svg {
        path:last-child {
          fill: none;
        }
      }
    }
  }
  .combo-buttons_arrow-icon {
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: 4px solid #a3a9ae;
    cursor: pointer;
    margin: 0;
    position: absolute;
    top: 21px;
    right: 10px;
  }
`;

export const StyledInput = styled(TextInput)`
  height: 44px;
  padding-left: 10px;
  border-left: 1px solid #d0d5da !important;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
`;

export const StyledDropDown = styled(DropDown)`
  padding: 12px 16px;
  box-sizing: border-box;
  margin-top: 4px;
  outline: 1px solid #d0d5da;
  border-radius: 3px;
  box-shadow: none;
`;

export const StyledDropDownItem = styled(DropDownItem)`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  padding: 0;
  height: 36px;
  svg {
    width: 36px !important;
    height: 36px !important;
  }

  .drop-down-icon > div {
    height: 36px;
  }

  .drop-down-icon {
    width: 36px;
    height: 36px;
    margin-right: 0;
    svg {
      path:last-child {
        fill: none;
      }
    }
  }
`;
