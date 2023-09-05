import PropTypes from "prop-types";
import React, { Component } from "react";
import { Tooltip as ReactTooltip } from "react-tooltip";

import Portal from "../portal";
import StyledTooltip from "./styled-tooltip";
import { flip, shift, offset } from "@floating-ui/dom";

const defaultOffset = 10;

class Tooltip extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    const {
      id,
      place,
      getContent,
      children,
      afterShow,
      afterHide,
      className,
      style,
      color,
      maxWidth,
    } = this.props;

    const content = getContent ? getContent() : children;

    const renderTooltip = () => (
      <StyledTooltip
        theme={this.props.theme}
        className={className}
        style={style}
        color={color}
        maxWidth={maxWidth}
      >
        <ReactTooltip
          clickable
          openOnClick
          closeOnScroll
          closeOnResize
          offset={this.props.offset}
          afterShow={afterShow}
          afterHide={afterHide}
          place={place}
          anchorSelect={`div[id='${id}'] svg`}
          className="__react_component_tooltip"
          positionStrategy="fixed"
          middlewares={[
            offset(this.props.offset ?? defaultOffset),
            flip({
              crossAxis: false,
              fallbackAxisSideDirection: place,
            }),
            shift(),
          ]}
        >
          {content}
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
  offset: PropTypes.number,
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
  place: "top",
};

export default Tooltip;
