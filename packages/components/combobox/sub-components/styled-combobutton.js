import styled, { css } from "styled-components";
import Base from "../../themes/base";
import NoUserSelect from "../../utils/commonStyles";

const modernViewButton = css`
  height: ${(props) => props.theme.comboBox.button.heightModernView};
  background: ${(props) =>
    props.isOpen
      ? props.theme.comboBox.button.activeBackgroundModernView
      : props.theme.comboBox.button.backgroundModernView};

  border: none;
`;

const hoverModernViewButton = css`
  background: ${(props) =>
    props.isOpen
      ? props.theme.comboBox.button.activeBackgroundModernView
      : props.theme.comboBox.button.hoverBackgroundModernView} !important;
`;

const StyledComboButton = styled.div`
  display: flex;
  align-items: center;
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

  padding-left: ${(props) => props.theme.comboBox.button.paddingLeft};

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

  :hover {
    ${(props) => props.modernView && hoverModernViewButton}

    border-color: ${(props) =>
      props.isOpen
        ? props.theme.comboBox.button.hoverBorderColorOpen
        : props.theme.comboBox.button.hoverBorderColor};
    cursor: ${(props) =>
      props.isDisabled || (!props.containOptions && !props.withAdvancedOptions)
        ? "default"
        : "pointer"};

    ${(props) =>
      props.isDisabled &&
      `
      border-color: ${props.theme.comboBox.button.hoverDisabledBorderColor};
    `}
  }
  .combo-button-label {
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

    max-width: ${(props) => props.theme.comboBox.label.maxWidth};

    ${(props) =>
      props.noBorder &&
      `
      line-height: ${props.theme.comboBox.label.lineHeightWithoutBorder};
      text-decoration: ${props.theme.comboBox.label.lineHeightTextDecoration};
    `}

    ${(props) =>
      props.isOpen &&
      props.noBorder &&
      `
      text-decoration: underline dashed;
    `};
  }
  .combo-button-label:hover {
    ${(props) =>
      props.noBorder &&
      !props.isDisabled &&
      `
      text-decoration: underline dashed;
    `}
  }
`;
StyledComboButton.defaultProps = { theme: Base };

const StyledOptionalItem = styled.div`
  margin-right: ${(props) => props.theme.comboBox.childrenButton.marginRight};

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
  align-self: start;

  .combo-buttons_expander-icon {
    path {
      fill: ${(props) => props.theme.comboBox.arrow.fillColor};
    }
  }

  width: ${(props) =>
    props.needDisplay ? props.theme.comboBox.arrow.width : "0px"};
  flex: ${(props) =>
    props.needDisplay ? props.theme.comboBox.arrow.flex : "0px"};
  margin-top: ${(props) =>
    props.noBorder
      ? props.theme.comboBox.arrow.marginTopWithBorder
      : props.theme.comboBox.arrow.marginTop};
  margin-right: ${(props) =>
    props.needDisplay ? props.theme.comboBox.arrow.marginRight : "0px"};
  margin-left: ${(props) =>
    props.needDisplay ? props.theme.comboBox.arrow.marginLeft : "0px"};

  ${(props) =>
    props.isOpen &&
    `
    transform: scale(1, -1);
  `}
`;
StyledArrowIcon.defaultProps = { theme: Base };

export { StyledArrowIcon, StyledIcon, StyledOptionalItem, StyledComboButton };
