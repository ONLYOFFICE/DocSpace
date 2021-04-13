import React from "react";
import styled from "styled-components";
import { isMobile } from "react-device-detect";

const StyledFrame = styled.div`
  .selectFrame {
    user-select: auto;
    display: block;
    position: absolute;
    line-height: 0;
    border: 1px dotted #5c6a8e;
    background-color: #6582c9;
    z-index: 200;
    visibility: hidden;
    opacity: 0.4;
  }
`;

class SelectedFrame extends React.Component {
  state = {
    mouseDown: false,
    top: 0,
    left: 0,
  };

  refFrame = React.createRef();

  componentDidMount() {
    document.addEventListener("mousedown", this.onMouseDown);
    window.addEventListener("mouseup", this.onMouseUp);
  }

  componentWillUnmount() {
    document.removeEventListener("mousedown", this.onMouseDown);
    window.removeEventListener("mouseup", this.onMouseUp);
  }

  getCoords = (e) => {
    const offsetScroll = 0;
    const posX = e.pageX;
    const posY = e.pageY + offsetScroll;
    return [posY, posX];
  };

  onMouseDown = (e) => {
    const mouseButton = e.which
      ? e.which !== 1
      : e.button
      ? e.button !== 0
      : false;

    const path = e.composedPath();
    const isDivTag = e.target.tagName === "DIV";
    const notSelectable = e.target.classList.contains("not-selectable");
    const draggable = e.target.classList.contains("draggable");
    const isBackdrop = e.target.classList.contains("backdrop-active");
    const notSelectablePath = path.some(
      (x) => x.classList && x.classList.contains("not-selectable")
    );

    if (
      mouseButton ||
      isMobile ||
      !isDivTag ||
      notSelectable ||
      draggable ||
      isBackdrop ||
      notSelectablePath
    )
      return;

    const mouseYX = this.getCoords(e);
    const top = mouseYX[0];
    const left = mouseYX[1];
    document.addEventListener("mousemove", this.onMouseMove, false);
    this.setState({ mouseDown: true, top, left });
  };

  setFramePosition = (mouseYX) => {
    const { top, left } = this.state;

    const frame = this.refFrame.current;

    const nextTop = mouseYX[0];
    const nextLeft = mouseYX[1];

    const height = top - nextTop;
    const styledHeight = height < 0 ? nextTop - top : height;
    const styledTop = height < 0 ? top : nextTop;

    frame.style.top = `${styledTop}px`;
    frame.style.height = `${styledHeight}px`;

    const width = left - nextLeft;
    const styledLeft = width < 0 ? left : nextLeft;
    const styledWidth = width < 0 ? nextLeft - left : width;

    frame.style.left = `${styledLeft}px`;
    frame.style.width = `${styledWidth}px`;
  };

  onMouseMove = (e) => {
    const { mouseDown } = this.state;

    if (mouseDown) {
      this.refFrame.current.style.visibility = "visible";
      this.refFrame.current.style.display = "block";
      const mouseYX = this.getCoords(e);
      this.setFramePosition(mouseYX);
    }
  };

  onMouseUp = (e) => {
    const mouseButton = e.which
      ? e.which !== 1
      : e.button
      ? e.button !== 0
      : false;
    if (mouseButton) {
      return;
    }
    const frame = this.refFrame.current;
    frame.style.visibility = "hidden";
    frame.style.display = "none";
    document.removeEventListener("mousemove", this.onMouseMove);
    this.setState({ mouseDown: false });
  };

  render() {
    const { children, ...rest } = this.props;
    return (
      <StyledFrame {...rest}>
        <div className="selectFrame" ref={this.refFrame} />
      </StyledFrame>
    );
  }
}

export default SelectedFrame;
