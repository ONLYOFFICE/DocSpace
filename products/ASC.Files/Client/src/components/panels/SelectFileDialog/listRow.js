import React from "react";
import { StyledFilesList } from "../StyledPanels";
import { ReactSVG } from "react-svg";

import Text from "@appserver/components/text";
import config from "../../../../package.json";

const ListRow = ({
  displayType,
  needRowSelection,
  index,
  onSelectFile,
  fileName,
  fileOwner,
  children,
}) => (
  <StyledFilesList
    displayType={displayType}
    needRowSelection={needRowSelection}
  >
    <div
      data-index={index}
      className="modal-dialog_file-name"
      onClick={onSelectFile}
    >
      <ReactSVG
        src={`${config.homepage}/images/icons/24/file_archive.svg`}
        className="select-file-dialog_icon"
      />
      <div data-index={index} className="files-list_full-name">
        <Text data-index={index} className="entry-title">
          {fileName && fileName.substring(0, fileName.indexOf(".gz"))}
        </Text>

        <div data-index={index} className="file-exst">
          {".gz"}
        </div>
      </div>
      <div className="files-list_file-owner_wrapper">
        {children ? (
          children
        ) : (
          <Text data-index={index} className="files-list_file-owner">
            {fileOwner}
          </Text>
        )}
      </div>
    </div>
  </StyledFilesList>
);

ListRow.defaultProps = {
  fileOwner: "",
  needRowSelection: true,
};

export default ListRow;
