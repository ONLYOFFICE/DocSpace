import React, { Component } from "react";
import PropTypes from "prop-types";
import ReactTooltip from "react-tooltip";
import Portal from "../portal";
import StyledTooltip from "./styled-tooltip";

class Tooltip extends Component {
  constructor(props) {
    super(props);
  }

  componentDidUpdate() {
    ReactTooltip.rebuild();
  }

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
      color,
      maxWidth,
    } = this.props;

    const renderTooltip = () => (
      <StyledTooltip
        className={className}
        style={style}
        color={color}
        maxWidth={maxWidth}
      >
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
        >
          {children}
        </ReactTooltip>
      </StyledTooltip>
    );

    const tooltip = renderTooltip();

    return <Portal element={tooltip} />;
  }
}

Tooltip.propTypes = {
  /** Used as HTML id property  */
  id: PropTypes.string,
  /** Behavior of tooltip */
  effect: PropTypes.oneOf(["float", "solid"]),
  /** Global tooltip placement */
  place: PropTypes.oneOf(["top", "right", "bottom", "left"]),
  /** Generate the tip content dynamically */
  getContent: PropTypes.func,
  afterHide: PropTypes.func,
  afterShow: PropTypes.func,
  /** Offset top all tooltips on page */
  offsetTop: PropTypes.number,
  /** Offset right all tooltips on page */
  offsetRight: PropTypes.number,
  /** Offset bottom all tooltips on page */
  offsetBottom: PropTypes.number,
  /** Offset left all tooltips on page */
  offsetLeft: PropTypes.number,
  /** Child elements */
  children: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  reference: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.shape({ current: PropTypes.any }),
  ]),
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  color: PropTypes.string,
  maxWidth: PropTypes.string,
};

Tooltip.defaultProps = {
  effect: "float",
  place: "top",
  offsetTop: 0,
  offsetRight: 0,
  offsetBottom: 0,
  offsetLeft: 0,
  color: "#f8f7bf",
};

export default Tooltip;
