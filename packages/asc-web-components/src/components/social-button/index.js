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
    const { label, iconName } = this.props;
    return (
      <StyledSocialButton {...this.props}>
        <ReactSVG src={iconName} />
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
