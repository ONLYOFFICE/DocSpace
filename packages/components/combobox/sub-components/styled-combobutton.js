import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";
import Base from "../../themes/base";
import NoUserSelect from "../../utils/commonStyles";

import TriangleDownIcon from "PUBLIC_DIR/images/triangle.down.react.svg";
import commonIconsStyles from "../../utils/common-icons-style";

import Loader from "../../loader";

const StyledTriangleDownIcon = styled(TriangleDownIcon)`
  ${commonIconsStyles}
`;

const modernViewButton = css`
  height: ${(props) => props.theme.comboBox.button.heightModernView};
  background: ${(props) =>
    props.isOpen || props.isLoading
      ? props.theme.comboBox.button.focusBackgroundModernView
      : props.theme.comboBox.button.backgroundModernView};

  border: none !important;
  padding-right: 0px;
`;

const hoverModernViewButton = css`
  background: ${(props) =>
    props.isOpen || props.isLoading
      ? props.theme.comboBox.button.focusBackgroundModernView
      : props.theme.comboBox.button.hoverBackgroundModernView} !important;
`;

const StyledComboButton = styled.div`
  display: flex;
  align-items: center;
  gap: ${(props) => props.type && "4px"};
  justify-content: center;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  height: ${(props) =>
    props.noBorder
      ? props.theme.comboBox.button.height
      : props.theme.comboBox.button.heightWithBorder};
  width: ${(props) =>
    (props.scaled && "100%") ||
    (props.size === "base" && props.theme.comboBox.width.base) ||
    (props.size === "middle" && props.theme.comboBox.width.middle) ||
    (props.size === "big" && props.theme.comboBox.width.big) ||
    (props.size === "huge" && props.theme.comboBox.width.huge) ||
    (props.size === "content" && "fit-content")};

  ${NoUserSelect};

  padding-left: ${(props) =>
    props.size === "content"
      ? props.theme.comboBox.button.paddingLeft
      : props.theme.comboBox.button.selectPaddingLeft};

  padding-right: ${(props) =>
    props.size === "content"
      ? props.displayArrow
        ? props.theme.comboBox.button.paddingRight
        : props.theme.comboBox.button.paddingRightNoArrow
      : props.displayArrow
      ? props.theme.comboBox.button.selectPaddingRight
      : props.theme.comboBox.button.selectPaddingRightNoArrow};

  background: ${(props) =>
    !props.noBorder
      ? props.theme.comboBox.button.background
      : props.theme.comboBox.button.backgroundWithBorder};

  color: ${(props) =>
    props.isDisabled
      ? props.theme.comboBox.button.disabledColor
      : props.theme.comboBox.button.color};

  box-sizing: border-box;

  ${(props) =>
    !props.noBorder &&
    !props.type &&
    `
    border:  ${props.theme.comboBox.button.border};
    border-radius: ${props.theme.comboBox.button.borderRadius};
  `}

  border-color: ${(props) =>
    props.isOpen && props.theme.comboBox.button.openBorderColor};

  ${(props) =>
    props.isDisabled &&
    !props.noBorder &&
    `
    border-color: ${props.theme.comboBox.button.disabledBorderColor};
    background: ${props.theme.comboBox.button.disabledBackground};
  `}

  ${(props) =>
    !props.noBorder &&
    `
    height: 32px;
  `}

  ${(props) => props.modernView && modernViewButton}


  .optionalBlock {
    svg {
      path {
        fill: ${(props) =>
          props.isOpen
            ? props.theme.iconButton.hoverColor
            : props.theme.iconButton.color};
      }
    }
  }
  :hover {
    border-color: ${(props) =>
      props.isOpen
        ? props.theme.comboBox.button.hoverBorderColorOpen
        : props.theme.comboBox.button.hoverBorderColor};
    cursor: ${(props) =>
      props.isDisabled ||
      (!props.containOptions && !props.withAdvancedOptions) ||
      props.isLoading
        ? "default"
        : "pointer"};

    ${(props) =>
      props.isDisabled &&
      `
      border-color: ${props.theme.comboBox.button.hoverDisabledBorderColor};
    `}

    ${(props) => props.modernView && hoverModernViewButton}

    .optionalBlock {
      svg {
        path {
          fill: ${(props) => props.theme.iconButton.hoverColor};
        }
      }
    }
  }
  .combo-button-label {
    visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};
    margin-right: ${(props) =>
      props.noBorder
        ? props.theme.comboBox.label.marginRight
        : props.theme.comboBox.label.marginRightWithBorder};
    color: ${(props) =>
      props.isDisabled
        ? props.theme.comboBox.label.disabledColor
        : props.isSelected
        ? props.theme.comboBox.label.selectedColor
        : props.theme.comboBox.label.color};

    max-width: ${(props) =>
      props.scaled ? "100%" : props.theme.comboBox.label.maxWidth};

    ${(props) =>
      props.noBorder &&
      `
      line-height: ${props.theme.comboBox.label.lineHeightWithoutBorder};
    `}
  }

  :focus {
    outline: none;
    border-color: ${(props) =>
      props.isOpen
        ? props.theme.comboBox.button.hoverBorderColorOpen
        : props.theme.comboBox.button.hoverBorderColor};

    .optionalBlock {
      svg {
        path {
          fill: ${(props) =>
            props.isOpen
              ? props.theme.iconButton.hoverColor
              : props.theme.iconButton.color};
        }
      }
    }
  }
`;
StyledComboButton.defaultProps = { theme: Base };

