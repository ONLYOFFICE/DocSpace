import Text from "@appserver/components/text";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import ToggleButton from "@appserver/components/toggle-button";
import { mobile } from "@appserver/components/utils/device";
import { Base } from "@appserver/components/themes";
import CrossIcon from "@appserver/components/public/static/images/cross.react.svg";

const mobileView = css`
  top: 64px;

  width: 100vw !important;
  height: calc(100vh - 64px) !important;
`;

const StyledFilterBlock = styled.div`
  position: fixed;
  top: 0;
  right: 0;

  width: 480px;
  height: 100vh;

  z-index: 400;

  display: flex;
  flex-direction: column;

  background: ${(props) => props.theme.filterInput.filter.background};

  @media ${mobile} {
    ${mobileView}
  }

  ${isMobileOnly && mobileView}

  .people-selector {
    height: 100%;
    width: 100%;

    .selector-wrapper,
    .column-options {
      width: 100%;
    }
  }
`;

StyledFilterBlock.defaultProps = { theme: Base };

const StyledFilterBlockHeader = styled.div`
  height: 53px;
  min-height: 53px;

  padding: 0 16px;
  margin: 0;

  box-sizing: border-box;

  border-bottom: ${(props) =>
    props.isSelector ? "none" : props.theme.filterInput.filter.border};

  display: flex;
  align-items: center;
  justify-content: ${(props) => (props.isSelector ? "start" : "space-between")};

  .arrow-button {
    margin-right: 12px;
  }

  svg {
    cursor: pointer;
  }
`;

StyledFilterBlockHeader.defaultProps = { theme: Base };

const StyledFilterBlockItem = styled.div`
  padding: ${(props) =>
    !props.withoutHeader ? "12px 16px 0px 16px" : "6px 16px 0px 16px"};

  display: flex;
  flex-direction: column;
  justify-content: start;
`;

const StyledFilterBlockItemHeader = styled.div`
  height: 16px;
  line-height: 16px;

  display: flex;
  align-items: center;
`;

const StyledFilterBlockItemContent = styled.div`
  margin-top: ${(props) => !props.withoutHeader && "12px"};

  height: fit-content;

  display: flex;
  flex-direction: row;
  align-items: center;
  flex-wrap: wrap;
`;

const StyledFilterBlockItemSelector = styled.div`
  height: 32px;
  width: 100%;

  display: flex;
  flex-direction: row;
  align-items: center;

  margin: 0 0 11px;
`;

const StyledFilterBlockItemSelectorText = styled(Text)`
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  color: ${(props) => props.theme.filterInput.filter.color};

  margin-left: 8px;
`;

StyledFilterBlockItemSelectorText.defaultProps = { theme: Base };

const selectedItemTag = css`
  background: ${(props) =>
    props.theme.filterInput.filter.selectedItem.background};
  border-color: ${(props) =>
    props.theme.filterInput.filter.selectedItem.border};
`;

const StyledFilterBlockItemTag = styled.div`
  height: 30px;
  max-height: 30px;

  display: flex;
  flex-direction: row;
  align-items: center;

  border: ${(props) => props.theme.filterInput.filter.border};
  border-radius: 16px;

  box-sizing: border-box;

  padding: 4px 15px;

  margin: 0 6px 12px 0;

  cursor: pointer;

  ${(props) => props.isSelected && selectedItemTag}
`;

StyledFilterBlockItemTag.defaultProps = { theme: Base };

const selectedItemTagText = css`
  color: ${(props) => props.theme.filterInput.filter.selectedItem.color};
`;

const StyledFilterBlockItemTagText = styled(Text)`
  height: 20px;

  font-weight: normal;
  font-size: 13px;
  line-height: 20px;

  ${(props) => props.isSelected && selectedItemTagText}
`;

StyledFilterBlockItemTagText.defaultProps = { theme: Base };

const StyledFilterBlockItemTagIcon = styled.div`
  margin-left: 8px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  svg {
    path {
      fill: ${(props) => props.theme.filterInput.filter.selectedItem.color};
    }
  }
`;

StyledFilterBlockItemTagIcon.defaultProps = { theme: Base };

const StyledFilterBlockItemToggle = styled.div`
  width: 100%;
  height: 36px;

  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
`;

const StyledFilterBlockItemToggleText = styled(Text)`
  font-weight: 600;
  font-size: 13px;
  line-height: 36px;
`;

const StyledFilterBlockItemToggleButton = styled(ToggleButton)`
  position: static;
`;

const StyledFilterBlockItemSeparator = styled.div`
  height: 1px;
  width: 100%;

  background: ${(props) => props.theme.filterInput.filter.separatorColor};

  margin: 2px 0 0 0;
`;

StyledFilterBlockItemToggleButton.defaultProps = { theme: Base };

const StyledFilterBlockFooter = styled.div`
  position: fixed;
  bottom: 0;
  right: 0;

  z-index: 401;

  width: 480px;
  height: 72px;
  min-height: 72px;

  border-top: ${(props) => props.theme.filterInput.filter.border};

  box-sizing: border-box;

  padding: 0 16px;
  margin: 0;

  display: flex;
  align-items: center;
  justify-content: center;

  @media ${mobile} {
    width: 100vw;
  }

  ${isMobileOnly &&
  css`
    width: 100vw;
  `}
`;

StyledFilterBlockFooter.defaultProps = { theme: Base };

const StyledControlContainer = styled.div`
  background: ${(props) => props.theme.catalog.control.background};
  width: 24px;
  height: 24px;
  position: fixed;
  top: 30px;
  right: 10px;
  border-radius: 100px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
  display: none;

  @media ${mobile} {
    display: flex;
  }

  ${isMobileOnly &&
  css`
    display: flex;
  `}
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 12px;
  height: 12px;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }

  display: none;

  @media ${mobile} {
    display: flex;
  }

  ${isMobileOnly &&
  css`
    display: flex;
  `}
`;

StyledCrossIcon.defaultProps = { theme: Base };

export {
  StyledFilterBlock,
  StyledFilterBlockHeader,
  StyledFilterBlockItem,
  StyledFilterBlockItemHeader,
  StyledFilterBlockItemContent,
  StyledFilterBlockItemSelector,
  StyledFilterBlockItemSelectorText,
  StyledFilterBlockItemTag,
  StyledFilterBlockItemTagText,
  StyledFilterBlockItemTagIcon,
  StyledFilterBlockItemToggle,
  StyledFilterBlockItemToggleText,
  StyledFilterBlockItemToggleButton,
  StyledFilterBlockItemSeparator,
  StyledFilterBlockFooter,
  StyledControlContainer,
  StyledCrossIcon,
};
