import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Text from "../text";
import ExpanderDownIcon from "../../../public/images/expander-down.react.svg";
const StyledLinkWrapper = styled.span`
  position: relative;

  .fixed-max-width {
    max-width: 260px;
  }
`;

const StyledLink = styled.a`
  cursor: pointer;
  user-select: none;
  padding-right: 20px;
  position: relative;
  display: inline-grid;
  color: ${(props) => props.color};
  text-decoration: underline dashed;

  .progress-bar_icon {
    position: absolute;
    width: 8px;
    min-width: 8px;
    height: 8px;
    right: 6px;
    top: 0;
    bottom: 0;
    margin: auto;

    path {
      fill: ${(props) => props.color};
      stroke: ${(props) => props.color};
    }

    ${(props) =>
      props.isOpen &&
      css`
        bottom: -1px;
        transform: scale(1, -1);
      `}
  }
`;

class Link extends React.Component {
  render() {
    const { color, children, isOpen, showIcon, ...rest } = this.props;

    //console.log("ProgressBar link render");
    return (
      <StyledLinkWrapper {...rest}>
        <StyledLink color={color} isOpen={isOpen}>
          <Text color={color}>{children}</Text>
          {showIcon && <ExpanderDownIcon className="progress-bar_icon" />}
        </StyledLink>
      </StyledLinkWrapper>
    );
  }
}

Link.propTypes = {
  children: PropTypes.any,
  color: PropTypes.string,
  isOpen: PropTypes.bool,
  showIcon: PropTypes.bool,
};

Link.displayName = "Link";

export default Link;
