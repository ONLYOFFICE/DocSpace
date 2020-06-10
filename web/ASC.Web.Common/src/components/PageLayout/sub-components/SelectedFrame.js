import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

const StyledFrame = styled.div`
  height: 100%;
  display: contents;

  .selectFrame {
    display: block;
    line-height: 0;
    border: 1px dotted #5c6a8e;
    background-color: #6582c9;
    position: fixed;
    z-index: 100;
    visibility: hidden;
    opacity: 0.4;
  }
`;

class SelectedFrame extends React.Component {
  state = {
    mouseDown: false,
    top: 0,
    left: 0
  };

  refFrame = React.createRef();
  container = null;
  offsetTopStart = 185;
  offsetTopEnd = 135;

  getCoords = e => {
    const posX = e.pageX;
    const posY = e.pageY;
    return [posY, posX];
    //posx = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
    //posy = e.clientY + document.body.scrollTop + document.documentElement.scrollTop;
  };

  onMouseDown = e => {
    this.container = document.getElementById("rowContainer");
    if(!this.container) {
      return;
    }

    const mouseYX = this.getCoords(e);
    const top = mouseYX[0];
    const left = mouseYX[1];
    let update = true;

    for (let childItem in this.container.childNodes) {
      if (this.container.childNodes[childItem].nodeType === 1) {
        const item = this.container.childNodes[childItem];
        const item2 = item.childNodes[0];
        const itemHeight = item2.offsetHeight;

        const topStart = top - this.offsetTopStart - itemHeight;
        const topEnd = mouseYX[0] - this.offsetTopEnd - itemHeight;

        if (item.offsetTop >= topStart && item.offsetTop <= topEnd) {
          if (item2.getAttribute("draggable") === "true") {
            update = false;
            break;
          }
        }
      }
    }

    if (update) {
      document.addEventListener("mousemove", this.onMouseMove, false);
      this.setState({ mouseDown: true, top, left });
    }
  };

  onMouseMove = e => {
    const { mouseDown, left, top } = this.state;

    if (mouseDown) {
      const mouseYX = this.getCoords(e);
      const frame = this.refFrame.current;
      let currentLeft = left;
      let currentTop = top;
      let nextTop = mouseYX[0];
      let nextLeft = mouseYX[1];

      if (currentLeft === nextLeft || currentTop === nextTop) {
        return;
      }

      if (currentLeft > nextLeft) {
        currentLeft = currentLeft + nextLeft;
        nextLeft = currentLeft - nextLeft;
        currentLeft = currentLeft - nextLeft;
      }

      if (currentTop > nextTop) {
        currentTop = currentTop + nextTop;
        nextTop = currentTop - nextTop;
        currentTop = currentTop - nextTop;
      }

      const width = nextLeft - currentLeft;
      const height = nextTop - currentTop;

      frame.style.top = `${currentTop}px`;
      frame.style.left = `${currentLeft}px`;
      frame.style.width = `${width}px`;
      frame.style.height = `${height}px`;
      frame.style.visibility = "visible";

      const selectedItems = [];
      for (let childItem in this.container.childNodes) {
        if (this.container.childNodes[childItem].nodeType === 1) {
          const item = this.container.childNodes[childItem];
          const item2 = item.childNodes[0];

          const itemHeight = item2.offsetHeight;
          const topStartUp = top - this.offsetTopStart - itemHeight;
          const topEndUp = mouseYX[0] - this.offsetTopEnd - itemHeight;

          const topEndDown = mouseYX[0] - this.offsetTopStart - itemHeight;
          const topStartDown = top - this.offsetTopEnd - itemHeight;

          if (
            (item.offsetTop >= topStartUp && item.offsetTop <= topEndUp) ||
            (item.offsetTop <= topStartDown && item.offsetTop >= topEndDown)
          ) {
            const value = item2.getAttribute("value");
            selectedItems.push(value);
          }
        }
      }

      this.props.setSelections(selectedItems);
    }
  };

  onMouseUp = () => {
    const frame = this.refFrame.current;
    frame.style.visibility = "hidden";
    document.removeEventListener("mousemove", this.onMouseMove);
    this.setState({ mouseDown: false });
  };

  componentDidMount() {
    window.addEventListener("mouseup", this.onMouseUp);
  }

  componentWillUnmount() {
    window.removeEventListener("mouseup", this.onMouseUp);
  }

  render() {
    const { children, ...rest } = this.props;
    return (
      <StyledFrame onMouseDown={this.onMouseDown} {...rest}>
        <div className="selectFrame" ref={this.refFrame} />
        {children}
      </StyledFrame>
    );
  }
}

SelectedFrame.propTypes = {
  children: PropTypes.any,
  setSelections: PropTypes.func
};

export default SelectedFrame;
