import React from "react";
import PropTypes from "prop-types";

import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import { handleAnyClick } from "../utils/event";
import uniqueId from "lodash/uniqueId";

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
      style,
    } = this.props;

    return (
      <div ref={this.ref} style={style}>
        <IconButton
          id={this.id}
          className={`${className} help-icon`}
          isClickable={true}
          iconName={iconName}
          size={13}
          color={color}
          data-for={this.id}
          dataTip={dataTip}
          onClick={this.onClick}
        />
        {getContent ? (
          <Tooltip
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
          />
        ) : (
          <Tooltip
            id={this.id}
            reference={this.refTooltip}
            effect="solid"
            place={place}
            offsetRight={offsetRight}
            offsetLeft={offsetLeft}
            afterShow={this.afterShow}
            afterHide={this.afterHide}
          >
            {tooltipContent}
          </Tooltip>
        )}
      </div>
    );
  }
}

HelpButton.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  offsetRight: PropTypes.number,
  offsetLeft: PropTypes.number,
  offsetTop: PropTypes.number,
  offsetBottom: PropTypes.number,
  tooltipMaxWidth: PropTypes.number,
  tooltipId: PropTypes.string,
  place: PropTypes.string,
  iconName: PropTypes.string,
  color: PropTypes.string,
  dataTip: PropTypes.string,
  getContent: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

HelpButton.defaultProps = {
  iconName: "/static/images/question.react.svg",
  place: "top",
  offsetRight: 120,
  offsetLeft: 0,
  offsetTop: 0,
  offsetBottom: 0,
  className: "icon-button",
  color: "#A3A9AE",
};

export default HelpButton;
