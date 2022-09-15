import Box from "@docspace/components/box";
import ComboBox from "@docspace/components/combobox";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import TextInput from "@docspace/components/text-input";
import styled from "styled-components";

export const StyledBox = styled(Box)`
  position: relative;
  max-width: 320px;
`;

export const StyledComboBox = styled(ComboBox)`
  .combo-button-label {
    font-weight: 400;
    line-height: 22px;
    padding: 10px;
    margin: 0;
    text-align: center;
  }
  .combo-button {
    height: 44px;
    border-right: 0;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    cursor: pointer;
    padding-left: 0;
  }
`;

export const StyledInput = styled(TextInput)`
  padding-left: 10px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
`;

export const StyledDropDown = styled(DropDown)`
  padding: 12px 16px;
  box-sizing: border-box;
  margin-top: 4px;
  border: 1px solid #d0d5da;
  border-radius: 3px;
`;

export const StyledDropDownItem = styled(DropDownItem)`
  display: flex;
  align-items: center;
  box-sizing: border-box;
  padding: 9px 0;
`;

export const CountryFlag = styled.div`
  border: 1px solid #d0d5da;
  margin-right: 10px;
  padding: 0 5px;
`;

export const CountryName = styled.h3`
  font-size: 13px;
  font-weight: 400;
  color: #33333;
  margin: 0;
  margin-right: 5px;
  line-height: 20px;
`;

export const CountryDialCode = styled.p`
  font-size: 13px;
  font-weight: 400;
  line-height: 20px;
  margin: 0;
  color: #a3a9ae;
`;
