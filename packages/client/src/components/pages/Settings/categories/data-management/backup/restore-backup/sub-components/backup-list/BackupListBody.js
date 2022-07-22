import React, { useCallback } from "react";
import { FixedSizeList as List } from "react-window";
import { ReactSVG } from "react-svg";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import Text from "@docspace/components/text";
import RadioButton from "@docspace/components/radio-button";
import TrashIcon from "PUBLIC_DIR/images/delete.react.svg";
import { StyledBackupList } from "../../../StyledBackup";

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

  const onTrashClick = (id) => {
    onDeleteBackup(id);
  };

  const Item = useCallback(
    ({ index, style }) => {
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

              <TrashIcon
                className="backup-list_trash-icon"
                onClick={() => onTrashClick(fileId)}
              />
            </div>
          </StyledBackupList>
        </div>
      );
    },
    [filesList, isFileChecked]
  );

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
