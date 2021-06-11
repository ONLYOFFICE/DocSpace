import React from "react";
import PropTypes from "prop-types";

import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import { handleAnyClick } from "../utils/event";
import uniqueId from "lodash/uniqueId";
import Aside from "../aside";
import { desktop } from "../utils/device";
import Backdrop from "../backdrop";
import Heading from "../heading";
import throttle from "lodash/throttle";
import { Content, HeaderContent, Body } from "./styled-help-button";

class HelpButton extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
      displayType: this.getTypeByWidth(),
      hideTooltip: false,
    };
    this.ref = React.createRef();
    this.refTooltip = React.createRef();
    this.id = this.props.id || uniqueId();

    this.throttledResize = throttle(this.resize, 300);
  }

  afterShow = () => {
    this.refTooltip.current.updatePosition();
    //console.log(`afterShow ${this.props.tooltipId} isOpen=${this.state.isOpen}`, this.ref, e);
    this.setState({ isOpen: true }, () => {
      handleAnyClick(true, this.handleClick);
    });

    if (this.state.hideTooltip) {
      this.refTooltip.current.hideTooltip();
    }
  };

  afterHide = () => {
    //console.log(`afterHide ${this.props.tooltipId} isOpen=${this.state.isOpen}`, this.ref, e);
    if (this.state.isOpen && !this.state.hideTooltip) {
      this.setState({ isOpen: false }, () => {
        handleAnyClick(false, this.handleClick);
      });
    }
  };

  handleClick = (e) => {
    //console.log(`handleClick ${this.props.tooltipId} isOpen=${this.state.isOpen}`, this.ref, e);

    if (!this.ref.current.contains(e.target)) {
      //console.log(`hideTooltip() tooltipId=${this.props.tooltipId}`, this.refTooltip.current);
      this.refTooltip.current.hideTooltip();
    }
  };

  onClose = () => {
    this.setState({ isOpen: false });
  };

  componentDidMount() {
    window.addEventListener("resize", this.throttledResize);
  }

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
    window.removeEventListener("resize", this.throttledResize);
  }

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType) {
      this.setState({ displayType: this.getTypeByWidth() });
    }
    if (this.state.isOpen && this.state.displayType === "aside") {
      window.addEventListener("popstate", this.popstate, false);
    }
  }

  popstate = () => {
    window.removeEventListener("popstate", this.popstate, false);
    this.onClose();
    window.history.go(1);
  };

  resize = () => {
    if (this.props.displayType !== "auto") return;
    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;
    this.setState({ displayType: type });
  };

  getTypeByWidth = () => {
    if (this.props.displayType !== "auto") return this.props.displayType;
    return window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "dropdown";
  };

  onClick = () => {
    let state = false;
    if (this.state.displayType === "aside") {
      state = true;
    }
    this.setState({ isOpen: !this.state.isOpen, hideTooltip: state });
  };

  render() {
    const { isOpen, displayType } = this.state;
    const {
      tooltipContent,
      place,
      offsetTop,
      offsetBottom,
      offsetRight,
      offsetLeft,
      zIndex,
      helpButtonHeaderContent,
      iconName,
      color,
      getContent,
      className,
      dataTip,
      style,
      tooltipColor,
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
            color={tooltipColor}
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
            color={tooltipColor}
          >
            {tooltipContent}
          </Tooltip>
        )}
        {displayType === "aside" && (
          <>
            <Backdrop
              onClick={this.onClose}
              visible={isOpen}
              zIndex={zIndex}
              isAside={true}
            />
            <Aside visible={isOpen} scale={false} zIndex={zIndex}>
              <Content>
                {helpButtonHeaderContent && (
                  <HeaderContent>
                    <Heading className="header" size="medium" truncate={true}>
                      {helpButtonHeaderContent}
                    </Heading>
                  </HeaderContent>
                )}
                <Body>{tooltipContent}</Body>
              </Content>
            </Aside>
          </>
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
  zIndex: PropTypes.number,
  /** Tooltip display type  */
  displayType: PropTypes.oneOf(["dropdown", "aside", "auto"]),
  /** Tooltip header content (tooltip opened in aside) */
  helpButtonHeaderContent: PropTypes.string,
  iconName: PropTypes.string,
  color: PropTypes.string,
  dataTip: PropTypes.string,
  getContent: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  tooltipColor: PropTypes.string,
};

HelpButton.defaultProps = {
  iconName: "/static/images/question.react.svg",
  place: "top",
  offsetRight: 120,
  offsetLeft: 0,
  offsetTop: 0,
  offsetBottom: 0,
  zIndex: 310,
  displayType: "auto",
  className: "icon-button",
  color: "#A3A9AE",
  tooltipColor: "#fff",
};

export default HelpButton;
