import React from "react";
import styled, { css } from "styled-components";
import Base from "../themes/base";
import PropTypes from "prop-types";

const ButtonWrapper = ({ label, iconName, isDisabled, noHover, ...props }) => (
  <button type="button" {...props}></button>
);

ButtonWrapper.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func,
  $iconOptions: PropTypes.object,
};

const StyledSocialButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  font-family: ${(props) => props.theme.fontFamily};
  border: none;
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  font-weight: ${(props) => props.theme.socialButton.fontWeight};
  text-decoration: ${(props) => props.theme.socialButton.textDecoration};
  margin: ${(props) => props.theme.socialButton.margin};
  padding: ${(props) => props.theme.socialButton.padding};
  border-radius: ${(props) => props.theme.socialButton.borderRadius};
  width: ${(props) => props.theme.socialButton.width};
  height: ${(props) =>
    props.size === "base"
      ? props.theme.socialButton.height
      : props.theme.socialButton.heightSmall};
  text-align: ${(props) => props.theme.socialButton.textAlign};
  border: ${(props) => props.theme.socialButton.border};
  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;

  stroke: ${(props) => props.theme.socialButton.stroke};

  &:focus {
    outline: ${(props) => props.theme.socialButton.outline};
  }

  ${(props) =>
    props.$iconOptions &&
    props.$iconOptions.color &&
    css`
      svg path {
        fill: ${props.$iconOptions.color};
      }
    `}

  ${(props) =>
    !props.isDisabled
      ? css`
          background: ${(props) =>
            props.isConnect
              ? props.theme.socialButton.connectBackground
              : props.theme.socialButton.background};
          box-shadow: ${(props) => props.theme.socialButton.boxShadow};

          ${(props) =>
            !props.noHover &&
            css`
              :hover,
              :active {
                cursor: pointer;
                box-shadow: ${(props) => props.theme.socialButton.boxShadow};

                .social_button_text {
                  color: ${(props) =>
                    !props.isConnect &&
                    props.theme.socialButton.text.hoverColor};
                }
              }

              :hover {
                background: ${(props) =>
                  props.isConnect
                    ? props.theme.socialButton.hoverConnectBackground
                    : props.theme.socialButton.hoverBackground};
              }

              :active {
                background: ${(props) =>
                  props.theme.socialButton.activeBackground};
                border: none;
              }
            `}
        `
      : css`
          box-shadow: none;
          background: ${(props) =>
            props.theme.socialButton.disableBackgroundColor};
          color: ${(props) => props.theme.socialButton.disableColor};

          svg path {
            fill: ${(props) => props.theme.socialButton.disableColor};
          }
        `};

  .iconWrapper {
    display: flex;
    pointer-events: none;
  }

  .social_button_text {
    position: relative;
    pointer-events: none;
    color: ${(props) =>
      props.isConnect
        ? props.theme.socialButton.text.connectColor
        : props.theme.socialButton.text.color};
    width: ${(props) => props.theme.socialButton.text.width};
    height: ${(props) => props.theme.socialButton.text.height};
    font-family: Roboto, "Open Sans", sans-serif, Arial;
    font-style: normal;
    font-weight: ${(props) => props.theme.socialButton.text.fontWeight};
    font-size: ${(props) => props.theme.socialButton.text.fontSize};
    line-height: ${(props) => props.theme.socialButton.text.lineHeight};
    letter-spacing: ${(props) => props.theme.socialButton.text.letterSpacing};
    user-select: none;
    overflow: ${(props) => props.theme.socialButton.text.overflow};
    text-overflow: ${(props) => props.theme.socialButton.text.textOverflow};
    white-space: ${(props) => props.theme.socialButton.text.whiteSpace};
  }

  svg {
    margin: ${(props) => props.theme.socialButton.svg.margin};
    width: ${(props) => props.theme.socialButton.svg.width};
    height: ${(props) => props.theme.socialButton.svg.height};
    min-width: ${(props) => props.theme.socialButton.svg.minWidth};
    min-height: ${(props) => props.theme.socialButton.svg.minHeight};

    path {
      fill: ${(props) => props.isConnect && props.theme.socialButton.svg.fill};
    }
  }
`;

StyledSocialButton.defaultProps = { theme: Base };

export default StyledSocialButton;
