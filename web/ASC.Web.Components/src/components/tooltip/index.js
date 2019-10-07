import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import ReactTooltip from "react-tooltip";
import { Text } from "../text";

const TooltipStyle = styled.div`
  .__react_component_tooltip {
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);

    max-width: 340px;

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
    const { type, effect, place } = this.props;

    return (
      <TooltipStyle>
        <ReactTooltip
          id="tooltipContent"
          getContent={dataTip => <Text.Body fontSize={13}>{dataTip}</Text.Body>}
          type={type}
          border={true}
          effect={effect}
          place={place}
          globalEventOff="click"
        />
      </TooltipStyle>
    );
  }
}

Tooltip.propTypes = {
  tooltipContent: PropTypes.string,
  effect: PropTypes.oneOf(["float", "solid"]),
  type: PropTypes.oneOf(["success", "warning", "error", "info", "light"]),
  place: PropTypes.oneOf(["top", "right", "bottom", "left"])
};

Tooltip.defaultProps = {
  type: "light",
  effect: "solid",
  place: "top",
  border: true
};

export default Tooltip;
