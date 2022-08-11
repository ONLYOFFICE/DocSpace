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

const StyledFacebookButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  border: none;
  display: flex;
  align-items: center;
  background-color: ${(props) => props.theme.facebookButton.background};
  border: ${(props) => props.theme.facebookButton.border};
  border-radius: 3px;
  width: 100%;
  ${(props) => !props.noHover && "cursor: pointer;"}
  padding: 0;
  outline: none;

  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;

  svg {
    margin: 11px;
    width: 18px;
    height: 18px;
    min-width: 18px;
    min-height: 18px;
  }

  ${(props) =>
    props.$iconOptions &&
    props.$iconOptions.color &&
    css`
      svg {
        path {
          fill: ${props.$iconOptions.color};
        }
      }
    `}

  .iconWrapper {
    display: flex;
    pointer-events: none;
  }

  .social_button_text {
    pointer-events: none;
    font-family: Roboto, "Open Sans", sans-serif, Arial;
    font-style: normal;
    font-weight: 600;
    font-size: 14px;
    line-height: 14px;
    color: ${(props) => props.theme.facebookButton.color};
    margin: 0 11px;
  }
`;

StyledFacebookButton.defaultProps = { theme: Base };

export default StyledFacebookButton;
