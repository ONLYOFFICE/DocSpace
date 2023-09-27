import React, { createRef } from "react";
import { StyledSelectionArea } from "./StyledSelectionArea";
import { frames } from "./selector-helpers";
import { withTheme } from "styled-components";

class SelectionArea extends React.Component {
  constructor(props) {
    super(props);

    this.areaRect = new DOMRect();
    this.scrollSpeed = { x: 0, y: 0 };
    this.scrollElement = null;
    this.areaLocation = { x1: 0, x2: 0, y1: 0, y2: 0 };
    this.areaRef = createRef(null);

    this.selectableNodes = new Set();

    this.elemRect = {};
    this.arrayOfTypes = [];
  }

  componentDidMount() {
    document.addEventListener("mousedown", this.onTapStart);
  }

  componentDidUpdate(prevProps) {
    const { isRooms, viewAs } = this.props;
    if (isRooms !== prevProps.isRooms || viewAs !== prevProps.viewAs) {
      this.arrayOfTypes = [];
    }
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

  isIntersects = (itemIndex, itemType) => {
    const { right, left, bottom, top } = this.areaRect;
    const { scrollTop } = this.scrollElement;
    const { viewAs, countTilesInRow, defaultHeaderHeight, arrayTypes, theme } =
      this.props;

    const isRtl = theme.interfaceDirection === "rtl";

    let itemTop, itemBottom, itemLeft, itemRight;

    if (viewAs === "tile") {
      let countOfMissingTiles = 0;
      const itemGap = arrayTypes.find((x) => x.type === itemType).rowGap;

      // TOP/BOTTOM item position
      if (itemIndex === 0) {
        itemTop = this.elemRect.top - scrollTop;
        itemBottom = itemTop + this.elemRect.height;
      } else {
        const indexOfType = this.arrayOfTypes.findIndex(
          (x) => x.type === itemType
        );
        const headersCount = indexOfType === 0 ? 0 : indexOfType;

        itemTop = headersCount * defaultHeaderHeight;
        const itemHeight = this.arrayOfTypes[indexOfType].itemHeight + itemGap;

        if (!headersCount) {
          const rowIndex = Math.trunc(itemIndex / countTilesInRow);

          itemTop += this.elemRect.top + itemHeight * rowIndex - scrollTop;
          itemBottom = itemTop + itemHeight - itemGap;
        } else {
          let prevRowsCount = 0;

          for (let i = 0; i < indexOfType; i++) {
            const item = arrayTypes.find(
              (x) => x.type === this.arrayOfTypes[i].type
            );

            countOfMissingTiles += item.countOfMissingTiles;
            prevRowsCount += item.rowCount;

            itemTop +=
              (this.arrayOfTypes[i].itemHeight + item.rowGap) * item.rowCount;
          }

          const nextRow =
            Math.floor((itemIndex + countOfMissingTiles) / countTilesInRow) -
            prevRowsCount;

          itemTop += this.elemRect.top + itemHeight * nextRow - scrollTop;
          itemBottom = itemTop + itemHeight - itemGap;
        }
      }

      let columnIndex = (itemIndex + countOfMissingTiles) % countTilesInRow;

      // Mirror fileIndex for RTL interface (2, 1, 0 => 0, 1, 2)
      if (isRtl && viewAs === "tile") {
        columnIndex = countTilesInRow - 1 - columnIndex;
      }

      // LEFT/RIGHT item position
      if (columnIndex == 0) {
        itemLeft = this.elemRect.left;
        itemRight = itemLeft + this.elemRect.width;
      } else {
        itemLeft =
          this.elemRect.left + (this.elemRect.width + itemGap) * columnIndex;
        itemRight = itemLeft + this.elemRect.width;
      }

      return (
        right > itemLeft &&
        left < itemRight &&
        bottom > itemTop &&
        top < itemBottom
      );
    } else {
      const itemHeight = this.elemRect.height;
      if (itemIndex === 0) {
        itemTop = this.elemRect.top - scrollTop;
        itemBottom = itemTop + itemHeight;
      } else {
        itemTop = this.elemRect.top + itemHeight * itemIndex - scrollTop;
        itemBottom = itemTop + itemHeight;
      }

      return bottom > itemTop && top < itemBottom;
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

    const { selectableClass, onMove, viewAs, itemClass } = this.props;
    const selectableItems = document.getElementsByClassName(selectableClass);

    const selectables = [...selectableItems, ...this.selectableNodes];

    for (let i = 0; i < selectables.length; i++) {
      const node = selectables[i];

      const splitItem =
        viewAs === "tile"
          ? node.getAttribute("value").split("_")
          : node
              .getElementsByClassName(itemClass)[0]
              .getAttribute("value")
              .split("_");

      const itemType = splitItem[0];
      const itemIndex = splitItem[4];

      //TODO: maybe do this line little bit better
      if (this.arrayOfTypes.findIndex((x) => x.type === itemType) === -1) {
        this.arrayOfTypes.push({
          type: itemType,
          itemHeight: node.getBoundingClientRect().height,
        });
      }

      const isIntersects = this.isIntersects(+itemIndex, itemType);

      if (isIntersects) {
        added.push(node);

        newSelected.push(node);
      } else {
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

    window.addEventListener("blur", this.onTapStop);

    this.scrollElement.addEventListener("scroll", this.onScroll);
  };

  removeListeners = () => {
    document.removeEventListener("mousemove", this.onMove);
    document.removeEventListener("mousemove", this.onTapMove);

    document.removeEventListener("mouseup", this.onTapStop);
    window.removeEventListener("blur", this.onTapStop);
    this.scrollElement.removeEventListener("scroll", this.onScroll);
  };

  onTapStart = (e) => {
    const {
      onMove,
      selectableClass,
      scrollClass,
      viewAs,
      itemsContainerClass,
      isRooms,
      folderHeaderHeight,
    } = this.props;

    if (
      e.target.closest(".not-selectable") ||
      e.target.closest(".tile-selected") ||
      e.target.closest(".table-row-selected") ||
      e.target.closest(".row-selected") ||
      !e.target.closest("#sectionScroll") ||
      e.target.closest(".table-container_row-checkbox") ||
      e.target.closest(".item-file-name")
    )
      return;

    // if (e.target.tagName === "A") {
    //   const node = e.target.closest("." + selectableClass);
    //   node && onMove && onMove({ added: [node], removed: [], clear: true });
    //   return;
    // }

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

    const threshold = 10;
    const { x1, y1 } = this.areaLocation;

    if (
      Math.abs(e.clientX - x1) >= threshold ||
      Math.abs(e.clientY - y1) >= threshold
    ) {
      onMove && onMove({ added: [], removed: [], clear: true });
    }

    this.addListeners();

    const itemsContainer = document.getElementsByClassName(itemsContainerClass);

    if (!itemsContainer) return;

    const itemsContainerRect = itemsContainer[0].getBoundingClientRect();

    if (!isRooms && viewAs === "tile") {
      this.elemRect.top =
        scroll.scrollTop + itemsContainerRect.top + folderHeaderHeight;
      this.elemRect.left = scroll.scrollLeft + itemsContainerRect.left;
    } else {
      this.elemRect.top = scroll.scrollTop + itemsContainerRect.top;
      this.elemRect.left = scroll.scrollLeft + itemsContainerRect.left;
    }

    const elemRect = itemsContainer[0]
      .getElementsByClassName(selectableClass)[0]
      .getBoundingClientRect();

    this.elemRect.width = elemRect.width;
    this.elemRect.height = elemRect.height;
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

    this.frame().next();
  };

  onTapStop = (e) => {
    this.removeListeners();

    this.scrollSpeed.x = 0;
    this.scrollSpeed.y = 0;

    this.selectableNodes.clear();

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

    for (let i = 0; i < selectables.length; i++) {
      const node = selectables[i];
      this.selectableNodes.add(node);
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

export default withTheme(SelectionArea);
