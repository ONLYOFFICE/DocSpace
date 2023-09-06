import PropTypes from "prop-types";
import React from "react";
import { Tooltip as ReactTooltip } from "react-tooltip";

import Portal from "../portal";
import StyledTooltip from "./styled-tooltip";
import { flip, shift, offset } from "@floating-ui/dom";

const defaultOffset = 10;
const Tooltip = (props) => {
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
    anchorSelect,
    clickable,
    openOnClick,
    isOpen,
    float,
  } = props;

  console.log({ getContent, children, isOpen });

  const renderTooltip = () => (
    <StyledTooltip
      theme={props.theme}
      className={className}
      style={style}
      color={color}
      maxWidth={maxWidth}
    >
      <ReactTooltip
        id={id}
        place={place}
        isOpen={isOpen}
        float={float}
        closeOnScroll
        closeOnResize
        render={getContent}
        clickable={clickable}
        afterShow={afterShow}
        afterHide={afterHide}
        positionStrategy="fixed"
        openOnClick={openOnClick}
        offset={props.offset}
        anchorSelect={anchorSelect}
        className="__react_component_tooltip"
        middlewares={[
          offset(props.offset ?? defaultOffset),
          flip({
            crossAxis: false,
            fallbackAxisSideDirection: place,
          }),
          shift(),
        ]}
      >
        {children}
      </ReactTooltip>
    </StyledTooltip>
  );

  const tooltip = renderTooltip();

  return <Portal element={tooltip} />;
};

Tooltip.propTypes = {
  /** Used as HTML id property  */
  id: PropTypes.string,
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
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Background color of the tooltip  */
  color: PropTypes.string,
  /** Maximum width of the tooltip */
  maxWidth: PropTypes.string,

  isOpen: PropTypes.bool,
  clickable: PropTypes.bool,
  openOnClick: PropTypes.bool,
  float: PropTypes.bool,
  anchorSelect: PropTypes.string,
};

Tooltip.defaultProps = {
  place: "top",
};

export default Tooltip;
