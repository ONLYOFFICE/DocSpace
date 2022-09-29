import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import TextInput from "@docspace/components/text-input";
import styled from "styled-components";

export const StyledBox = styled(Box)`
  display: flex;
  align-items: center;
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
  height: 44px;
  .combo-button {
    width: 100%;
    height: 100%;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    cursor: pointer;
    padding-left: 0;
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

export const CountryName = styled.h3`
  font-size: 13px;
  font-weight: 400;
  color: #33333;
  margin: 0;
  margin-left: 10px;
  line-height: 20px;
`;

export const CountryDialCode = styled.p`
  font-size: 13px;
  font-weight: 400;
  line-height: 20px;
  margin: 0;
  margin-left: 5px;
  color: #a3a9ae;
`;

export const StyledText = styled.p`
  color: #d0d5da;
  text-align: center;
  font-size: 14px;
  padding: 50px 0;
  margin: 0;
`;

export const ErrorText = styled.span`
  display: inline-block;
  margin: 0;
  margin-top: 5px;
  color: #f21c0e;
  font-weight: 400;
  font-size: 11px;
  line-height: 14px;
`;
