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
  children,
  fileExst,
  iconUrl,
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
        src={`${config.homepage}/${iconUrl}`}
        className="select-file-dialog_icon"
      />
      <div data-index={index} className="files-list_full-name">
        <Text data-index={index} className="entry-title">
          {fileName}
        </Text>

        <div data-index={index} className="file-exst">
          {fileExst}
        </div>
      </div>
      <div className="files-list_file-owner_wrapper">{children}</div>
    </div>
  </StyledFilesList>
);

ListRow.defaultProps = {
  needRowSelection: true,
};

export default ListRow;
