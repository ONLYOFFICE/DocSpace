import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./RowsView/FilesRowContainer";
import FilesTileContainer from "./TilesView/FilesTileContainer";
import EmptyContainer from "../../../../components/EmptyContainer";
import withLoader from "../../../../HOCs/withLoader";
import TableView from "./TableView/TableContainer";
import { Consumer } from "@appserver/components/utils/context";

let currentDroppable = null;
let isDragActive = false;

const SectionBodyContent = (props) => {
  const {
    t,
    tReady,
    fileActionId,
    isEmptyFilesList,
    folderId,
    dragging,
    setDragging,
    startDrag,
    setStartDrag,
    setTooltipPosition,
    isRecycleBinFolder,
    moveDragItems,
    viewAs,
    setSelection,
    setBufferSelection,
    tooltipPageX,
    tooltipPageY,
  } = props;

  useEffect(() => {
    const customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    if (isMobile) {
      customScrollElm && customScrollElm.scrollTo(0, 0);
    }

    !isMobile && window.addEventListener("mousedown", onMouseDown);
    startDrag && window.addEventListener("mouseup", onMouseUp);
    startDrag && document.addEventListener("mousemove", onMouseMove);

    document.addEventListener("dragover", onDragOver);
    document.addEventListener("dragleave", onDragLeaveDoc);
    document.addEventListener("drop", onDropEvent);

    return () => {
      window.removeEventListener("mousedown", onMouseDown);
      window.removeEventListener("mouseup", onMouseUp);
      document.removeEventListener("mousemove", onMouseMove);

      document.removeEventListener("dragover", onDragOver);
      document.removeEventListener("dragleave", onDragLeaveDoc);
      document.removeEventListener("drop", onDropEvent);
    };
  }, [onMouseUp, onMouseMove, startDrag, folderId, viewAs]);

  const onMouseDown = (e) => {
    if (
      (e.target.closest(".scroll-body") &&
        !e.target.closest(".files-item") &&
        !e.target.closest(".not-selectable") &&
        !e.target.closest(".table-container_group-menu")) ||
      e.target.closest(".files-main-button") ||
      e.target.closest(".add-button") ||
      e.target.closest(".search-input-block")
    ) {
      setSelection([]);
      setBufferSelection(null);
    }
  };

  const onMouseMove = (e) => {
    if (
      Math.abs(e.pageX - tooltipPageX) < 5 &&
      Math.abs(e.pageY - tooltipPageY) < 5
    ) {
      return false;
    }

    isDragActive = true;
    if (!dragging) {
      document.body.classList.add("drag-cursor");
      setDragging(true);
    }

    setTooltipPosition(e.pageX, e.pageY);
    const wrapperElement = document.elementFromPoint(e.clientX, e.clientY);
    if (!wrapperElement) {
      return;
    }

    const droppable = wrapperElement.closest(".droppable");
    if (currentDroppable !== droppable) {
      if (currentDroppable) {
        if (viewAs === "table") {
          const value = currentDroppable.getAttribute("value");
          const classElements = document.getElementsByClassName(value);

          for (let cl of classElements) {
            cl.classList.remove("droppable-hover");
          }
        } else {
          currentDroppable.classList.remove("droppable-hover");
        }
      }
      currentDroppable = droppable;

      if (currentDroppable) {
        if (viewAs === "table") {
          const value = currentDroppable.getAttribute("value");
          const classElements = document.getElementsByClassName(value);

          for (let cl of classElements) {
            cl.classList.add("droppable-hover");
          }
        } else {
          currentDroppable.classList.add("droppable-hover");
          currentDroppable = droppable;
        }
      }
    }
  };

  const onMouseUp = (e) => {
    document.body.classList.remove("drag-cursor");

    const treeElem = e.target.closest(".tree-drag");
    const treeDataValue = treeElem?.dataset?.value;
    const splitValue = treeDataValue && treeDataValue.split(" ");
    const isDragging = splitValue && splitValue.includes("dragging");
    const treeValue = isDragging ? splitValue[0] : null;
    const treeProvider = splitValue && splitValue[splitValue.length - 1];

    const elem = e.target.closest(".droppable");
    const title = elem && elem.dataset.title;
    const value = elem && elem.getAttribute("value");
    if ((!value && !treeValue) || isRecycleBinFolder || !isDragActive) {
      setDragging(false);
      setStartDrag(false);
      isDragActive = false;
      return;
    }

    const folderId = value ? value.split("_")[1] : treeValue;
    const providerKey = value ? value.split("_")[2].trim() : treeProvider;

    setStartDrag(false);
    setDragging(false);
    onMoveTo(folderId, title, providerKey);
    isDragActive = false;
    return;
  };

  const onMoveTo = (destFolderId, title, providerKey) => {
    const id = isNaN(+destFolderId) ? destFolderId : +destFolderId;
    moveDragItems(id, title, providerKey, {
      copy: t("Translations:CopyOperation"),
      move: t("Translations:MoveToOperation"),
    }); //TODO: then catch
  };

  const onDropEvent = () => {
    setDragging(false);
  };

  const onDragOver = (e) => {
    e.preventDefault();
    if (
      e.dataTransfer.items.length > 0 &&
      e.dataTransfer.dropEffect !== "none"
    ) {
      setDragging(true);
    }
  };

  const onDragLeaveDoc = (e) => {
    e.preventDefault();
    if (!e.relatedTarget || !e.dataTransfer.items.length) {
      setDragging(false);
    }
  };

  //console.log("Files Home SectionBodyContent render", props);

  return (
    <Consumer>
      {(context) =>
        (!fileActionId && isEmptyFilesList) || null ? (
          <EmptyContainer />
        ) : viewAs === "tile" ? (
          <FilesTileContainer sectionWidth={context.sectionWidth} t={t} />
        ) : viewAs === "table" ? (
          <TableView sectionWidth={context.sectionWidth} tReady={tReady} />
        ) : (
          <FilesRowContainer
            sectionWidth={context.sectionWidth}
            tReady={tReady}
          />
        )
      }
    </Consumer>
  );
};

export default inject(
  ({
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
    filesActionsStore,
  }) => {
    const {
      fileActionStore,
      isEmptyFilesList,
      dragging,
      setDragging,
      viewAs,
      setTooltipPosition,
      startDrag,
      setStartDrag,
      setSelection,
      tooltipPageX,
      tooltipPageY,
      setBufferSelection,
    } = filesStore;

    return {
      dragging,
      startDrag,
      setStartDrag,
      fileActionId: fileActionStore.id,
      isEmptyFilesList,
      setDragging,
      folderId: selectedFolderStore.id,
      setTooltipPosition,
      isRecycleBinFolder: treeFoldersStore.isRecycleBinFolder,
      moveDragItems: filesActionsStore.moveDragItems,
      viewAs,
      setSelection,
      setBufferSelection,
      tooltipPageX,
      tooltipPageY,
    };
  }
)(
  withRouter(
    withTranslation(["Home", "Common", "Translations"])(
      withLoader(observer(SectionBodyContent))()
    )
  )
);
