import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./FilesRow/FilesRowContainer";
import FilesTileContainer from "./FilesTile/FilesTileContainer";
import EmptyContainer from "./EmptyContainer";

class SectionBodyContent extends React.Component {
  componentDidMount() {
    this.customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    // document.addEventListener("dragstart", this.onDragStart);
    // document.addEventListener("dragover", this.onDragOver);
    // document.addEventListener("dragleave", this.onDragLeaveDoc);
    // document.addEventListener("drop", this.onDropEvent);
  }

  componentWillUnmount() {
    // document.removeEventListener("dragstart", this.onDragStart);
    // document.removeEventListener("dragover", this.onDragOver);
    // document.removeEventListener("dragleave", this.onDragLeaveDoc);
    // document.removeEventListener("drop", this.onDropEvent);
  }

  componentDidUpdate(prevProps, prevState) {
    Object.entries(this.props).forEach(
      ([key, val]) =>
        prevProps[key] !== val && console.log(`Prop '${key}' changed`)
    );
    if (this.state) {
      Object.entries(this.state).forEach(
        ([key, val]) =>
          prevState[key] !== val && console.log(`State '${key}' changed`)
      );
    }
  }

  componentDidUpdate(prevProps) {
    const { folderId } = this.props;

    if (isMobile) {
      if (folderId !== prevProps.folderId) {
        this.customScrollElm && this.customScrollElm.scrollTo(0, 0);
      }
    }
  }

  onDragStart = (e) => {
    if (e.dataTransfer.dropEffect === "none") {
      this.props.canDrag && setCanDrag(false);
    }
  };

  onDropEvent = () => {
    this.props.dragging && this.props.setDragging(false);
  };

  onDragOver = (e) => {
    e.preventDefault();
    const { dragging, setDragging, canDrag } = this.props;
    if (e.dataTransfer.items.length > 0 && !dragging && canDrag) {
      setDragging(true);
    }
  };

  onDragLeaveDoc = (e) => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (dragging && !e.relatedTarget) {
      setDragging(false);
    }
  };

  render() {
    const {
      fileActionId,
      viewAs,
      isMobile,
      firstLoad,
      isLoading,
      isEmptyFilesList,
    } = this.props;

    //console.log("Files Home SectionBodyContent render", this.props);

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
  }
}

export default inject(({ initFilesStore, filesStore, selectedFolderStore }) => {
  const {
    dragging,
    setDragging,
    isLoading,
    viewAs,
    canDrag,
    setCanDrag,
  } = initFilesStore;
  const { firstLoad, fileActionStore, filesList } = filesStore;

  const { id: fileActionId } = fileActionStore;

  return {
    dragging,
    fileActionId,
    firstLoad,
    viewAs,
    isLoading,
    isEmptyFilesList: filesList.length <= 0,
    canDrag,
    setCanDrag,
    setDragging,
    folderId: selectedFolderStore.id,
  };
})(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
