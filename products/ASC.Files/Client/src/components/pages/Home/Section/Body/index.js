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

    this.state = {
      isDrag: false,
      canDrag: true,
    };

    this.tooltipRef = React.createRef();
    this.currentDroppable = null;
  }

  componentDidMount() {
    this.customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    window.addEventListener("mouseup", this.onMouseUp);

    document.addEventListener("dragstart", this.onDragStart);
    document.addEventListener("dragover", this.onDragOver);
    document.addEventListener("dragleave", this.onDragLeaveDoc);
    document.addEventListener("drop", this.onDropEvent);
  }

  componentWillUnmount() {
    window.removeEventListener("mouseup", this.onMouseUp);

    document.addEventListener("dragstart", this.onDragStart);
    document.removeEventListener("dragover", this.onDragOver);
    document.removeEventListener("dragleave", this.onDragLeaveDoc);
    document.removeEventListener("drop", this.onDropEvent);
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
      this.state.canDrag && this.setState({ canDrag: false });
    }
  };

  onDropEvent = () => {
    this.props.dragging && this.props.setDragging(false);
  };

  onDragOver = (e) => {
    e.preventDefault();
    const { dragging, setDragging } = this.props;
    if (e.dataTransfer.items.length > 0 && !dragging && this.state.canDrag) {
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

  onMouseDown = (e) => {
    if (
      window.innerWidth < 1025 ||
      e.target.tagName === "rect" ||
      e.target.tagName === "path"
    ) {
      return;
    }
    const mouseButton = e.which
      ? e.which !== 1
      : e.button
      ? e.button !== 0
      : false;
    const label = e.currentTarget.getAttribute("label");
    if (mouseButton || e.currentTarget.tagName !== "DIV" || label) {
      return;
    }
    document.addEventListener("mousemove", this.onMouseMove);
    this.setTooltipPosition(e);
    const { selection } = this.props;

    const elem = e.currentTarget.closest(".draggable");
    if (!elem) {
      return;
    }
    const value = elem.getAttribute("value");
    if (!value) {
      return;
    }
    let splitValue = value.split("_");
    let item = null;
    if (splitValue[0] === "folder") {
      splitValue.splice(0, 1);
      if (splitValue[splitValue.length - 1] === "draggable") {
        splitValue.splice(-1, 1);
      }
      splitValue = splitValue.join("_");

      item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
    } else {
      splitValue.splice(0, 1);
      if (splitValue[splitValue.length - 1] === "draggable") {
        splitValue.splice(-1, 1);
      }
      splitValue = splitValue.join("_");

      item = selection.find((x) => x.id + "" === splitValue && x.fileExst);
    }
    if (item) {
      this.setState({ isDrag: true });
    }
  };

  onMouseUp = (e) => {
    const { selection, dragging, setDragging, dragItem } = this.props;

    document.body.classList.remove("drag-cursor");

    if (this.state.isDrag || !this.state.canDrag) {
      this.setState({ isDrag: false, canDrag: true });
    }
    const mouseButton = e.which
      ? e.which !== 1
      : e.button
      ? e.button !== 0
      : false;
    if (mouseButton || !this.tooltipRef.current || !dragging) {
      return;
    }
    document.removeEventListener("mousemove", this.onMouseMove);
    this.tooltipRef.current.style.display = "none";

    const elem = e.target.closest(".dropable");
    if (elem && selection.length && dragging) {
      const value = elem.getAttribute("value");
      if (!value) {
        setDragging(false);
        return;
      }
      let splitValue = value.split("_");
      let item = null;
      if (splitValue[0] === "folder") {
        splitValue.splice(0, 1);
        if (splitValue[splitValue.length - 1] === "draggable") {
          splitValue.splice(-1, 1);
        }
        splitValue = splitValue.join("_");

        item = selection.find((x) => x.id + "" === splitValue && !x.fileExst);
      } else {
        return;
      }
      if (item) {
        setDragging(false);
        return;
      } else {
        setDragging(false);
        this.onMoveTo(splitValue);
        return;
      }
    } else {
      setDragging(false);
      if (dragItem) {
        this.onMoveTo(dragItem);
        return;
      }
      return;
    }
  };

  onMouseMove = (e) => {
    if (this.state.isDrag) {
      document.body.classList.add("drag-cursor");
      !this.props.dragging && this.props.setDragging(true);
      const tooltip = this.tooltipRef.current;
      tooltip.style.display = "block";
      this.setTooltipPosition(e);

      const wrapperElement = document.elementFromPoint(e.clientX, e.clientY);
      if (!wrapperElement) {
        return;
      }
      const droppable = wrapperElement.closest(".dropable");

      if (this.currentDroppable !== droppable) {
        if (this.currentDroppable) {
          this.currentDroppable.style.background = backgroundDragEnterColor;
        }
        this.currentDroppable = droppable;

        if (this.currentDroppable) {
          droppable.style.background = backgroundDragColor;
          this.currentDroppable = droppable;
        }
      }
    }
  };

  setTooltipPosition = (e) => {
    const tooltip = this.tooltipRef.current;
    if (tooltip) {
      const margin = 8;
      tooltip.style.left = e.pageX + margin + "px";
      tooltip.style.top = e.pageY + margin + "px";
    }
  };

  onMoveTo = (destFolderId) => {
    const {
      selection,
      t,
      isShare,
      isCommon,
      isAdmin,
      setSecondaryProgressBarData,
      copyToAction,
      moveToAction,
    } = this.props;

    const folderIds = [];
    const fileIds = [];
    const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2
    const deleteAfter = true;

    setSecondaryProgressBarData({
      icon: "move",
      visible: true,
      percent: 0,
      label: t("MoveToOperation"),
      alert: false,
    });

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    if (isAdmin) {
      if (isShare) {
        copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    } else {
      if (isShare || isCommon) {
        copyToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      } else {
        moveToAction(
          destFolderId,
          folderIds,
          fileIds,
          conflictResolveType,
          deleteAfter
        );
      }
    }
  };

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

  startMoveOperation = () => {
    this.props.moveToAction(this.props.dragItem);
    this.onCloseThirdPartyMoveDialog();
  };

  startCopyOperation = () => {
    this.props.copyToAction(this.props.dragItem);
    this.onCloseThirdPartyMoveDialog();
  };

  render() {
    //console.log("Files Home SectionBodyContent render", this.props);

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

export default inject(
  ({
    auth,
    initFilesStore,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    filesActionsStore,
  }) => {
    const {
      dragging,
      setDragging,
      isLoading,
      viewAs,
      dragItem,
      tooltipValue,
    } = initFilesStore;
    const {
      firstLoad,
      selection,
      fileActionStore,
      iconOfDraggedFile,
      filesList,
    } = filesStore;

    const { isShareFolder, isCommonFolder } = treeFoldersStore;
    const { id: fileActionId } = fileActionStore;
    const {
      setSecondaryProgressBarData,
    } = uploadDataStore.secondaryProgressDataStore;
    const { copyToAction, moveToAction } = filesActionsStore;

    return {
      isAdmin: auth.isAdmin,
      dragging,
      fileActionId,
      firstLoad,
      selection,
      isShare: isShareFolder,
      isCommon: isCommonFolder,
      viewAs,
      dragItem,
      iconOfDraggedFile,
      tooltipValue,
      isLoading,
      isEmptyFilesList: filesList.length <= 0,

      setDragging,
      setSecondaryProgressBarData,
      copyToAction,
      moveToAction,
    };
  }
)(withRouter(withTranslation("Home")(observer(SectionBodyContent))));
