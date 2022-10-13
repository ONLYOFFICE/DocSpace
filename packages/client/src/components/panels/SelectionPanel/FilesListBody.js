import React, { useCallback, useEffect, useRef, useState } from "react";
import Loader from "@docspace/components/loader";
import Text from "@docspace/components/text";
import { useTranslation, withTranslation } from "react-i18next";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import { inject, observer } from "mobx-react";
import FilesListRow from "./FilesListRow";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
import { StyledItemsLoader } from "../SelectionPanel/StyledSelectionPanel";
import Loaders from "@docspace/common/components/Loaders";

let countLoad, timerId;
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
  maxHeight = 384,
}) => {
  const { t } = useTranslation(["SelectFile", "Common"]);
  const [isLoading, setIsLoading] = useState(false);

  const filesListRef = useRef(null);
  if (page === 0) {
    countLoad = 0;
  }

  useEffect(() => {
    return () => {
      clearTimeout(timerId);
      timerId = null;
    };
  }, []);
  useEffect(() => {
    if (filesListRef && filesListRef.current) {
      filesListRef.current.resetloadMoreItemsCache(true);
    }
  }, [folderId, page, displayType]);

  useEffect(() => {
    if (isNextPageLoading) {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);
    } else {
      isLoading && setIsLoading(false);
      clearTimeout(timerId);
      timerId = null;
    }
  }, [isNextPageLoading]);

  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? files.length + 1 : files.length;

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      const isLoaded = !hasNextPage || index < files.length;
      if (isLoaded) {
        clearTimeout(timerId);
        timerId = null;
      }
      return isLoaded;
    },
    [files, hasNextPage]
  );

  const loadMoreItems = useCallback(() => {
    if (folderId && page == 0 && isNextPageLoading) {
      loadNextPage && loadNextPage();
      return;
    }

    if (isNextPageLoading) return;
    countLoad++;

    folderId && loadNextPage && loadNextPage();
  }, [isNextPageLoading, files, displayType, folderId]);

  const renderPageLoader = (style) => {
    return (
      <div style={style}>
        <StyledItemsLoader key="loader">
          <Loader
            theme={theme}
            type="oval"
            size="16px"
            className="panel-loader"
          />
          <Text theme={theme} as="span">
            {t("Common:LoadingProcessing")} {t("Common:LoadingDescription")}
          </Text>
        </StyledItemsLoader>
      </div>
    );
  };

  const renderFirstLoader = useCallback(
    (style) => {
      return (
        <div style={style}>
          <div className="selection-panel_loader" key="loader">
            {isLoading ? <Loaders.ListLoader withoutFirstRectangle /> : <></>}
          </div>
        </div>
      );
    },
    [isLoading]
  );

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

      if (!isLoaded || !folderId) {
        if (countLoad >= 1) {
          return renderPageLoader(style);
        }

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
                height={maxHeight}
                itemCount={itemCount}
                itemSize={48}
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
            headerText={t("Files:EmptyFolderHeader")}
            imageSrc="/static/images/empty.screen.react.svg"
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
})(observer(withTranslation(["Common", "Files"])(FilesListBody)));
