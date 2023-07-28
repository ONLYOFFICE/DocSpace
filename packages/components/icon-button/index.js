import React from "react";
import PropTypes from "prop-types";

import isEmpty from "lodash/isEmpty";
import StyledOuter from "./styled-icon-button";
import { ReactSVG } from "react-svg";

class IconButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      currentIconName: this.props.iconName,
      currentIconColor: this.props.color,
    };
    this.onMouseEnter = this.onMouseEnter.bind(this);
    this.onMouseLeave = this.onMouseLeave.bind(this);
    this.onMouseDown = this.onMouseDown.bind(this);
    this.onMouseUp = this.onMouseUp.bind(this);

    this.isNeedUpdate = false;
  }

  onMouseEnter(e) {
    const {
      isDisabled,
      iconHoverName,
      iconName,
      hoverColor,
      color,
      onMouseEnter,
    } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: !("ontouchstart" in document.documentElement)
        ? iconHoverName || iconName
        : iconName,
      currentIconColor: hoverColor || color,
    });

    onMouseEnter && onMouseEnter(e);
  }
  onMouseLeave(e) {
    const { isDisabled, iconName, color, onMouseLeave } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: iconName,
      currentIconColor: color,
    });

    onMouseLeave && onMouseLeave(e);
  }
  onMouseDown(e) {
    const {
      isDisabled,
      iconClickName,
      iconName,
      clickColor,
      color,
      onMouseDown,
    } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: !("ontouchstart" in document.documentElement)
        ? iconClickName || iconName
        : iconName,
      currentIconColor: clickColor || color,
    });

    onMouseDown && onMouseDown(e);
  }
  onMouseUp(e) {
    const { isDisabled, iconHoverName, iconName, color, onMouseUp } =
      this.props;

    if (isDisabled) return;

    switch (e.nativeEvent.which) {
      case 1: //Left click
        this.setState({
          currentIconName: !("ontouchstart" in document.documentElement)
            ? iconHoverName || iconName
            : iconName,
          currentIconColor: iconHoverName || color,
        });

        onMouseUp && onMouseUp(e);
        break;
      case 3: //Right click
        onMouseUp && onMouseUp(e);
        break;
      default:
        break;
    }
  }

  onClick = (e) => {
    const { onClick, isDisabled } = this.props;
    if (isDisabled) return;
    onClick && onClick(e);
  };

  componentDidUpdate(prevProps) {
    const { iconName, color } = this.props;

    let newState = {};

    if (iconName !== prevProps.iconName) {
      newState.currentIconName = iconName;
    }
    if (color !== prevProps.color) {
      newState.currentIconColor = color;
    }

    if (!isEmpty(newState)) this.setState(newState);
  }
  render() {
    //console.log("IconButton render", this.state.currentIconColor);
    const {
      className,
      size,
      isDisabled,
      isFill,
      isClickable,
      onClick,
      id,
      style,
      dataTip,
      title,
      theme,
      color,
      hoverColor,
      iconNode,
      ...rest
    } = this.props;

    return (
      <StyledOuter
        className={className}
        size={size}
        title={title}
        isDisabled={isDisabled}
        onMouseEnter={this.onMouseEnter}
        onMouseLeave={this.onMouseLeave}
        onMouseDown={this.onMouseDown}
        onMouseUp={this.onMouseUp}
        onClick={this.onClick}
        isClickable={typeof onClick === "function" || isClickable}
        data-tip={dataTip}
        data-event="click focus"
        data-for={id}
        id={id}
        style={style}
        color={this.state.currentIconColor}
        isFill={isFill}
        {...rest}
      >
        {/* {React.createElement(Icons["CalendarIcon"], {
          size: "scale",
          color: this.state.currentIconColor,
          isfill: isFill,
        })} */}

        {iconNode ? (
          iconNode
        ) : (
          <ReactSVG
            className="icon-button_svg not-selectable"
            src={this.state.currentIconName}
          />
        )}
      </StyledOuter>
    );
  }
}

IconButton.propTypes = {
  /** Sets component class */
  className: PropTypes.string,
  /** Icon color */
  color: PropTypes.string,
  /** Icon color on hover action */
  hoverColor: PropTypes.string,
  /** Icon color on click action */
  clickColor: PropTypes.string,
  /** Button height and width value */
  size: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Determines if icon fill is needed */
  isFill: PropTypes.bool,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Sets cursor value */
  isClickable: PropTypes.bool,
  /** Icon node */
  iconNode: PropTypes.node,
  /** Icon name */
  iconName: PropTypes.string,
  /** Icon name on hover action */
  iconHoverName: PropTypes.string,
  /** Icon name on click action */
  iconClickName: PropTypes.string,
  /** Sets a button callback function triggered when the button is clicked */
  onClick: PropTypes.func,
  /** Sets a button callback function triggered when the cursor enters the area */
  onMouseEnter: PropTypes.func,
  /** Sets a button callback function triggered when the cursor moves down */
  onMouseDown: PropTypes.func,
  /** Sets a button callback function triggered when the cursor moves up */
  onMouseUp: PropTypes.func,
  /** Sets a button callback function triggered when the cursor leaves the icon */
  onMouseLeave: PropTypes.func,
  /** Sets component id */
  id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** The data-* attribute is used to store custom data private to the page or application. Required to display a tip over the hovered element */
  dataTip: PropTypes.string,
};

IconButton.defaultProps = {
  size: 25,
  isFill: true,
  isDisabled: false,
  isClickable: false,
  dataTip: "",
};

export default IconButton;
