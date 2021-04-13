import React from "react";
import styled, { css } from "styled-components";
import Base from "../themes/base";
import PropTypes from "prop-types";

const ButtonWrapper = ({ label, iconName, isDisabled, ...props }) => (
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
  background-color: #ffffff;
  border: 1px solid #1877f2;
  border-radius: 3px;
  width: 100%;
  cursor: pointer;
  padding: 0;
  outline: none;

  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;

  ${(props) =>
    props.$iconOptions &&
    props.$iconOptions.color &&
    css`
      svg {
        margin: 6px;
        width: 26px;
        height: 26px;

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
    font-weight: 700;
    font-size: 14px;
    line-height: 14px;
    color: #1877f2;
    margin: 0 11px;
  }
`;

StyledFacebookButton.defaultProps = { theme: Base };

export default StyledFacebookButton;
