import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Text from "../text";
import equal from "fast-deep-equal/react";
import { Icons } from "../icons";

// eslint-disable-next-line no-unused-vars
const ButtonWrapper = ({ label, iconName, isDisabled, ...props }) => (
  <button type="button" {...props}></button>
);

ButtonWrapper.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func,
};

const StyledSocialButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  font-family: "Open Sans", sans-serif;
  border: none;
  display: inline-block;
  font-weight: 600;
  text-decoration: none;
  margin: 20px 0 0 20px;
  padding: 0;
  border-radius: 2px;
  width: 201px;
  height: 40px;
  text-align: left;
  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;
  stroke: none;

  &:focus {
    outline: none;
  }

  ${(props) =>
    !props.isDisabled
      ? css`
          background: #ffffff;
          box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.24),
            0px 0px 1px rgba(0, 0, 0, 0.12);
          color: rgba(0, 0, 0, 0.54);

          :hover,
          :active {
            cursor: pointer;
            box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.24),
              0px 0px 2px rgba(0, 0, 0, 0.12);
          }

          :hover {
            background: #ffffff;
          }

          :active {
            background: #eeeeee;
            border: none;
          }
        `
      : css`
          box-shadow: none;
          background: rgba(0, 0, 0, 0.08);
          color: rgba(0, 0, 0, 0.4);

          svg path {
            fill: rgba(0, 0, 0, 0.4);
          }
        `};

  .social_button_text {
    position: absolute;
    width: 142px;
    height: 16px;
    margin: 12px 9px 12px 10px;
    font-family: Roboto, "Open Sans", sans-serif, Arial;
    font-style: normal;
    font-weight: 500;
    font-size: 14px;
    line-height: 16px;
    letter-spacing: 0.21875px;
    user-select: none;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  svg {
    margin: 11px;
    width: 18px;
    height: 18px;
    min-width: 18px;
    min-height: 18px;
  }
`;

class SocialButton extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const { label, iconName } = this.props;
    return (
      <StyledSocialButton {...this.props}>
        {React.createElement(Icons[iconName], {})}

        {label && (
          <Text as="span" className="social_button_text">
            {label}
          </Text>
        )}
      </StyledSocialButton>
    );
  }
}

SocialButton.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SocialButton.defaultProps = {
  label: "",
  iconName: "SocialButtonGoogleIcon",
  tabIndex: -1,
  isDisabled: false,
};

export default SocialButton;
