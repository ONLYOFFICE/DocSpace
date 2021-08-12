import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./RowsView/FilesRowContainer";
import FilesTileContainer from "./TilesView/FilesTileContainer";
import EmptyContainer from "../../../../components/EmptyContainer";
import withLoader from "../../../../HOCs/withLoader";

let currentDroppable = null;

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
      e.target.closest(".scroll-body") &&
      !e.target.closest(".files-item") &&
      !e.target.closest(".not-selectable")
    )
      setSelection([]);
  };

  const onMouseMove = (e) => {
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
        currentDroppable.classList.remove("droppable-hover");
      }
      currentDroppable = droppable;

      if (currentDroppable) {
        currentDroppable.classList.add("droppable-hover");
        currentDroppable = droppable;
      }
    }
  };

  const onMouseUp = (e) => {
    document.body.classList.remove("drag-cursor");

    const treeElem = e.target.closest(".tree-drag");
    const treeClassList = treeElem && treeElem.classList;
    const isDragging = treeElem && treeClassList.contains("dragging");

    let index = null;
    for (let i in treeClassList) {
      if (treeClassList[i] === "dragging") {
        index = i - 1;
        break;
      }
    }

    const treeValue = isDragging ? treeClassList[index].split("_")[1] : null;

    const elem = e.target.closest(".droppable");
    const title = elem && elem.dataset.title;
    const value = elem && elem.getAttribute("value");
    if ((!value && !treeValue) || isRecycleBinFolder) {
      setDragging(false);
      setStartDrag(false);
      return;
    }

    const folderId = value ? value.split("_")[1] : treeValue;

    setStartDrag(false);
    setDragging(false);
    onMoveTo(folderId, title);
    return;
  };

  const onMoveTo = (destFolderId, title) => {
    const id = isNaN(+destFolderId) ? destFolderId : +destFolderId;
    moveDragItems(id, title, {
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
  return (!fileActionId && isEmptyFilesList) || null ? (
    <EmptyContainer />
  ) : viewAs === "tile" ? (
    <FilesTileContainer t={t} />
  ) : (
    <FilesRowContainer tReady={tReady} />
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
      filesList,
      dragging,
      setDragging,
      viewAs,
      setTooltipPosition,
      startDrag,
      setStartDrag,
      setSelection,
    } = filesStore;

    return {
      dragging,
      startDrag,
      setStartDrag,
      fileActionId: fileActionStore.id,
      isEmptyFilesList: filesList.length <= 0,
      setDragging,
      startDrag,
      setStartDrag,
      folderId: selectedFolderStore.id,
      setTooltipPosition,
      isRecycleBinFolder: treeFoldersStore.isRecycleBinFolder,
      moveDragItems: filesActionsStore.moveDragItems,
      viewAs,
      setSelection,
    };
  }
)(
  withRouter(
    withTranslation(["Home", "Translations"])(
      withLoader(observer(SectionBodyContent))()
    )
  )
);
