import React from "react";
import styled from "styled-components";
import {
  IconButton,
  Row,
  Text,
  Icons,
  Tooltip,
  Link,
} from "asc-web-components";

import LoadingButton from "./LoadingButton";
import { connect } from "react-redux";
import {
  cancelCurrentUpload,
  setSelection,
  setSelected,
  setSharingPanelVisible,
} from "../../../store/files/actions";
import {
  getLoadingFile,
  getFileByFileId,
  getArchiveFormats,
  getImageFormats,
  getSoundFormats,
  getSharePanelVisible,
  getMediaViewerImageFormats,
  getMediaViewerMediaFormats,
  isUploaded,
  getSelected,
  isMediaOrImage,
  getIconSrc,
} from "../../../store/files/selectors";

const StyledFileRow = styled(Row)`
  margin: 0 16px;
  width: calc(100% - 16px);
  box-sizing: border-box;
  font-weight: 600;

  .upload_panel-icon {
    margin-left: auto;
    line-height: 24px;
    display: flex;
    align-items: center;
    flex-direction: row-reverse;

    svg {
      width: 16px;
      height: 16px;
    }
  }

  .img_error {
    filter: grayscale(1);
  }

  .__react_component_tooltip.type-light {
    background-color: #f8f7bf !important;
    box-shadow: none;
    -moz-box-shadow: none;
    -webkit-box-shadow: none;
  }
  .__react_component_tooltip.place-left::after {
    border-left: 6px solid #f8f7bf !important;
  }

  .__react_component_tooltip.place-right::after {
    border-right: 6px solid #f8f7bf !important;
  }

  .__react_component_tooltip.place-top::after {
    border-top: 6px solid #f8f7bf !important;
  }

  .__react_component_tooltip.place-bottom::after {
    border-bottom: 6px solid #f8f7bf !important;
  }
`;

const FileRow = (props) => {
  const {
    t,
    item,
    uploaded,
    cancelCurrentUpload,
    //onMediaClick,
    currentFileUploadProgress,
    fileIcon,
    isMedia,
    ext,
    name,
    color,
  } = props;

  const onCancelCurrentUpload = (e) => {
    console.log("cancel upload ", e);
    const id = e.currentTarget.dataset.id;
    return cancelCurrentUpload(id);
  };

  const onOpenSharingPanel = (item) => {
    //console.log(item);
    const {
      selected,
      setSelected,
      setSelection,
      setSharingPanelVisible,
      sharingPanelVisible,
    } = props;
    selected === "close" && setSelected("none");
    setSelection([item]);
    setSharingPanelVisible(!sharingPanelVisible);
  };

  // const onMediaClick = (id) => {
  //   console.log("id", id);
  //   const item = { visible: true, id: id };
  //   this.props.setMediaViewerData(item);
  // };

  return (
    <>
      <StyledFileRow
        className="download-row"
        key={item.uniqueId}
        checkbox={false}
        element={<img className={item.error && "img_error"} src={fileIcon} alt="" />}
      >
        <>
          {item.fileId ? (
            isMedia ? (
              <Text
                fontWeight="600"
                color={item.error && "#A3A9AE"}
                // MediaViewer doesn't work
                /*onClick={() => onMediaClick(item.fileId)}*/
              >
                {name}
              </Text>
            ) : (
              <Link
                fontWeight="600"
                color={item.error && "#A3A9AE"}
                href={item.fileInfo ? item.fileInfo.webUrl : ""}
                target="_blank"
              >
                {name}
              </Link>
            )
          ) : (
            <Text fontWeight="600" color={item.error && "#A3A9AE"}>
              {name}
            </Text>
          )}
          {ext ? (
            <Text fontWeight="600" color="#A3A9AE">
              {ext}
            </Text>
          ) : (
            <></>
          )}
          {item.fileId ? (
            <IconButton
              iconName="CatalogSharedIcon"
              className="upload_panel-icon"
              color={color}
              isClickable={true}
              onClick={() =>
                onOpenSharingPanel(item.fileInfo ? item.fileInfo : "")
              }
            />
          ) : item.error || (!item.fileId && uploaded) ? (
            <div className="upload_panel-icon">
              {" "}
              <Icons.LoadErrorIcon
                size="medium"
                data-for="errorTooltip"
                data-tip={item.error || t("UnknownError")}
              />
              <Tooltip
                id="errorTooltip"
                className="tooltip-custom"
                getContent={(dataTip) => <Text fontSize="13px">{dataTip}</Text>}
                effect="float"
                place="left"
                maxWidth={320}
                color="#f8f7bf"
              />
            </div>
          ) : (
            <div
              className="upload_panel-icon"
              data-id={item.uniqueId}
              onClick={onCancelCurrentUpload}
            >
              <LoadingButton percent={currentFileUploadProgress} />
            </div>
          )}
        </>
      </StyledFileRow>
    </>
  );
};
const mapStateToProps = (state, ownProps) => {
  const loadingFile = getLoadingFile(state);

  const { item } = ownProps;

  let ext;
  let name;
  let splitted;
  if (item.file) {
    splitted = item.file.name.split(".");
    ext = splitted.length > 1 ? "." + splitted.pop() : "";
    name = splitted[0];
  } else {
    ext = item.fileInfo.fileExst;
    splitted = item.fileInfo.title.split(".");
    name = splitted[0];
  }
  const currentFile = item.fileId ? getFileByFileId(state, item.fileId) : null; // TODO: Change file search in upload collection instead of directory files list
  let color = "#A3A9AE";
  if (currentFile && currentFile.shared) color = "#657077";

  const { uniqueId } = item;
  return {
    currentFileUploadProgress:
      loadingFile && loadingFile.uniqueId === uniqueId
        ? loadingFile.percent
        : null,
    archiveFormats: getArchiveFormats(state),
    imageFormats: getImageFormats(state),
    soundFormats: getSoundFormats(state),
    sharingPanelVisible: getSharePanelVisible(state),
    mediaViewerImageFormats: getMediaViewerImageFormats(state),
    mediaViewerMediaFormats: getMediaViewerMediaFormats(state),
    uploaded: isUploaded(state),
    selected: getSelected(state),
    isMedia: isMediaOrImage(ext)(state),
    fileIcon: getIconSrc(ext, 24)(state),
    ext,
    name,
    color,
  };
};

export default connect(mapStateToProps, {
  setSharingPanelVisible,
  cancelCurrentUpload,
  // setMediaViewerData,
  setSelection,
  setSelected,
})(FileRow);
