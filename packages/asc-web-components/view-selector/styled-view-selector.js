import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledViewSelector = styled.div`
  height: 32px;
  position: relative;
  box-sizing: border-box;
  display: flex;
`;

const firstItemStyle = css`
  border-top-left-radius: 3px;
  border-bottom-left-radius: 3px;
  border-right: 1px solid rgba(255, 255, 255, 0);
`;

const lastItemStyle = css`
  border-top-right-radius: 3px;
  border-bottom-right-radius: 3px;
  border-left: 1px solid rgba(255, 255, 255, 0);
`;

const middleItemsStyle = css`
  border-left: 1px solid rgba(255, 255, 255, 0);
  border-right: 1px solid rgba(255, 255, 255, 0);
`;

const IconWrapper = styled.div`
  position: relative;
  width: 32px;
  height: 100%;
  padding: 8px;
  box-sizing: border-box;
  border: 1px solid;

  border-color: ${(props) =>
    props.isDisabled
      ? props.theme.viewSelector.disabledFillColor
      : props.isChecked
      ? props.theme.viewSelector.checkedFillColor
      : props.theme.viewSelector.borderColor};

  ${(props) => !props.firstItem && !props.lastItem && middleItemsStyle}
  ${(props) => props.firstItem && firstItemStyle}
  ${(props) => props.lastItem && lastItemStyle}

  background-color: ${(props) =>
    props.isChecked
      ? props.isDisabled
        ? props.theme.viewSelector.disabledFillColor
        : props.theme.viewSelector.checkedFillColor
      : props.isDisabled
      ? props.theme.viewSelector.fillColorDisabled
      : props.theme.viewSelector.fillColor};

  &:hover {
    ${(props) =>
      props.isDisabled || props.isChecked
        ? css`
            cursor: default;
          `
        : css`
            cursor: pointer;
            border: 1px solid
              ${(props) => props.theme.viewSelector.hoverBorderColor};
          `}
  }
  svg {
    width: 15px;
    height: 15px;

    ${(props) =>
      !props.isDisabled
        ? !props.isChecked
          ? css`
              path {
                fill: ${(props) => props.theme.viewSelector.checkedFillColor};
              }
            `
          : css`
              path {
                fill: ${(props) => props.theme.viewSelector.fillColor};
              }
            `
        : css`
            path {
              fill: ${(props) =>
                props.theme.viewSelector.disabledFillColorInner};
            }
          `};
  }
`;

IconWrapper.defaultProps = { theme: Base };

StyledViewSelector.defaultProps = { theme: Base };

export { StyledViewSelector, IconWrapper };
