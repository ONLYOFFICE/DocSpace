import React from "react";
import { ReactSVG } from "react-svg";
import Text from "@appserver/components/text";
import { StyledBackupList } from "../styled-backup";
const ListRow = (props) => {
  const {
    displayType,
    needRowSelection,
    index,
    fileName,
    children,
    fileExst,
    isChecked,
    noCheckBox,
  } = props;
  return (
    <StyledBackupList
      displayType={displayType}
      needRowSelection={needRowSelection}
      isChecked={isChecked}
      noCheckBox={noCheckBox}
    >
      <div data-index={index} className="backup-list_file-name">
        <ReactSVG
          src={" /static/images/icons/24/file_archive.svg"}
          className="backup-list_icon"
        />
        <div data-index={index} className="backup-list_full-name">
          <Text data-index={index} className="backup-list_entry-title">
            {fileName}
          </Text>

          <div data-index={index} className="backup-list_file-exst">
            {fileExst}
          </div>
        </div>
        <div className="backup-list_children">{children}</div>
      </div>
    </StyledBackupList>
  );
};

export default ListRow;
