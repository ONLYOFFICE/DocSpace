import React from "react";
import styled, { css } from "styled-components";
import NoUserSelect from "../../utils/commonStyles";
import { Base } from "../../themes";

// eslint-disable-next-line react/prop-types, no-unused-vars
const ClearLabel = ({ spacing, isDisabled, orientation, ...props }) => (
  <label {...props} />
);

const Label = styled(ClearLabel)`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;

  ${NoUserSelect};

  cursor: ${(props) => !props.isDisabled && "pointer"};

  .radio-button {
    margin-right: ${(props) => props.theme.radioButton.marginRight};

    path {
      fill: ${(props) =>
        props.isDisabled && props.theme.radioButton.disableColor};
    }
  }
  .radio-button_text {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.radioButton.textDisableColor
        : props.theme.radioButton.textColor};
  }

  ${(props) =>
    props.isDisabled
      ? css`
          cursor: default;
          path:first-child {
            stroke: ${props.theme.radioButton.disableBorderColor};
          }
          path:nth-child(even) {
            fill: ${props.theme.radioButton.disableFillColor};
          }
        `
      : css`
          cursor: pointer;

          &:hover {
            svg {
              path:first-child {
                stroke: ${props.theme.radioButton.hoverBorderColor};
              }
            }
          }
        `}

  &:not(:first-child) {
    ${(props) =>
      props.orientation === "horizontal" &&
      css`
        margin-left: ${props.spacing};
      `};
  }

  &:not(:last-child) {
    ${(props) =>
      props.orientation === "vertical" &&
      css`
        margin-bottom: ${props.spacing};
      `};
  }
`;
Label.defaultProps = { theme: Base };
const Input = styled.input`
  position: absolute;
  z-index: -1;
  opacity: 0.0001;
`;

export { Label, Input };
