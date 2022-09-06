import React, { createRef } from "react";
import { StyledSelectionArea } from "./StyledSelector";
import { frames } from "./selector-helpers";

class Selector extends React.Component {
  constructor(props) {
    super(props);

    this.selected = [];
    this.areaRect = new DOMRect();
    this.scrollSpeed = { x: 0, y: 0 };
    this.areaLocation = { x1: 0, x2: 0, y1: 0, y2: 0 };

    this.areaRef = createRef(null);

    // this.selectableNodes = new Set();
  }

  componentDidMount() {
    document.addEventListener("mousedown", this.onTapStart);
    document.addEventListener("touchstart", this.onTapStart, {
      passive: false,
    });
  }

  componentWillUnmount() {
    document.removeEventListener("mousedown", this.onTapStart);
    document.removeEventListener("touchstart", this.onTapStart, {
      passive: false,
    });
  }

  frame = () =>
    frames(() => {
      this.recalculateSelectionAreaRect();
      this.updateElementSelection();
      this.redrawSelectionArea();
    });

  isIntersects = (a, b) => {
    return (
      a.right > b.left &&
      a.left < b.right &&
      a.bottom > b.top &&
      a.top < b.bottom
    );
  };

  recalculateSelectionAreaRect = () => {
    const targetElement =
      document.getElementsByClassName(this.props.containerClass)[0] ??
      document.querySelectorAll("html")[0];

    if (!targetElement) return;

    const {
      scrollTop,
      scrollHeight,
      clientHeight,
      scrollLeft,
      scrollWidth,
      clientWidth,
    } = targetElement;

    const targetRect = targetElement.getBoundingClientRect();

    const { x1, y1 } = this.areaLocation;
    let { x2, y2 } = this.areaLocation;

    if (x2 < targetRect.left) {
      this.scrollSpeed.x = scrollLeft ? -Math.abs(targetRect.left - x2) : 0;
      x2 = x2 < targetRect.left ? targetRect.left : x2;
    } else if (x2 > targetRect.right) {
      this.scrollSpeed.x =
        scrollWidth - scrollLeft - clientWidth
          ? Math.abs(targetRect.left + targetRect.width - x2)
          : 0;
      x2 = x2 > targetRect.right ? targetRect.right : x2;
    } else {
      this.scrollSpeed.x = 0;
    }

    if (y2 < targetRect.top) {
      this.scrollSpeed.y = scrollTop ? -Math.abs(targetRect.top - y2) : 0;
      y2 = y2 < targetRect.top ? targetRect.top : y2;
    } else if (y2 > targetRect.bottom) {
      this.scrollSpeed.y =
        scrollHeight - scrollTop - clientHeight
          ? Math.abs(targetRect.top + targetRect.height - y2)
          : 0;
      y2 = y2 > targetRect.bottom ? targetRect.bottom : y2;
    } else {
      this.scrollSpeed.y = 0;
    }

    const x3 = Math.min(x1, x2);
    const y3 = Math.min(y1, y2);
    const x4 = Math.max(x1, x2);
    const y4 = Math.max(y1, y2);

    this.areaRect.x = x3;
    this.areaRect.y = y3;
    this.areaRect.width = x4 - x3;
    this.areaRect.height = y4 - y3;
  };

  updateElementSelection = () => {
    const added = [];
    const removed = [];
    const newSelected = [];

    // TODO: not added tile

    const { selectableClass, onMove } = this.props;
    const selectables = document.getElementsByClassName(selectableClass);

    for (let i = 0; i < selectables.length; i++) {
      const node = selectables[i];

      const isIntersects = this.isIntersects(
        this.areaRect,
        node.getBoundingClientRect()
      );

      if (isIntersects) {
        if (!this.selected.includes(node)) {
          added.push(node);
        }

        // this.selectableNodes.add(node);
        newSelected.push(node);
      }
    }

    for (let i = 0; i < this.selected.length; i++) {
      const node = this.selected[i];

      if (!newSelected.includes(node)) {
        removed.push(node);
      }
    }

    this.selected = newSelected;
    onMove && onMove({ added, removed });
  };

