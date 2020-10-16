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
    pointer-events: auto;
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
  }

  componentDidUpdate() {
    ReactTooltip.rebuild();
  }

  overridePosition = ({ left, top }) => {
    const d = document.documentElement;
    left = Math.min(d.clientWidth - 340, left);
    top = Math.min(d.clientHeight - 0, top);
    left = Math.max(0, left);
    top = Math.max(0, top);
    //console.log("left:", left, "top:", top);
    return { top, left };
  };

  render() {
    const {
      effect,
      place,
      id,
      getContent,
      offsetTop,
      offsetRight,
      offsetBottom,
      offsetLeft,
      children,
      afterShow,
      afterHide,
      reference,
      className,
      style,
    } = this.props;

    return (
      <TooltipStyle className={className} style={style}>
        <ReactTooltip
          id={id}
          ref={reference}
          getContent={getContent}
          type="light"
          effect={effect}
          place={place}
          offset={{
            top: offsetTop,
            right: offsetRight,
            bottom: offsetBottom,
            left: offsetLeft,
          }}
          wrapper="div"
          afterShow={afterShow}
          afterHide={afterHide}
          isCapture={true}
          overridePosition={this.overridePosition}
        >
          {children}
        </ReactTooltip>
      </TooltipStyle>
    );
  }
}

Tooltip.propTypes = {
  id: PropTypes.string,
  effect: PropTypes.oneOf(["float", "solid"]),
  place: PropTypes.oneOf(["top", "right", "bottom", "left"]),
  getContent: PropTypes.func,
  afterHide: PropTypes.func,
  afterShow: PropTypes.func,
  offsetTop: PropTypes.number,
  offsetRight: PropTypes.number,
  offsetBottom: PropTypes.number,
  offsetLeft: PropTypes.number,
  children: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  reference: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.shape({ current: PropTypes.any }),
  ]),
  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Tooltip.defaultProps = {
  effect: "float",
  place: "top",
  offsetTop: 0,
  offsetRight: 0,
  offsetBottom: 0,
  offsetLeft: 0,
};

export default Tooltip;
