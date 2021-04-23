import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import Text from "../text";
import StyledFacebookButton from "./styled-facebook-button";
import { ReactSVG } from "react-svg";
// eslint-disable-next-line no-unused-vars

class FacebookButton extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const { label, iconName, ...otherProps } = this.props;
    return (
      <StyledFacebookButton {...otherProps}>
        <ReactSVG className="iconWrapper" src={iconName} />
        {label && (
          <Text as="span" className="social_button_text">
            {label}
          </Text>
        )}
      </StyledFacebookButton>
    );
  }
}

FacebookButton.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  onClick: PropTypes.func,
  $iconOptions: PropTypes.object,
};

FacebookButton.defaultProps = {
  tabIndex: -1,
  isDisabled: false,
  $iconOptions: {},
};

export default FacebookButton;
