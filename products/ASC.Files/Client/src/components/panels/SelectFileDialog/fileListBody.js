import React, { useCallback } from "react";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { useTranslation, withTranslation } from "react-i18next";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import { inject, observer } from "mobx-react";
import ListRow from "./listRow";
const FileListBody = ({
  children,
  filesList,
  onSelectFile,
  loadNextPage,
  hasNextPage,
  isNextPageLoading,
  displayType,
  viewer,
  listHeight,
  needRowSelection,
  emptyFilesList,
}) => {
  const { t } = useTranslation(["SelectFile", "Common"]);
  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < filesList.length;
    },
    [hasNextPage, filesList]
  );
  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? filesList.length + 1 : filesList.length;

  const loadMoreItems = useCallback(
    (startIndex) => {
      if (isNextPageLoading) return;
      console.log("startIndex", startIndex);
      const options = {
        startIndex: startIndex || 0,
      };

      loadNextPage && loadNextPage(options);
    },
    [isNextPageLoading, filesList]
  );

  const Item = useCallback(
    ({ index, style }) => {
      const file = filesList[index];
      const fileName = file && file.title;
      const fileOwner = file
        ? file.createdBy &&
          ((viewer.id === file.createdBy.id && t("Common:MeLabel")) ||
            file.createdBy.displayName)
        : "";

      return (
        <div style={style}>
          {!isItemLoaded(index) ? (
            <div key="loader">
              <Loader
                type="oval"
                size="16px"
                style={{
                  display: "inline",
                  marginRight: "10px",
                }}
              />
              <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                "Common:LoadingDescription"
              )}`}</Text>
            </div>
          ) : (
            <ListRow
              displayType={displayType}
              needRowSelection={needRowSelection}
              index={index}
              onSelectFile={onSelectFile}
              fileName={fileName}
              fileOwner={fileOwner}
              children={children}
            />
          )}
        </div>
      );
    },
    [filesList]
  );
  return (
    <>
      <AutoSizer>
        {({ width, height }) => (
          <InfiniteLoader
            //ref={listOptionsRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
                className="options_list"
                height={displayType === "aside" ? height : listHeight}
                itemCount={itemCount}
                itemSize={displayType === "aside" ? 56 : 36}
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

      {!hasNextPage && itemCount === 0 && <Text>{emptyFilesList}</Text>}
    </>
  );
};
FileListBody.defaultProps = {
  listHeight: 320,
};
export default inject(({ auth }) => {
  const { user } = auth.userStore;
  return {
    viewer: user,
  };
})(observer(withTranslation("Common")(FileListBody)));
