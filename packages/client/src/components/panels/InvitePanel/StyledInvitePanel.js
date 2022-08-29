import styled from "styled-components";
import Heading from "@docspace/components/heading";
import TextInput from "@docspace/components/text-input";
import ComboBox from "@docspace/components/combobox";
import Box from "@docspace/components/box";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";
import Text from "@docspace/components/text";

import { Base } from "@docspace/components/themes";

const StyledInvitePanel = styled.div``;

const StyledBlock = styled.div`
  padding: ${(props) => (props.noPadding ? "0px" : "0 16px")};
  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};
`;

StyledBlock.defaultProps = { theme: Base };

const fillAvailableWidth = `
  width: 100%;
  width: -moz-available;
  width: -webkit-fill-available;
  width: fill-available;
`;

const StyledHeading = styled(Heading)`
  font-weight: 700;
  font-size: 18px;
`;

const StyledSubHeader = styled(Heading)`
  font-weight: 700;
  font-size: 16px;
  padding-left: 16px;
  margin: 20px 0;
`;

const StyledRow = styled.div`
  ${fillAvailableWidth}

  display: inline-flex;
  align-items: center;
  gap: 8px;

  min-height: 41px;
  margin-left: 16px;
  box-sizing: border-box;
  border-bottom: none;

  a {
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
  }
`;

const StyledInviteInput = styled(TextInput)`
  ${fillAvailableWidth}

  margin-left: 16px;
  margin-bottom: 20px;
`;

const StyledComboBox = styled(ComboBox)`
  margin-left: auto;

  .combo-button-label,
  .combo-button-label:hover {
    text-decoration: none;
  }
`;

const StyledInviteInputContainer = styled(Box)`
  position: relative;
`;

const StyledDropDown = styled(DropDown)`
  ${fillAvailableWidth}
`;
const StyledDropDownItem = styled(DropDownItem)`
  display: flex;
  align-items: center;
  gap: 8px;
  height: 48px;
`;

const SearchItemText = styled(Text)`
  line-height: 16px;

  font-size: ${(props) =>
    props.primary ? "14px" : props.info ? "11px" : "12px"};
  font-weight: ${(props) => (props.primary || props.info ? "600" : "400")};

  color: ${(props) =>
    props.primary || props.info
      ? props.theme.text.color
      : props.theme.text.disableColor};
  ${(props) => props.info && `margin-left: auto`}
`;

StyledBlock.defaultProps = { theme: Base };

export {
  StyledBlock,
  StyledHeading,
  StyledInvitePanel,
  StyledRow,
  StyledSubHeader,
  StyledInviteInput,
  StyledComboBox,
  StyledInviteInputContainer,
  StyledDropDown,
  StyledDropDownItem,
  SearchItemText,
};
