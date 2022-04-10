import React, { useCallback, useEffect, useRef } from "react";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { useTranslation, withTranslation } from "react-i18next";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import { inject, observer } from "mobx-react";
import FilesListRow from "./FilesListRow";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";

import Loaders from "@appserver/common/components/Loaders";

let countLoad;
const FilesListBody = ({
  files,
  onSelectFile,
  loadNextPage,
  hasNextPage,
  isNextPageLoading,
  displayType,
  folderId,
  fileId,
  theme,
  page,
  folderSelection,
  getIcon,
}) => {
  const { t } = useTranslation(["SelectFile", "Common"]);
  const filesListRef = useRef(null);

  useEffect(() => {
    countLoad = 0;
    if (filesListRef && filesListRef.current) {
      filesListRef.current.resetloadMoreItemsCache(true);
    }
  }, [folderId, page, displayType]);
  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < files.length;
    },
    [files, hasNextPage]
  );
  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? files.length + 1 : files.length;

  const loadMoreItems = useCallback(() => {
    if (isNextPageLoading) return;
    countLoad++;

    loadNextPage && loadNextPage();
  }, [isNextPageLoading, files, displayType]);

  const renderPageLoader = (style) => {
    return (
      <div style={style}>
        <div
          key="loader"
          className="panel-loader-wrapper loader-wrapper_margin"
        >
          <Loader
            theme={theme}
            type="oval"
            size="16px"
            className="panel-loader"
          />
          <Text theme={theme} as="span">
            {t("Common:LoadingProcessing")} ${t("Common:LoadingDescription")}
          </Text>
        </div>
      </div>
    );
  };
  const renderFirstLoader = (style) => {
    return (
      <div style={style}>
        <div
          key="loader"
          className="panel-loader-wrapper loader-wrapper_margin"
        >
          <Loaders.Rows
            theme={theme}
            style={{
              marginBottom: displayType === "aside" ? "24px" : "26px",
              marginTop: displayType === "aside" ? "8px" : "10px",
            }}
            count={displayType === "aside" ? 12 : 5}
          />
        </div>
      </div>
    );
  };

  const isFileChecked = useCallback(
    (id) => {
      const checked = fileId ? id === fileId : false;
      return checked;
    },
    [fileId]
  );

  const Item = useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        if (countLoad >= 1) return renderPageLoader(style);
        return renderFirstLoader(style);
      }

      const item = files[index];
      const isChecked = folderSelection ? false : isFileChecked(item.id);

      return (
        <div style={style}>
          <FilesListRow
            theme={theme}
            displayType={displayType}
            index={index}
            onSelectFile={onSelectFile}
            item={item}
            isChecked={isChecked}
            folderSelection={folderSelection}
            icon={getIcon(
              32,
              item.fileExst,
              item.providerKey,
              item.contentLength
            )}
          />
        </div>
      );
    },
    [files, fileId, displayType, renderFirstLoader, renderPageLoader]
  );
  return (
    <div className="selection-panel_files-list-body">
      <AutoSizer>
        {({ width, height }) => (
          <InfiniteLoader
            theme={theme}
            ref={filesListRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                theme={theme}
                height={height}
                itemCount={itemCount}
                itemSize={displayType === "aside" ? 56 : 50}
                onItemsRendered={onItemsRendered}
                ref={ref}
                width={width + 8}
                outerElementType={CustomScrollbarsVirtualList}
              >
                {Item}
              </List>
            )}
          </InfiniteLoader>
        )}
      </AutoSizer>

      {!hasNextPage && itemCount === 0 && (
        <div className="select-file-dialog_empty-container">
          <EmptyContainer
            theme={theme}
            headerText={t("Home:EmptyFolderHeader")}
            imageSrc="/static/images/empty_screen.png"
          />
        </div>
      )}
    </div>
  );
};
FilesListBody.defaultProps = {
  listHeight: 300,
  isMultiSelect: false,
};

export default inject(({ auth, settingsStore }) => {
  const { user } = auth.userStore;
  const { getIcon } = settingsStore;
  return {
    viewer: user,
    getIcon,
  };
})(observer(withTranslation(["Common", "Home"])(FilesListBody)));
