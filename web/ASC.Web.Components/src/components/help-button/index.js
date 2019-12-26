import React from "react";
import PropTypes from "prop-types";
import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import { handleAnyClick } from "../../utils/event";
import uniqueId from "lodash/uniqueId";
import Aside from "../aside";
import { desktop } from "../../utils/device";
import Backdrop from "../backdrop";
import Heading from "../heading";
import throttle from "lodash/throttle";
import styled from "styled-components";

const HelpContainer = styled.div``;

const Content = styled.div`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  background-color: #fff;
  padding: 0 16px 16px;

  .header {
    max-width: 500px;
    margin: 0;
    line-height: 56px;
    font-weight: 700;
  }
`;

const HeaderContent = styled.div`
  display: flex;
  align-items: center;
  border-bottom: 1px solid #dee2e6;
`;

const Body = styled.div`
  position: relative;
  padding: 16px 0;
`;

class HelpButton extends React.Component {
  constructor(props) {
    super(props);

    this.state = { isOpen: false, displayType: this.getTypeByWidth() };
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
  };

  afterHide = () => {
    //console.log(`afterHide ${this.props.tooltipId} isOpen=${this.state.isOpen}`, this.ref, e);
    if (this.state.isOpen) {
      this.setState({ isOpen: false }, () => {
        handleAnyClick(false, this.handleClick);
      });
    }
  };

  handleClick = e => {
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
    this.setState({ isOpen: !this.state.isOpen });
  };

  render() {
    const { isOpen, displayType } = this.state;
    const {
      tooltipContent,
      place,
      offsetRight,
      offsetLeft,
      zIndex,
      helpButtonHeaderContent,
      iconName,
      color,
      getContent,
      className,
      dataTip,
      style
    } = this.props;

    return (
      <HelpContainer ref={this.ref} style={style}>
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
        {displayType === "dropdown" ? (
          getContent ? (
            <Tooltip
              id={this.id}
              reference={this.refTooltip}
              effect="solid"
              place={place}
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
              getContent={getContent}
            >
              {tooltipContent}
            </Tooltip>
          )
        ) : (
          <>
            <Backdrop onClick={this.onClose} visible={isOpen} zIndex={zIndex} />
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
      </HelpContainer>
    );
  }
}

HelpButton.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  offsetRight: PropTypes.number,
  tooltipMaxWidth: PropTypes.number,
  tooltipId: PropTypes.string,
  place: PropTypes.string,
  offsetLeft: PropTypes.number,
  zIndex: PropTypes.number,
  displayType: PropTypes.oneOf(["dropdown", "aside", "auto"]),
  helpButtonHeaderContent: PropTypes.string,
  iconName: PropTypes.string,
  color: PropTypes.string,
  dataTip: PropTypes.string,
  getContent: PropTypes.func,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

HelpButton.defaultProps = {
  place: "top",
  offsetRight: 120,
  offsetLeft: 0,
  zIndex: 310,
  displayType: "auto",
  className: "icon-button",
  iconName: "QuestionIcon"
};

export default HelpButton;
