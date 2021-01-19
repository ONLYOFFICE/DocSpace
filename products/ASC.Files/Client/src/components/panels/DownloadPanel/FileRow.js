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
  getLoadingFile,
  getFileByFileId,
} from "../../../store/files/selectors";

const StyledFileRow = styled(Row)`
  margin: 0 16px;
  width: calc(100% - 16px);
  box-sizing: border-box;
  font-weight: 600;

  .download_panel-icon {
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
  .__react_component_tooltip.type-light {
    background-color: #f8f7bf !important;
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

const checkExt = (ext) => {
  const extArray = [
    "avi",
    "csv",
    "djvu",
    "doc",
    "docx",
    "dvd",
    "ebook",
    "file_arcive",
    "flv",
    "html",
    "iaf",
    "image",
    "m2ts",
    "mkv",
    "mov",
    "mp4",
    "mpg",
    "odp",
    "ods",
    "odt",
    "pdf",
    "pps",
    "ppsx",
    "ppt",
    "pptx",
    "rtf",
    "sound",
    "svg",
    "txt",
    "xls",
    "xlsx",
    "xps",
  ];
  return extArray.includes(ext);
};

const FileRow = (props) => {
  const {
    t,
    item,
    index,
    archiveFormats,
    imageFormats,
    soundFormats,
    uploaded,
    onOpenSharingPanel,
    cancelCurrentUpload,
    mediaViewerImageFormats,
    mediaViewerMediaFormats,
    onMediaClick,
    currentFileUploadProgress,
    currentFile,
  } = props;
  const name = item.file.name.split(".");
  let ext = name.length > 1 ? name.pop() : "";
  let originalExt = null;

  if (archiveFormats.includes(`.${ext}`)) {
    originalExt = ext;
    ext = "file_arcive";
  }

  if (imageFormats.includes(`.${ext}`)) {
    originalExt = ext;
    ext = "image";
  }

  if (soundFormats.includes(`.${ext}`)) {
    originalExt = ext;
    ext = "sound";
  }

  const fileIcon = checkExt(ext) ? (
    <img src={`images/icons/24/${ext}.svg`} alt={`${ext}`} />
  ) : (
    <img src="images/icons/24/file.svg" alt="file" />
  );

  const color =
    item.fileId && currentFile(item.fileId) && currentFile(item.fileId).shared
      ? "#657077"
      : "#A3A9AE";

  return (
    <>
      {item.cancel ? (
        <></>
      ) : (
        <StyledFileRow
          className="download-row"
          key={item.uniqueId}
          checkbox={false}
          element={fileIcon}
        >
          <>
            {item.fileId ? (
              mediaViewerImageFormats.includes(`.${ext}`) ||
              mediaViewerMediaFormats.includes(`.${ext}`) ||
              mediaViewerImageFormats.includes(`.${originalExt}`) ||
              mediaViewerMediaFormats.includes(`.${originalExt}`) ? (
                <Text
                  fontWeight="600"
                  // MediaViewer doesn't work
                  /*onClick={() => onMediaClick(item.fileId)}*/
                >
                  {name}
                </Text>
              ) : (
                <Link
                  fontWeight="600"
                  href={item.fileInfo ? item.fileInfo.webUrl : ""}
                  target="_blank"
                >
                  {name}
                </Link>
              )
            ) : (
              <Text fontWeight="600">{name}</Text>
            )}
            {originalExt || ext ? (
              <Text fontWeight="600" color="#A3A9AE">
                .{originalExt ? originalExt : ext}
              </Text>
            ) : (
              <></>
            )}
            {item.fileId ? (
              <IconButton
                iconName="CatalogSharedIcon"
                className="download_panel-icon"
                color={color}
                isClickable={true}
                onClick={() =>
                  onOpenSharingPanel(item.fileInfo ? item.fileInfo : "")
                }
              />
            ) : item.error || (!item.fileId && uploaded) ? (
              <div className="download_panel-icon">
                {" "}
                <Icons.LoadErrorIcon
                  size="medium"
                  data-for="errorTooltip"
                  data-tip={item.error || t("UnknownError")}
                />
                <Tooltip
                  id="errorTooltip"
                  className="tooltip-custom"
                  getContent={(dataTip) => (
                    <Text fontSize="13px">{dataTip}</Text>
                  )}
                  effect="float"
                  place="left"
                  maxWidth={320}
                  color="#f8f7bf"
                />
              </div>
            ) : (
              <div className="download_panel-icon">
                <LoadingButton
                  percent={currentFileUploadProgress}
                  onClick={() => cancelCurrentUpload(index)}
                />
              </div>
            )}
          </>
        </StyledFileRow>
      )}
    </>
  );
};
const mapStateToProps = (state, ownProps) => {
  const loadingFile = getLoadingFile(state);

  const { item } = ownProps;
  const { uniqueId } = item;
  return {
    currentFile: (fileId) => getFileByFileId(state, fileId),
    currentFileUploadProgress:
      loadingFile && loadingFile.uniqueId === uniqueId
        ? loadingFile.percent
        : null,
  };
};

export default connect(mapStateToProps)(FileRow);
