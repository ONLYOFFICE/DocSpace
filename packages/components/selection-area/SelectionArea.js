import React, { createRef } from "react";
import { StyledSelectionArea } from "./StyledSelectionArea";
import { frames } from "./selector-helpers";
import { isDesktop } from "../utils/device";

class SelectionArea extends React.Component {
  constructor(props) {
    super(props);

    this.areaRect = new DOMRect();
    this.scrollSpeed = { x: 0, y: 0 };
    this.scrollElement = null;
    this.areaLocation = { x1: 0, x2: 0, y1: 0, y2: 0 };
    this.areaRef = createRef(null);

    this.selectableNodes = new Set();

    this.array = [];

    this.elemRect = {};
  }

  componentDidMount() {
    document.addEventListener("mousedown", this.onTapStart);
  }

  componentWillUnmount() {
    document.removeEventListener("mousedown", this.onTapStart);
  }

  frame = () =>
    frames(() => {
      this.recalculateSelectionAreaRect();
      this.updateElementSelection();
      this.redrawSelectionArea();
    });

  isIntersects = (itemIndex) => {
    const { right, left, bottom, top } = this.areaRect;
    const { scrollTop } = this.scrollElement;
    const { viewAs, countTilesInRow } = this.props;

    const itemHeight = this.props.itemHeight ?? this.elemRect.height;

    let bTop, bBottom, bLeft, bRight;

    // Tile view
    if (viewAs === "tile") {
      const marginLeft = 14;
      const marginTop = 14;

      let index = Math.trunc(+itemIndex / countTilesInRow);

      const indexInRow = itemIndex % countTilesInRow;

      if (itemIndex == 0) {
        bTop = this.elemRect.top - scrollTop;
        bBottom = this.elemRect.top + itemHeight - scrollTop;
      } else {
        bTop = this.elemRect.top + (itemHeight + marginTop) * index - scrollTop;
        bBottom =
          this.elemRect.top +
          itemHeight +
          (itemHeight + marginTop) * index -
          scrollTop;
      }

      if (indexInRow == 0) {
        bLeft = this.elemRect.left;
        bRight = this.elemRect.left + this.elemRect.width;
      } else {
        bLeft =
          this.elemRect.left + (this.elemRect.width + marginLeft) * indexInRow;
        bRight =
          this.elemRect.left +
          (this.elemRect.width + marginLeft) * indexInRow +
          this.elemRect.width;
      }
    } else {
      // Table/row view

      if (itemIndex == 0) {
        bTop = this.elemRect.top + scrollTop;
        bBottom = this.elemRect.top + itemHeight - scrollTop;
      } else {
        bTop = this.elemRect.top + itemHeight * itemIndex - scrollTop;
        bBottom =
          this.elemRect.top + itemHeight + itemHeight * itemIndex - scrollTop;
      }
    }

    if (viewAs === "tile") {
      return right > bLeft && left < bRight && bottom > bTop && top < bBottom;
    } else {
      return bottom > bTop && top < bBottom;
    }
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

    this.areaRect.x = x3 + 1;
    this.areaRect.y = y3 + 1;
    this.areaRect.width = x4 - x3 - 3;
    this.areaRect.height = y4 - y3 - 3;
  };

  updateElementSelection = () => {
    const added = [];
    const removed = [];
    const newSelected = [];

    const { selectableClass, onMove, viewAs } = this.props;
    const selectables1 = document.getElementsByClassName(selectableClass);

    // console.log("updateElementSelection", selectables);
    const selectables = [...selectables1, ...this.array];

    for (let i = 0; i < selectables.length; i++) {
      const node = selectables[i];

      const itemIndex =
        viewAs === "tile"
          ? node.getAttribute("value").split("_")[4]
          : node
              .getElementsByClassName("files-item")[0]
              .getAttribute("value")
              .split("_")[4];

      // console.log("node", node);
      // console.log("node", node.getBoundingClientRect());
      const isIntersects = this.isIntersects(itemIndex);

      if (isIntersects) {
        if (!this.selectableNodes.has(node)) {
          added.push(node);
        }

        newSelected.push(node);
        this.selectableNodes.add(node);
      }
    }

    for (let node of this.selectableNodes) {
      const isContains = document.contains(node);
      const isIncludes = newSelected.includes(node);

      if (!isIncludes && isContains) {
        this.selectableNodes.delete(node);
        removed.push(node);
      }
    }

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
    document.addEventListener("mousemove", this.onMove, {
      passive: false,
    });
    document.addEventListener("mouseup", this.onTapStop);
    this.scrollElement.addEventListener("scroll", this.onScroll);
  };