  redrawSelectionArea = () => {
    const { x, y, width, height } = this.areaRect;
    const { style } = this.areaRef.current;

    style.left = `${x}px`;
    style.top = `${y}px`;
    style.width = `${width}px`;
    style.height = `${height}px`;
  };

  addListeners = () => {
    const scroll = this.props.scroll ?? document;

    document.addEventListener("touchmove", this.onMove, {
      passive: false,
    });
    document.addEventListener("mousemove", this.onMove, {
      passive: false,
    });
    document.addEventListener("mouseup", this.onTapStop);
    document.addEventListener("touchcancel", this.onTapStop);
    document.addEventListener("touchend", this.onTapStop);
    scroll.addEventListener("scroll", this.onScroll);
  };

  removeListeners = () => {
    const scroll = this.props.scroll ?? document;

    document.removeEventListener("mousemove", this.onMove);
    document.removeEventListener("touchmove", this.onMove);
    document.removeEventListener("touchmove", this.onTapMove);
    document.removeEventListener("mousemove", this.onTapMove);

    document.removeEventListener("mouseup", this.onTapStop);
    document.removeEventListener("touchcancel", this.onTapStop);
    document.removeEventListener("touchend", this.onTapStop);
    scroll.removeEventListener("scroll", this.onScroll);
  };

  onTapStart = (e) => {
    this.areaLocation = { x1: e.clientX, y1: e.clientY, x2: 0, y2: 0 };

    const scrollElement = this.props.scroll ?? document;

    this.scrollDelta = {
      x: scrollElement.scrollLeft,
      y: scrollElement.scrollTop,
    };

    this.addListeners();
  };

  onMove = (e) => {
    const threshold = 10;
    const { x1, y1 } = this.areaLocation;

    if (!this.areaRef.current) return;

    if (
      Math.abs(e.clientX - x1) >= threshold ||
      Math.abs(e.clientY - y1) >= threshold
    ) {
      document.removeEventListener("mousemove", this.onMove, {
        passive: false,
      });
      document.removeEventListener("touchmove", this.onMove, {
        passive: false,
      });

      document.addEventListener("mousemove", this.onTapMove, {
        passive: false,
      });
      document.addEventListener("touchmove", this.onTapMove, {
        passive: false,
      });

      this.areaRef.current.style.display = "block";

      this.setupSelectionArea();
      this.onTapMove(e);
    }
  };

  onTapMove = (e) => {
    this.areaLocation.x2 = e.clientX;
    this.areaLocation.y2 = e.clientY;

    this.frame().next(e);
  };

  onTapStop = (e) => {
    this.removeListeners();

    this.scrollSpeed.x = 0;
    this.scrollSpeed.y = 0;
    this.selected = [];

    this.frame()?.cancel();

    if (this.areaRef.current) this.areaRef.current.style.display = "none";
  };

  onScroll = () => {
    const scrollElement = this.props.scroll ?? document;
    const { scrollTop, scrollLeft } = scrollElement;

    this.areaLocation.x1 += this.scrollDelta.x - scrollLeft;
    this.areaLocation.y1 += this.scrollDelta.y - scrollTop;
    this.scrollDelta.x = scrollLeft;
    this.scrollDelta.y = scrollTop;

    this.setupSelectionArea();
    this.frame().next(null);
  };

  setupSelectionArea = () => {
    const area = this.areaRef.current;

    area.style.marginTop = 0;
    area.style.marginLeft = 0;
  };

  render() {
    // console.log("Selector render");

    return (
      <StyledSelectionArea className="selection-area" ref={this.areaRef} />
    );
  }
}

Selector.defaultProps = {
  selectableClass: "",
};

export default Selector;
