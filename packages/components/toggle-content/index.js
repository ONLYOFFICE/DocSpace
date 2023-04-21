import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { ArrowContentReactSvg } from "./svg";
import Heading from "../heading";
import { StyledContent, StyledContainer } from "./styled-toggle-content";
import commonIconsStyles from "../utils/common-icons-style";

const StyledArrowContentIcon = styled(ArrowContentReactSvg)`
  ${commonIconsStyles}
`;
// eslint-disable-next-line react/prop-types, no-unused-vars
class ToggleContent extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen } = props;

    this.state = {
      isOpen,
    };
  }

  toggleContent = () => {
    if (!this.props.enableToggle) return;

    this.setState({ isOpen: !this.state.isOpen });
  };

  componentDidUpdate(prevProps) {
    const { isOpen } = this.props;
    if (isOpen !== prevProps.isOpen) {
      this.setState({ isOpen });
    }
  }

  render() {
    // console.log("ToggleContent render");

    const { children, className, id, label, style, enableToggle } = this.props;

    const { isOpen } = this.state;

    return (
      <StyledContainer
        className={className}
        isOpen={isOpen}
        id={id}
        style={style}
        enableToggle={enableToggle}
      >
        <div className="toggle-container">
          <span className="span-toggle-content" onClick={this.toggleContent}>
            <StyledArrowContentIcon
              className="arrow-toggle-content"
              size="medium"
            />
            <Heading
              className="heading-toggle-content"
              level={2}
              size="small"
              isInline={true}
            >
              {label}
            </Heading>
          </span>
        </div>
        <StyledContent isOpen={isOpen}>{children}</StyledContent>
      </StyledContainer>
    );
  }
}

ToggleContent.propTypes = {
  /** Displays the child elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id  */
  id: PropTypes.string,
  /** Displays the component's state */
  isOpen: PropTypes.bool,
  /** Sets the header label */
  label: PropTypes.string.isRequired,
  /** The change event is triggered when the element's value is modified */
  onChange: PropTypes.func,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Enables/disables toggle */
  enableToggle: PropTypes.bool,
};

ToggleContent.defaultProps = {
  isOpen: false,
  enableToggle: true,
  label: "",
};

export default ToggleContent;
