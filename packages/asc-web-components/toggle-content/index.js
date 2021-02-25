import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { ArrowContentIcon } from "./svg";
import Heading from "../heading";
import { StyledContent, StyledContainer } from "./styled-toggle-content";
import commonIconsStyles from "../utils/common-icons-style";

const StyledArrowContentIcon = styled(ArrowContentIcon)`
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

  toggleContent = () => this.setState({ isOpen: !this.state.isOpen });

  componentDidUpdate(prevProps) {
    const { isOpen } = this.props;
    if (isOpen !== prevProps.isOpen) {
      this.setState({ isOpen });
    }
  }

  render() {
    // console.log("ToggleContent render");

    const { children, className, id, label, style } = this.props;

    const { isOpen } = this.state;

    return (
      <StyledContainer
        className={className}
        isOpen={isOpen}
        id={id}
        style={style}
      >
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
        <StyledContent isOpen={isOpen}>{children}</StyledContent>
      </StyledContainer>
    );
  }
}

ToggleContent.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  isOpen: PropTypes.bool,
  label: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

ToggleContent.defaultProps = {
  isOpen: false,
  label: "",
};

export default ToggleContent;
