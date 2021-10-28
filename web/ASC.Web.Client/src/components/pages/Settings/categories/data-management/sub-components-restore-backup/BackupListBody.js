import React, { useCallback } from "react";
import { FixedSizeList as List } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

import ListRow from "./ListRow";
import Link from "@appserver/components/link";

import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import TrashIcon from "../../../../../../../../../public/images/button.trash.react.svg";
import ContextMenuButton from "@appserver/components/context-menu-button";
const BackupListBody = ({
  displayType,
  needRowSelection,
  filesList,
  onDeleteClick,
  onRestoreClick,
  t,
}) => {
  const Item = ({ index, style }) => {
    const file = filesList[index];
    const fileName = file.fileName;
    const fileExst = "gz";
    const modifyFileName = fileName.substring(0, fileName.indexOf("gz"));

    const getContextBackupOptions = useCallback(() => {
      return [
        {
          "data-index": `${index}`,
          key: "restore-backup",
          label: t("RestoreBackup"),
          onClick: onRestoreClick,
        },
        {
          "data-index": `${index}`,
          key: "delete-backup",
          label: t("Common:Delete"),
          onClick: onDeleteClick,
        },
      ];
    }, [t, onDeleteClick, onRestoreClick]);

    return (
      <div style={style}>
        <ListRow
          displayType={displayType}
          index={index}
          fileName={modifyFileName}
          fileExst={fileExst}
        >
          <div className="backup-list_options">
            {displayType === "modal" ? (
              <>
                <Link
                  data-index={index}
                  className="backup-list_restore-link"
                  onClick={onRestoreClick}
                >
                  {t("RestoreBackup")}
                </Link>
                <TrashIcon
                  data-index={index}
                  className="backup-list_trash-icon"
                  onClick={onDeleteClick}
                />
              </>
            ) : (
              <ContextMenuButton
                className="restore_context-options"
                directionX="right"
                iconName="/static/images/vertical-dots.react.svg"
                size={16}
                color="#A3A9AE"
                getData={getContextBackupOptions}
                isDisabled={false}
              />
            )}
          </div>
        </ListRow>
      </div>
    );
  };
  return (
    <div className="backup-list-row-list">
      <AutoSizer>
        {({ height, width }) => (
          <List
            height={height}
            width={width + 8}
            itemSize={displayType === "aside" ? 56 : 36}
            itemCount={filesList.length}
            itemData={filesList}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {Item}
          </List>
        )}
      </AutoSizer>
    </div>
  );
};

export default BackupListBody;
