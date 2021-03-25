import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";
import { isMobile } from "react-device-detect";
import { observer, inject } from "mobx-react";
import FilesRowContainer from "./FilesRow/FilesRowContainer";
import FilesTileContainer from "./FilesTile/FilesTileContainer";
import EmptyContainer from "./EmptyContainer";

const backgroundDragColor = "#EFEFB2";

const backgroundDragEnterColor = "#F8F7BF";

const CustomTooltip = styled.div`
  position: fixed;
  display: none;
  padding: 8px;
  z-index: 150;
  background: #fff;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  -moz-border-radius: 6px;
  -webkit-border-radius: 6px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);

  .tooltip-moved-obj-wrapper {
    display: flex;
    align-items: center;
  }
  .tooltip-moved-obj-icon {
    margin-right: 6px;
  }
  .tooltip-moved-obj-extension {
    color: #a3a9ae;
  }
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    // this.tooltipRef = React.createRef();
  }

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

  // setTooltipPosition = (e) => {
  //   const tooltip = this.tooltipRef.current;
  //   if (tooltip) {
  //     const margin = 8;
  //     tooltip.style.left = e.pageX + margin + "px";
  //     tooltip.style.top = e.pageY + margin + "px";
  //   }
  // };

  renderFileMoveTooltip = () => {
    const { selection, iconOfDraggedFile } = this.props;
    const { title } = selection[0];

    const reg = /^([^\\]*)\.(\w+)/;
    const matches = title.match(reg);

    let nameOfMovedObj, fileExtension;
    if (matches) {
      nameOfMovedObj = matches[1];
      fileExtension = matches.pop();
    } else {
      nameOfMovedObj = title;
    }

    return (
      <div className="tooltip-moved-obj-wrapper">
        {iconOfDraggedFile ? (
          <img
            className="tooltip-moved-obj-icon"
            src={`${iconOfDraggedFile}`}
            alt=""
          />
        ) : null}
        {nameOfMovedObj}
        {fileExtension ? (
          <span className="tooltip-moved-obj-extension">.{fileExtension}</span>
        ) : null}
      </div>
    );
  };

  render() {
    const {
      selection,
      fileActionId,
      dragging,
      viewAs,
      t,
      isMobile,
      firstLoad,
      tooltipValue,
      isLoading,
      isEmptyFilesList,
    } = this.props;

    //console.log("Files Home SectionBodyContent render", this.props);

    let fileMoveTooltip;
    if (dragging) {
      fileMoveTooltip = tooltipValue
        ? selection.length === 1 &&
          tooltipValue.label === "TooltipElementMoveMessage"
          ? this.renderFileMoveTooltip()
          : t(tooltipValue.label, { element: tooltipValue.filesCount })
        : "";
    }

    return (!fileActionId && isEmptyFilesList) || null ? (
      firstLoad || (isMobile && isLoading) ? (
        <Loaders.Rows />
      ) : (
        <EmptyContainer />
      )
    ) : (
      <>
        <CustomTooltip ref={this.tooltipRef}>{fileMoveTooltip}</CustomTooltip>
        {viewAs === "tile" ? <FilesTileContainer /> : <FilesRowContainer />}
      </>
    );
  }
}

export default inject(({ initFilesStore, filesStore, selectedFolderStore }) => {
  const {
    dragging,
    setDragging,
    isLoading,
    viewAs,
    tooltipValue,
    canDrag,
    setCanDrag,
  } = initFilesStore;
  const {
    firstLoad,
    selection,
    fileActionStore,
    iconOfDraggedFile,
    filesList,
  } = filesStore;

  const { id: fileActionId } = fileActionStore;

  return {
    dragging,
    fileActionId,
    firstLoad,
    selection,
    viewAs,
    iconOfDraggedFile,
    tooltipValue,
    isLoading,
    isEmptyFilesList: filesList.length <= 0,
    canDrag,
    setCanDrag,
    setDragging,
    folderId: selectedFolderStore.id,
  };
})(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
