import React from "react";
import PropTypes from "prop-types";
import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import { handleAnyClick } from "../utils/event";
import uniqueId from "lodash/uniqueId";

import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";

class HelpButton extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      hideTooltip: false,
    };
    this.ref = React.createRef();
    this.refTooltip = React.createRef();
    this.id = this.props.id || uniqueId();
  }

  afterShow = () => {
    this.refTooltip.current.updatePosition();
    handleAnyClick(true, this.handleClick);

    if (this.state.hideTooltip) {
      this.refTooltip.current.hideTooltip();
    }
  };

  afterHide = () => {
    if (!this.state.hideTooltip) {
      handleAnyClick(false, this.handleClick);
    }
  };

  handleClick = (e) => {
    if (!this.ref.current.contains(e.target)) {
      this.refTooltip.current.hideTooltip();
    }
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  onClick = () => {
    this.setState({ hideTooltip: false });
  };

  render() {
    const {
      tooltipContent,
      tooltipProps,
      place,
      offsetTop,
      offsetBottom,
      offsetRight,
      offsetLeft,
      iconName,
      color,
      getContent,
      className,
      dataTip,
      tooltipMaxWidth,
      style,
      size,
    } = this.props;

    return (
      <div ref={this.ref} style={style}>
        <IconButton
          theme={this.props.theme}
          id={this.id}
          className={`${className} help-icon`}
          isClickable={true}
          iconName={iconName}
          size={size}
          color={color}
          data-for={this.id}
          dataTip={dataTip}
          onClick={this.onClick}
        />
        {getContent ? (
          <Tooltip
            tooltipProps={tooltipProps}
            theme={this.props.theme}
            id={this.id}
            reference={this.refTooltip}
            effect="solid"
            place={place}
            offsetTop={offsetTop}
            offsetBottom={offsetBottom}
            offsetRight={offsetRight}
            offsetLeft={offsetLeft}
            afterShow={this.afterShow}
            afterHide={this.afterHide}
            getContent={getContent}
            maxWidth={tooltipMaxWidth}
            {...tooltipProps}
          />
        ) : (
          <Tooltip
            theme={this.props.theme}
            id={this.id}
            reference={this.refTooltip}
            effect="solid"
            place={place}
            offsetRight={offsetRight}
            offsetLeft={offsetLeft}
            afterShow={this.afterShow}
            afterHide={this.afterHide}
            maxWidth={tooltipMaxWidth}
            {...tooltipProps}
          >
            {tooltipContent}
          </Tooltip>
        )}
      </div>
    );
  }
}

HelpButton.propTypes = {
  /** Displays the child elements  */
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  /** Sets the tooltip content  */
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  /** Required to set additional properties of the tooltip */
  tooltipProps: PropTypes.object,
  /** Sets the right offset for all the tooltips on the page */
  offsetRight: PropTypes.number,
  /** Sets the left offset for all the tooltips on the page */
  offsetLeft: PropTypes.number,
  /** Sets the top offset for all the tooltips on the page */
  offsetTop: PropTypes.number,
  /** Sets the bottom offset for all the tooltips on the page */
  offsetBottom: PropTypes.number,
  /** Sets the maximum width of the tooltip  */
  tooltipMaxWidth: PropTypes.string,
  /** Sets the tooltip id */
  tooltipId: PropTypes.string,
  /** Global tooltip placement */
  place: PropTypes.string,
  /** Specifies the icon name */
  iconName: PropTypes.string,
  /** Icon color */
  color: PropTypes.string,
  /** The data-* attribute is used to store custom data private to the page or application. Required to display a tip over the hovered element */
  dataTip: PropTypes.string,
  /** Sets a callback function that generates the tip content dynamically */
  getContent: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Button height and width value */
  size: PropTypes.number,
};

HelpButton.defaultProps = {
  iconName: InfoReactSvgUrl,
  place: "top",
  offsetRight: 60,
  offsetLeft: 0,
  offsetTop: 0,
  offsetBottom: 0,
  className: "icon-button",
  size: 12,
};

export default HelpButton;
