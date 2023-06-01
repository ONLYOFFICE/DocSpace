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
      ...rest
    } = this.props;

    const renderTooltip = () => (
      <StyledTooltip
        theme={this.props.theme}
        className={className}
        style={style}
        color={color}
        maxWidth={maxWidth}
      >
        <ReactTooltip
          theme={this.props.theme}
          id={id}
          ref={reference}
          getContent={getContent}
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
          {...rest}
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
  /** Tooltip behavior */
  effect: PropTypes.oneOf(["float", "solid"]),
  /** Global tooltip placement */
  place: PropTypes.oneOf(["top", "right", "bottom", "left"]),
  /** Sets a callback function that generates the tip content dynamically */
  getContent: PropTypes.func,
  /** A function to be called after the tooltip is hidden */
  afterHide: PropTypes.func,
  /** A function to be called after the tooltip is shown */
  afterShow: PropTypes.func,
  /** Sets the top offset for all the tooltips on the page */
  offsetTop: PropTypes.number,
  /** Sets the right offset for all the tooltips on the page */
  offsetRight: PropTypes.number,
  /** Sets the bottom offset for all the tooltips on the page */
  offsetBottom: PropTypes.number,
  /** Sets the left offset for all the tooltips on the page */
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
  /** Background color of the tooltip  */
  color: PropTypes.string,
  /** Maximum width of the tooltip */
  maxWidth: PropTypes.string,
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