  removeListeners = () => {
    document.removeEventListener("mousemove", this.onMove);
    document.removeEventListener("mousemove", this.onTapMove);

    document.removeEventListener("mouseup", this.onTapStop);
    this.scrollElement.removeEventListener("scroll", this.onScroll);
  };

  onTapStart = (e) => {
    const {
      onMove,
      selectableClass,
      scrollClass,
      viewAs,
      itemsContainerClass,
    } = this.props;

    if (e.target.closest(".not-selectable")) return;

    const selectables = document.getElementsByClassName(selectableClass);
    if (!selectables.length) return;

    this.areaLocation = { x1: e.clientX, y1: e.clientY, x2: 0, y2: 0 };

    const scroll =
      scrollClass && document.getElementsByClassName(scrollClass)
        ? document.getElementsByClassName(scrollClass)[0]
        : document;

    this.scrollElement = scroll;

    this.scrollDelta = {
      x: scroll.scrollLeft,
      y: scroll.scrollTop,
    };

    onMove && onMove({ added: [], removed: [], clear: true });
    this.addListeners();

    const itemsContainer = document.getElementsByClassName(itemsContainerClass);

    if (!itemsContainer) return;

    this.elemRect.top =
      scroll.scrollTop + itemsContainer[0].getBoundingClientRect().top;

    this.elemRect.left =
      scroll.scrollLeft + itemsContainer[0].getBoundingClientRect().left;

    if (viewAs === "tile") {
      this.elemRect.width = itemsContainer[0]
        .getElementsByClassName(selectableClass)[0]
        .getBoundingClientRect().width;

      this.elemRect.height = itemsContainer[0]
        .getElementsByClassName(selectableClass)[0]
        .getBoundingClientRect().height;
    }
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

      document.addEventListener("mousemove", this.onTapMove, {
        passive: false,
      });

      this.areaRef.current.style.display = "block";

      this.onTapMove(e);
    }
  };

  onTapMove = (e) => {
    this.areaLocation.x2 = e.clientX;
    this.areaLocation.y2 = e.clientY;

    // console.log("onTapMove");

    this.frame().next();
  };

  onTapStop = (e) => {
    this.removeListeners();

    this.scrollSpeed.x = 0;
    this.scrollSpeed.y = 0;
    this.selectableNodes.clear();

    this.array = [];

    this.frame()?.cancel();

    if (this.areaRef.current) {
      const { style } = this.areaRef.current;

      style.display = "none";
      style.left = "0px";
      style.top = "0px";
      style.width = "0px";
      style.height = "0px";
    }
  };

  onScroll = (e) => {
    const { scrollTop, scrollLeft } = e.target;

    this.areaLocation.x1 += this.scrollDelta.x - scrollLeft;
    this.areaLocation.y1 += this.scrollDelta.y - scrollTop;
    this.scrollDelta.x = scrollLeft;
    this.scrollDelta.y = scrollTop;

    const selectables = document.getElementsByClassName(
      this.props.selectableClass
    );

    if (!this.array.length) {
      this.array = selectables;
    } else {
      this.array = [...this.array, ...selectables];
    }

    this.frame().next(null);
  };

  render() {
    // console.log("SelectionArea render");

    return (
      <StyledSelectionArea className="selection-area" ref={this.areaRef} />
    );
  }
}

SelectionArea.defaultProps = {
  selectableClass: "",
};

export default SelectionArea;