const StyledOptionalItem = styled.div`
  margin-right: ${(props) => props.theme.comboBox.childrenButton.marginRight};

  visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};

  path {
    fill: ${(props) =>
      props.defaultOption
        ? props.isDisabled
          ? props.theme.comboBox.childrenButton.defaultDisabledColor
          : props.theme.comboBox.childrenButton.defaultColor
        : props.isDisabled
        ? props.theme.comboBox.childrenButton.disabledColor
        : props.theme.comboBox.childrenButton.color};
  }
`;
StyledOptionalItem.defaultProps = { theme: Base };

const StyledIcon = styled.div`
  margin-right: ${(props) => props.theme.comboBox.childrenButton.marginRight};
  width: ${(props) => props.theme.comboBox.childrenButton.width};
  height: ${(props) => props.theme.comboBox.childrenButton.height};

  visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};

  .combo-button_selected-icon {
    path {
      fill: ${(props) =>
        props.defaultOption
          ? props.isDisabled
            ? props.theme.comboBox.childrenButton.defaultDisabledColor
            : props.theme.comboBox.childrenButton.defaultColor
          : props.isDisabled
          ? props.theme.comboBox.childrenButton.disabledColor
          : props.isSelected
          ? props.theme.comboBox.childrenButton.selectedColor
          : props.theme.comboBox.childrenButton.color};
    }
  }
  svg {
    &:not(:root) {
      width: 100%;
      height: 100%;
    }
  }
`;
StyledIcon.defaultProps = { theme: Base };

const StyledArrowIcon = styled.div`
  display: flex;
  align-self: center;

  visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};

  .combo-buttons_expander-icon {
    path {
      fill: ${(props) => props.theme.comboBox.label.selectedColor};
    }
  }

  width: ${(props) =>
    props.displayArrow ? props.theme.comboBox.arrow.width : "0px"};
  flex: ${(props) =>
    props.displayArrow ? props.theme.comboBox.arrow.flex : "0px"};
  margin-right: ${(props) =>
    props.displayArrow ? props.theme.comboBox.arrow.marginRight : "0px"};
  margin-left: ${(props) =>
    props.displayArrow ? props.theme.comboBox.arrow.marginLeft : "0px"};

  ${(props) =>
    props.isOpen &&
    `
    transform: scale(1, -1);
  `}

  ${isMobileOnly &&
  css`
    margin-left: auto;
  `}
`;

StyledArrowIcon.defaultProps = { theme: Base };

const StyledLoader = styled(Loader)`
  position: absolute;
  margin-left: ${(props) =>
    props.displaySize === "content" ? "-16px" : "-8px"};
  margin-top: 2px;
`;

export {
  StyledArrowIcon,
  StyledIcon,
  StyledOptionalItem,
  StyledComboButton,
  StyledTriangleDownIcon,
  StyledLoader,
};
