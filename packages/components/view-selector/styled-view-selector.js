import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledViewSelector = styled.div`
  height: 32px;
  width: ${(props) =>
    props.isFilter ? `32px` : `calc(${props.countItems} * 32px)`};
  position: relative;
  box-sizing: border-box;
  display: flex;

  ${(props) =>
    props.isFilter
      ? css``
      : props.countItems > 2
      ? css`
          .view-selector-icon:hover {
            z-index: 2;
          }
          .view-selector-icon:not(:first-child) {
            margin-left: -1px;
          }
        `
      : css`
          .view-selector-icon:first-child {
            border-right: none;
          }
          .view-selector-icon:last-child {
            border-left: none;
          }
        `}
`;

const firstItemStyle = css`
  border-top-left-radius: 3px;
  border-bottom-left-radius: 3px;
`;

const lastItemStyle = css`
  border-top-right-radius: 3px;
  border-bottom-right-radius: 3px;
`;

const IconWrapper = styled.div`
  position: relative;
  padding: 7px;
  box-sizing: border-box;
  border: 1px solid;

  ${(props) => props.isChecked && `z-index: 1;`}

  border-color: ${(props) =>
    props.isDisabled
      ? props.theme.viewSelector.disabledFillColor
      : props.isChecked
      ? props.theme.viewSelector.checkedFillColor
      : props.theme.viewSelector.borderColor};

  ${(props) =>
    props.isFilter &&
    css`
      border-radius: 3px;
    `}
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

  & > div {
    width: 16px;
    height: 16px;
  }

  svg {
    width: 16px;
    height: 16px;

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
