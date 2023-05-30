import styled, { css } from "styled-components";
import Heading from "@docspace/components/heading";
import TextInput from "@docspace/components/text-input";
import ComboBox from "@docspace/components/combobox";
import Box from "@docspace/components/box";
import DropDown from "@docspace/components/drop-down";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import HelpButton from "@docspace/components/help-button";
import Link from "@docspace/components/link";
import ToggleButton from "@docspace/components/toggle-button";

import CheckIcon from "PUBLIC_DIR/images/check.edit.react.svg";
import CrossIcon from "PUBLIC_DIR/images/cross.edit.react.svg";
import DeleteIcon from "PUBLIC_DIR/images/mobile.actions.remove.react.svg";

import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import Base from "@docspace/components/themes/base";

const fillAvailableWidth = css`
  width: 100%;
  width: -moz-available;
  width: -webkit-fill-available;
  width: fill-available;
`;

const StyledInvitePanel = styled.div`
  .invite-panel-body {
    height: ${(props) =>
      props.hasInvitedUsers ? "calc(100% - 55px - 70px)" : "calc(100% - 55px)"};

    .scroll-body {
      padding-right: 0px !important;
    }
  }
`;

const ScrollList = styled.div`
  width: 100%;
  height: ${(props) =>
    props.scrollAllPanelContent && props.isTotalListHeight
      ? "auto"
      : props.offsetTop && `calc(100% - ${props.offsetTop}px)`};
`;

const StyledBlock = styled.div`
  padding: ${(props) => (props.noPadding ? "0px" : "0 16px")};
  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};
`;

StyledBlock.defaultProps = { theme: Base };

const StyledHeading = styled(Heading)`
  font-weight: 700;
  font-size: 18px;
`;

const StyledSubHeader = styled(Heading)`
  font-weight: 700;
  font-size: 16px;
  padding-left: 16px;
  padding-right: 16px;
  margin: 16px 0 8px 0;

  ${(props) =>
    props.inline &&
    css`
      display: inline-flex;
      align-items: center;
      gap: 16px;
    `};
`;

const StyledDescription = styled(Text)`
  padding-left: 16px;
  padding-right: 16px;
  color: ${(props) =>
    props.theme.createEditRoomDialog.commonParam.descriptionColor};
  margin-bottom: 16px;

  font-weight: 400;
  font-size: 12px;
  line-height: 16px;
`;

StyledDescription.defaultProps = { theme: Base };

const StyledRow = styled.div`
  width: calc(100% - 32px) !important;

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

  .invite-panel_access-selector {
    margin-left: auto;
    margin-right: 0;
  }
`;

const StyledInviteInput = styled.div`
  ${fillAvailableWidth}
  margin-left: 16px;
  margin-right: ${(props) => (props.hideSelector ? "16px" : "8px")};

  .input-link {
    height: 32px;

    > input {
      height: 30px;
    }
  }
`;

const StyledAccessSelector = styled.div`
  margin-right: 16px;
`;

const StyledEditInput = styled(TextInput)`
  width: 100%;
`;

const StyledComboBox = styled(ComboBox)`
  margin-left: auto;

  .combo-button-label,
  .combo-button-label:hover {
    text-decoration: none;
  }

  display: flex;
  align-items: center;
  justify-content: center;

  .combo-buttons_arrow-icon {
    margin-left: 2px;
  }

  padding: 0px;

  .combo-button {
    border-radius: 3px;
  }
`;

const StyledInviteInputContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  margin-bottom: 20px;

  .header_aside-panel {
    max-width: 100% !important;
  }
`;

const StyledDropDown = styled(DropDown)`
  ${(props) => props.width && `width: ${props.width}px`};

  .list-item {
    display: flex;
    align-items: center;
    gap: 8px;
    height: 48px;
  }
`;

const SearchItemText = styled(Text)`
  line-height: 16px;

  font-size: ${(props) =>
    props.primary ? "14px" : props.info ? "11px" : "12px"};
  font-weight: ${(props) => (props.primary || props.info ? "600" : "400")};

  color: ${(props) =>
    (props.primary && !props.disabled) || props.info
      ? props.theme.text.color
      : props.theme.text.disableColor};
  ${(props) => props.info && `margin-left: auto`}
`;

SearchItemText.defaultProps = { theme: Base };

const StyledEditButton = styled(Button)`
  width: 32px;
  height: 32px;
  padding: 0px;
`;

const iconStyles = css`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesEditingWrapper.fill} !important;
  }
  :hover {
    fill: ${(props) => props.theme.filesEditingWrapper.hoverFill} !important;
  }
`;

const StyledCheckIcon = styled(CheckIcon)`
  ${iconStyles}
`;

StyledCheckIcon.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  ${iconStyles}
`;

StyledCrossIcon.defaultProps = { theme: Base };

const StyledDeleteIcon = styled(DeleteIcon)`
  margin-left: auto;

  ${iconStyles}
`;

StyledDeleteIcon.defaultProps = { theme: Base };

const StyledHelpButton = styled(HelpButton)`
  margin-right: 8px;
`;

const StyledButtons = styled(Box)`
  padding: 16px 16px 16px 16px;
  display: flex;
  align-items: center;
  gap: 10px;

  position: absolute;
  bottom: 0px;
  width: 100%;
  background: ${(props) => props.theme.filesPanels.sharing.backgroundButtons};
  border-top: ${(props) => props.theme.filesPanels.sharing.borderTop};
`;

const StyledLink = styled(Link)`
  float: right;
`;

StyledButtons.defaultProps = { theme: Base };

const StyledToggleButton = styled(ToggleButton)`
  right: 8px;
  margin-top: -4px;
`;

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
  SearchItemText,
  StyledEditInput,
  StyledEditButton,
  StyledCheckIcon,
  StyledCrossIcon,
  StyledHelpButton,
  StyledDeleteIcon,
  StyledButtons,
  StyledLink,
  ScrollList,
  StyledAccessSelector,
  StyledToggleButton,
  StyledDescription,
};
