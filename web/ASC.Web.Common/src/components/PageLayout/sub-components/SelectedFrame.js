import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

const StyledFrame = styled.div`
  .selectFrame {
    user-select: auto;
    display: block;
    position: absolute;
    line-height: 0;
    border: 1px dotted #5c6a8e;
    background-color: #6582c9;
    z-index: 100;
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
  container = null;
  wrapper = null;

  getCoords = (e) => {
    const offsetScroll = this.props.scrollRef.current.viewScrollTop || 0;
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
    this.wrapper = document.getElementsByClassName("section-wrapper")[0];
    this.container = document.getElementById("rowContainer");
    //console.log("e.target.tagName", e.target.tagName);
    //e.target.tagName !== "DIV"
    if (
      mouseButton ||
      !this.container ||
      e.target.tagName === "INPUT" ||
      e.target.tagName === "BUTTON"
    ) {
      return;
    }

    const { scrollRef } = this.props;
    const { view } = scrollRef.current;

    const mouseYX = this.getCoords(e);
    const top = mouseYX[0];
    const left = mouseYX[1];
    let needUpdate = true;
    let offsetScroll;

    const offsetTop = view.offsetParent.offsetTop;
    const offsetLeft = view.offsetParent.offsetLeft;

    const filterContainer = this.wrapper.childNodes[0].childNodes[0];
    const filterContainerHeight = 47;
    const offset =
      window.getComputedStyle(filterContainer).display === "none"
        ? 0
        : filterContainerHeight;

    const smallPadding = -4;
    const bigPadding = 24;

    if (this.props.viewAs === "tile") {
      for (let childItem in this.container.childNodes) {
        if (
          this.container.childNodes[childItem].nodeType === 1 &&
          this.container.childNodes[childItem].tagName === "DIV"
        ) {
          const elements = this.container.childNodes[childItem].childNodes;
          for (let item of elements) {
            const itemOffsetLeft = item.offsetLeft || 0;
            const itemOffsetTop = item.offsetTop || 0;
            const itemHeight = item.offsetHeight;
            const itemWidth = item.clientWidth;

            const topStartUp =
              top - itemHeight - offsetTop - offset - smallPadding;
            const topEndUp = mouseYX[0] - offsetTop - offset - smallPadding;
            const topStartDown = top - offsetTop - offset - smallPadding;
            const topEndDown =
              mouseYX[0] - itemHeight - offsetTop - offset - smallPadding;

            const leftStart = left - itemWidth - offsetLeft - bigPadding;
            const leftEnd = mouseYX[1] - offsetLeft - bigPadding;

            const leftStart2 = left - offsetLeft - bigPadding;
            const leftEnd2 = mouseYX[1] - itemWidth - offsetLeft - bigPadding;

            if (
              (itemOffsetTop >= topStartUp &&
                itemOffsetTop <= topEndUp &&
                ((itemOffsetLeft >= leftStart && itemOffsetLeft <= leftEnd) ||
                  (itemOffsetLeft <= leftStart2 &&
                    itemOffsetLeft >= leftEnd2))) ||
              (itemOffsetTop <= topStartDown &&
                itemOffsetTop >= topEndDown &&
                ((itemOffsetLeft <= leftStart2 && itemOffsetLeft >= leftEnd2) ||
                  (itemOffsetLeft >= leftStart && itemOffsetLeft <= leftEnd)))
            ) {
              const value = item.childNodes[0].getAttribute("value");
              if (value && value.split("_")[2]) {
                needUpdate = false;
                break;
              }
            }
          }
        }
      }
    } else {
      for (let childItem in this.container.childNodes) {
        if (this.container.childNodes[childItem].nodeType === 1) {
          const item = this.container.childNodes[childItem];
          const currentItem = item.childNodes[0];
          const itemHeight = currentItem.offsetHeight;
          const itemOffsetTop = item.offsetTop;

          //const topStart = top - itemHeight - this.props.scrollRef.current.view.offsetParent.offsetTop - offset - 16;
          //const topEnd = mouseYX[0] - itemHeight;
          offsetScroll = this.props.scrollRef.current.viewScrollTop || 0;
          const topStart =
            top - itemHeight - offsetTop - offset - smallPadding - offsetScroll;
          const topEnd = mouseYX[0] - offsetTop - offset - smallPadding;

          if (
            itemOffsetTop - offsetScroll >= topStart &&
            itemOffsetTop - offsetScroll <= topEnd
          ) {
            const value = currentItem.getAttribute("value");
            if (value && value.split("_")[2]) {
              needUpdate = false;
              break;
            }
          }
        }
      }
    }

    if (needUpdate) {
      document.addEventListener("mousemove", this.onMouseMove, false);
      this.setState({ mouseDown: true, top, left });
    }
  };

  onMouseMove = (e) => {
    const { mouseDown, left, top } = this.state;
    const { scrollRef, viewAs, setSelections } = this.props;
    const { view } = scrollRef.current;

    if (mouseDown) {
      const mouseYX = this.getCoords(e);
      const frame = this.refFrame.current;
      let currentLeft = left;
      let currentTop = top;
      let nextTop = mouseYX[0];
      let nextLeft = mouseYX[1];

      const offsetTop = view.offsetParent.offsetTop;
      const offsetLeft = view.offsetParent.offsetLeft;

      const filterContainer = this.wrapper.childNodes[0].childNodes[0];
      const filterContainerHeight = 47;
      const offset =
        window.getComputedStyle(filterContainer).display === "none"
          ? 0
          : filterContainerHeight;

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

      frame.style.maxWidth = `${
        view.clientWidth - currentLeft + offsetLeft - 2
      }px`;
      const maxHeight =
        view.clientHeight > this.wrapper.clientHeight
          ? view.clientHeight
          : this.wrapper.clientHeight;
      frame.style.maxHeight = `${maxHeight - currentTop + offsetTop - 2}px`;

      const styledTop = currentTop - offsetTop > 0 ? currentTop - offsetTop : 0;
      const styledHeight =
        currentTop - offsetTop >= 0 ? height : top - offsetTop;

      frame.style.top = `${styledTop}px`;
      frame.style.height = `${styledHeight}px`;

      const styledLeft =
        currentLeft - offsetLeft > 0 ? currentLeft - offsetLeft : 0;
      const styledWidth = styledLeft > 0 ? width : left - offsetLeft;

      frame.style.left = `${styledLeft}px`;
      frame.style.width = `${styledWidth}px`;

      frame.style.visibility = "visible";
      frame.style.display = "block";

      const smallPadding = -4;
      const bigPadding = 24;

      const selectedItems = [];

      if (viewAs === "tile") {
        for (let childItem in this.container.childNodes) {
          if (
            this.container.childNodes[childItem].nodeType === 1 &&
            this.container.childNodes[childItem].tagName === "DIV"
          ) {
            const elements = this.container.childNodes[childItem].childNodes;
            for (let item of elements) {
              const itemOffsetLeft = item.offsetLeft || 0;
              const itemOffsetTop = item.offsetTop || 0;
              const itemHeight = item.offsetHeight;
              const itemWidth = item.clientWidth;

              const topStartUp =
                top - itemHeight - offsetTop - offset - smallPadding;
              const topEndUp = mouseYX[0] - offsetTop - offset - smallPadding;
              const topStartDown = top - offsetTop - offset - smallPadding;
              const topEndDown =
                mouseYX[0] - itemHeight - offsetTop - offset - smallPadding;

              const leftStart = left - itemWidth - offsetLeft - bigPadding;
              const leftEnd = mouseYX[1] - offsetLeft - bigPadding;

              const leftStart2 = left - offsetLeft - bigPadding;
              const leftEnd2 = mouseYX[1] - itemWidth - offsetLeft - bigPadding;

              if (
                (itemOffsetTop >= topStartUp &&
                  itemOffsetTop <= topEndUp &&
                  ((itemOffsetLeft >= leftStart && itemOffsetLeft <= leftEnd) ||
                    (itemOffsetLeft <= leftStart2 &&
                      itemOffsetLeft >= leftEnd2))) ||
                (itemOffsetTop <= topStartDown &&
                  itemOffsetTop >= topEndDown &&
                  ((itemOffsetLeft <= leftStart2 &&
                    itemOffsetLeft >= leftEnd2) ||
                    (itemOffsetLeft >= leftStart && itemOffsetLeft <= leftEnd)))
              ) {
                const value = item.childNodes[0].getAttribute("value");
                selectedItems.push(value);
              }
            }
          }
        }
      } else {
        for (let childItem in this.container.childNodes) {
          if (this.container.childNodes[childItem].nodeType === 1) {
            const item = this.container.childNodes[childItem];
            const currentItem = item.childNodes[0];

            const itemHeight = currentItem.offsetHeight;
            const itemOffsetTop = item.offsetTop || 0;

            const topStartUp =
              top - itemHeight - offsetTop - offset - smallPadding;
            const topEndUp = mouseYX[0] - offsetTop - offset - smallPadding;
            const topStartDown = top - offsetTop - offset - smallPadding;
            const topEndDown =
              mouseYX[0] - itemHeight - offsetTop - offset - smallPadding;

            if (
              (itemOffsetTop >= topStartUp && itemOffsetTop <= topEndUp) ||
              (itemOffsetTop <= topStartDown && itemOffsetTop >= topEndDown)
            ) {
              const value = currentItem.getAttribute("value");
              selectedItems.push(value);
            }
          }
        }
      }

      setSelections(selectedItems);
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
  scrollRef: PropTypes.any,
  setSelections: PropTypes.func,
  viewAs: PropTypes.string,
};

export default SelectedFrame;
