import Text from "@docspace/components/text";
import styled, { css } from "styled-components";
import { isMobileOnly, isMobile } from "react-device-detect";

import ToggleButton from "@docspace/components/toggle-button";
import { mobile, tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";

const StyledFilterBlock = styled.div`
  position: fixed;
  top: 0;
  right: 0;

  width: 480px;
  height: 100%;

  z-index: 400;

  display: flex;
  flex-direction: column;

  background: ${(props) => props.theme.filterInput.filter.background};

  @media ${tablet} {
    max-width: calc(100% - 69px);
  }

  ${isMobile &&
  css`
    max-width: calc(100% - 69px);
  `}

  @media (max-width: 428px) {
    bottom: 0;
    top: unset;
    height: calc(100% - 64px);
    width: 100%;
    max-width: 100%;
  }

  .people-selector {
    height: 100%;
    width: 100%;

    .selector-wrapper,
    .column-options {
      width: 100%;
    }
  }

  .filter-body {
    height: ${(props) => (props.showFooter ? "calc(100% - 125px)" : "100%")};

    .combo-item {
      padding: 0;
    }

    .combo-button {
      justify-content: space-between;

      .combo-button-label {
        font-size: 13px;
        font-weight: 400;
        line-height: 20px;
      }
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

  h1 {
    font-weight: 700;
  }

  .arrow-button {
    margin-right: 12px;
  }

  svg {
    cursor: pointer;
  }
`;

StyledFilterBlockHeader.defaultProps = { theme: Base };

const StyledFilterBlockItem = styled.div`
  margin: ${(props) =>
    props.withoutHeader ? "0" : props.isFirst ? "12px 0 0 0" : "16px 0 0 0"};
  padding: 0 15px 0 16px;

  max-width: 480px;
  box-sizing: border-box;

  display: flex;
  flex-direction: column;
  justify-content: start;
`;

const StyledFilterBlockItemHeader = styled.div`
  height: 16px;
  line-height: 16px;
  margin-right: -16px;

  display: flex;
  align-items: center;
`;

const StyledFilterBlockItemContent = styled.div`
  margin: ${(props) =>
    props.withoutSeparator ? "12px -16px 0 0" : "12px -16px 16px 0"};

  height: fit-content;

  display: flex;
  flex-direction: row;
  align-items: center;
  flex-wrap: wrap;

  gap: ${(props) => (props.withMultiItems ? "12px 8px" : "8px")};
`;

const StyledFilterBlockItemSelector = styled.div`
  height: 32px;
  width: 100%;

  display: flex;
  flex-direction: row;
  align-items: center;
`;

const StyledFilterBlockItemSelectorText = styled(Text)`
  font-weight: 600;
  font-size: 13px;
  line-height: 15px;
  color: ${(props) => props.theme.filterInput.filter.color};
  margin-left: 8px;
  cursor: pointer;
`;

StyledFilterBlockItemSelectorText.defaultProps = { theme: Base };

const selectedItemTag = css`
  background: ${(props) =>
    props.theme.filterInput.filter.selectedItem.background};
  border-color: ${(props) =>
    props.theme.filterInput.filter.selectedItem.border};
`;

const selectedItemTagText = css`
  color: ${(props) => props.theme.filterInput.filter.selectedItem.color};
  font-weight: 600;
`;

const StyledFilterBlockItemTagText = styled(Text)`
  height: 20px;

  font-weight: 400;
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

  grid-gap: 0px;
`;
const StyledFilterBlockItemCheckboxContainer = styled.div`
  .checkbox {
    margin-right: 8px !important;
  }

  .checkbox-text {
    line-height: 20px;
  }
`;

const StyledFilterBlockItemSeparator = styled.div`
  height: 1px;
  width: calc(100% + 16px);

  margin-right: 16px;

  background: ${(props) => props.theme.filterInput.filter.separatorColor};
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

  gap: 10px;

  display: flex;
  align-items: center;
  justify-content: center;

  @media (max-width: 428px) {
    width: 100%;
  }
`;

StyledFilterBlockFooter.defaultProps = { theme: Base };

const StyledControlContainer = styled.div`
  display: flex;

  width: 24px;
  height: 24px;
  position: absolute;

  border-radius: 100px;
  cursor: pointer;

  align-items: center;
  justify-content: center;
  z-index: 450;

  top: 14px;
  left: -34px;

  ${isMobile &&
  css`
    top: 14px;
  `}

  @media (max-width: 428px) {
    top: -34px;
    right: 10px;
    left: unset;
  }
`;

StyledControlContainer.defaultProps = { theme: Base };

const StyledCrossIcon = styled(CrossIcon)`
  width: 17px;
  height: 17px;
  z-index: 455;
  path {
    fill: ${(props) => props.theme.catalog.control.fill};
  }
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
  StyledFilterBlockItemTagText,
  StyledFilterBlockItemTagIcon,
  StyledFilterBlockItemToggle,
  StyledFilterBlockItemToggleText,
  StyledFilterBlockItemToggleButton,
  StyledFilterBlockItemCheckboxContainer,
  StyledFilterBlockItemSeparator,
  StyledFilterBlockFooter,
  StyledControlContainer,
  StyledCrossIcon,
};
