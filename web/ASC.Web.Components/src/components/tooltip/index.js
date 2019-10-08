import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import ReactTooltip from "react-tooltip";

const TooltipStyle = styled.div`
  .__react_component_tooltip {
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    opacity: 1;
    padding: 16px;

    max-width: ${props => (props.maxWidth ? props.maxWidth + "px" : `340px`)};

    &:before {
      border: none;
    }
    &:after {
      border: none;
    }
  }
`;

class Tooltip extends Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    const { effect, place, maxWidth, id, getContent, offset } = this.props;

    return (
      <TooltipStyle maxWidth={maxWidth}>
        <ReactTooltip
          id={id}
          getContent={getContent}
          type="light"
          effect={effect}
          place={place}
          globalEventOff="click"
          offset={offset}
          wrapper="span"
        />
      </TooltipStyle>
    );
  }
}

Tooltip.propTypes = {
  id: PropTypes.string,
  effect: PropTypes.oneOf(["float", "solid"]),
  place: PropTypes.oneOf(["top", "right", "bottom", "left"]),
  maxWidth: PropTypes.number,
  offset: PropTypes.object,
  getContent: PropTypes.func
};

Tooltip.defaultProps = {
  effect: "float",
  place: "top",
  offset: { right: 70 }
};

export default Tooltip;
