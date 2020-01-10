import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "../icons";
import isEmpty from "lodash/isEmpty";

const StyledOuter = styled.div`
  width: ${props =>
    props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
  cursor: ${props =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};
    line-height: 0;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;
class IconButton extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      currentIconName: this.props.iconName,
      currentIconColor: this.props.color
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
      onMouseEnter
    } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: !('ontouchstart' in document.documentElement) ? iconHoverName || iconName : iconName,
      currentIconColor: hoverColor || color
    });

    onMouseEnter && onMouseEnter(e);
  }
  onMouseLeave(e) {
    const { isDisabled, iconName, color, onMouseLeave } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: iconName,
      currentIconColor: color
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
      onMouseDown
    } = this.props;

    if (isDisabled) return;

    this.setState({
      currentIconName: !('ontouchstart' in document.documentElement) ? iconClickName || iconName : iconName,
      currentIconColor: clickColor || color
    });

    onMouseDown && onMouseDown(e);
  }
  onMouseUp(e) {
    const {
      isDisabled,
      iconHoverName,
      iconName,
      color,
      onClick,
      onMouseUp
    } = this.props;

    if (isDisabled) return;

    switch (e.nativeEvent.which) {
      case 1: //Left click
        this.setState({
          currentIconName: !('ontouchstart' in document.documentElement) ? iconHoverName || iconName : iconName,
          currentIconColor: iconHoverName || color
        });

        onClick && onClick(e);
        onMouseUp && onMouseUp(e);
        break;
      case 3: //Right click
        onMouseUp && onMouseUp(e);
        break;
      default:
        break;
    }
  }
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
    //console.log("IconButton render");
    const {
      className,
      size,
      isDisabled,
      isFill,
      isClickable,
      onClick,
      id,
      style,
      dataTip
    } = this.props;

    return (
      <StyledOuter
        className={className}
        size={size}
        isDisabled={isDisabled}
        onMouseEnter={this.onMouseEnter}
        onMouseLeave={this.onMouseLeave}
        onMouseDown={this.onMouseDown}
        onMouseUp={this.onMouseUp}
        isClickable={typeof onClick === "function" || isClickable}
        data-tip={dataTip}
        data-event="click focus"
        data-for={id}
        style={style}
        //{...this.props}
      >
        {React.createElement(Icons[this.state.currentIconName], {
          size: "scale",
          color: this.state.currentIconColor,
          isfill: isFill
        })}
      </StyledOuter>
    );
  }
}

IconButton.propTypes = {
  className: PropTypes.string,
  color: PropTypes.string,
  hoverColor: PropTypes.string,
  clickColor: PropTypes.string,
  size: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isFill: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isClickable: PropTypes.bool,
  iconName: PropTypes.string.isRequired,
  iconHoverName: PropTypes.string,
  iconClickName: PropTypes.string,
  onClick: PropTypes.func,
  onMouseEnter: PropTypes.func,
  onMouseDown: PropTypes.func,
  onMouseUp: PropTypes.func,
  onMouseLeave: PropTypes.func,
  id: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  dataTip: PropTypes.string
};

IconButton.defaultProps = {
  color: "#d0d5da",
  size: 25,
  isFill: true,
  iconName: "AZSortingIcon",
  isDisabled: false,
  isClickable: false,
  dataTip: ""
};

export default IconButton;
