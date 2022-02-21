import React, { useCallback } from "react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import TrashIcon from "../../../../../../../../../public/images/button.trash.react.svg";
import { StyledBackupList } from "../StyledBackup";
import { ReactSVG } from "react-svg";
import Text from "@appserver/components/text";
import RadioButton from "@appserver/components/radio-button";

const BackupListBody = ({
  filesList,
  onDeleteBackup,
  onSelectFile,
  selectedFileIndex,
}) => {
  const isFileChecked = useCallback(
    (index) => {
      return index === selectedFileIndex;
    },
    [selectedFileIndex]
  );

  const Item = ({ index, style }) => {
    const file = filesList[index];
    const fileId = file.id;
    const fileName = file.fileName;
    const isChecked = isFileChecked(index);

    return (
      <div style={style}>
        <StyledBackupList isChecked={isChecked}>
          <div className="backup-list_item">
            <ReactSVG
              src={" /static/images/icons/24/file_archive.svg"}
              className="backup-list_icon"
            />

            <Text className="backup-list_full-name">{fileName}</Text>

            <RadioButton
              fontSize="13px"
              fontWeight="400"
              value=""
              label=""
              isChecked={isChecked}
              onClick={onSelectFile}
              name={`${index}_${fileId}`}
              className="backup-list-dialog_checked"
            />

            <div className="backup-list_trash">
              <TrashIcon
                className="backup-list_trash-icon"
                onClick={onDeleteBackup}
              />
            </div>
          </div>
        </StyledBackupList>
      </div>
    );
  };
  return (
    <AutoSizer>
      {({ height, width }) => (
        <List
          height={height}
          width={width + 8}
          itemSize={48}
          itemCount={filesList.length}
          itemData={filesList}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Item}
        </List>
      )}
    </AutoSizer>
  );
};

export default BackupListBody;
