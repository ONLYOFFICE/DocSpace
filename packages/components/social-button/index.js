import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import Text from "../text";
import StyledSocialButton from "./styled-social-button";
import { ReactSVG } from "react-svg";
// eslint-disable-next-line no-unused-vars

class SocialButton extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const { label, iconName, IconComponent, isConnect, ...otherProps } =
      this.props;
    return (
      <StyledSocialButton isConnect={isConnect} {...otherProps}>
        <div>
          {IconComponent ? (
            <IconComponent className="iconWrapper" />
          ) : (
            <ReactSVG className="iconWrapper" src={iconName} />
          )}
        </div>
        <div>
          {label && (
            <Text as="span" className="social_button_text">
              {label}
            </Text>
          )}
        </div>
      </StyledSocialButton>
    );
  }
}

SocialButton.propTypes = {
  /** Button text */
  label: PropTypes.string,
  /** Button icon */
  iconName: PropTypes.string,
  /** Accepts tabindex prop */
  tabIndex: PropTypes.number,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets a callback function that is triggered when the button is clicked */
  onClick: PropTypes.func,
  /** Accepts the icon options  */
  $iconOptions: PropTypes.object,
  /** Sets the image size. Contains the small and the basic size options */
  size: PropTypes.oneOf(["base", "small"]),
  /** Changes the button style if the user is connected to the social network account */
  isConnect: PropTypes.bool,
};

SocialButton.defaultProps = {
  label: "",
  iconName: "SocialButtonGoogleIcon",
  tabIndex: -1,
  isDisabled: false,
  $iconOptions: {},
  size: "base",
  isConnect: false,
};

export default SocialButton;
