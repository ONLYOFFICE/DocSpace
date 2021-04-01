import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./FilesRow/FilesRowContainer";
import FilesTileContainer from "./FilesTile/FilesTileContainer";
import EmptyContainer from "./EmptyContainer";

let currentDroppable = null;

const SectionBodyContent = (props) => {
  const {
    t,
    fileActionId,
    viewAs,
    firstLoad,
    isLoading,
    isEmptyFilesList,
    folderId,
    dragging,
    setDragging,
    setTooltipPosition,
    isRecycleBinFolder,
    moveDragItems,
  } = props;

  useEffect(() => {
    const customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    if (isMobile) {
      customScrollElm && customScrollElm.scrollTo(0, 0);
    }

    dragging && window.addEventListener("mouseup", onMouseUp);
    dragging && document.addEventListener("mousemove", onMouseMove);

    document.addEventListener("dragover", onDragOver);
    document.addEventListener("dragleave", onDragLeaveDoc);
    document.addEventListener("drop", onDropEvent);
    return () => {
      window.removeEventListener("mouseup", onMouseUp);
      document.removeEventListener("mousemove", onMouseMove);

      document.removeEventListener("dragover", onDragOver);
      document.removeEventListener("dragleave", onDragLeaveDoc);
      document.removeEventListener("drop", onDropEvent);
    };
  }, [onMouseUp, onMouseMove, dragging, folderId]);

  const onMouseMove = (e) => {
    !dragging && setDragging(true);

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
    const value = elem && elem.getAttribute("value");
    if ((!value && !treeValue) || isRecycleBinFolder) {
      return setDragging(false);
    }

    const folderId = value ? value.split("_")[1] : treeValue;

    setDragging(false);
    onMoveTo(folderId);
    return;
  };

  const onMoveTo = (destFolderId) => {
    const id = isNaN(+destFolderId) ? destFolderId : +destFolderId;
    moveDragItems(id, t("MoveToOperation")); //TODO: then catch
  };

  const onDropEvent = () => {
    dragging && setDragging(false);
  };

  const onDragOver = (e) => {
    e.preventDefault();
    if (e.dataTransfer.items.length > 0 && !dragging) {
      setDragging(true);
    }
  };

  const onDragLeaveDoc = (e) => {
    e.preventDefault();
    if (dragging && !e.relatedTarget) {
      setDragging(false);
    }
  };

  //console.log("Files Home SectionBodyContent render", props);

  return (!fileActionId && isEmptyFilesList) || null ? (
    firstLoad || (isMobile && isLoading) ? (
      <Loaders.Rows />
    ) : (
      <EmptyContainer />
    )
  ) : viewAs === "tile" ? (
    <FilesTileContainer />
  ) : (
    <FilesRowContainer />
  );
};

export default inject(
  ({
    initFilesStore,
    filesStore,
    selectedFolderStore,
    treeFoldersStore,
    filesActionsStore,
  }) => {
    const {
      dragging,
      setDragging,
      isLoading,
      viewAs,
      setTooltipPosition,
    } = initFilesStore;
    const { firstLoad, fileActionStore, filesList } = filesStore;

    return {
      dragging,
      fileActionId: fileActionStore.id,
      firstLoad,
      viewAs,
      isLoading,
      isEmptyFilesList: filesList.length <= 0,
      setDragging,
      folderId: selectedFolderStore.id,
      setTooltipPosition,
      isRecycleBinFolder: treeFoldersStore.isRecycleBinFolder,
      moveDragItems: filesActionsStore.moveDragItems,
    };
  }
)(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
