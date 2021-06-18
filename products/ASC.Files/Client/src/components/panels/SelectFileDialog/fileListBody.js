import React, { useCallback } from "react";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { useTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import config from "../../../../package.json";
import Checkbox from "@appserver/components/checkbox";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";

const FileListBody = ({
  isLoadingData,
  filesList,
  onSelectFile,
  isModalView,
  loadNextPage,
  hasNextPage,
  isNextPageLoading,
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
            <div
              id={index}
              className="modal-dialog_file-name"
              onClick={onSelectFile}
            >
              <ReactSVG
                src={`${config.homepage}/images/icons/24/file_archive.svg`}
                className="select-file-dialog_icon"
              />
              <div className="entry-title">
                {filesList[index] &&
                  filesList[index].title.substring(
                    0,
                    filesList[index].title.indexOf(".gz")
                  )}
              </div>
              <div className="file-exst">{".gz"}</div>
            </div>
          )}
        </div>
      );
    },
    [filesList]
  );
  return (
    <>
      {!isLoadingData ? (
        <AutoSizer>
          {({ width }) => (
            <InfiniteLoader
              //ref={listOptionsRef}
              isItemLoaded={isItemLoaded}
              itemCount={itemCount}
              loadMoreItems={loadMoreItems}
            >
              {({ onItemsRendered, ref }) => (
                <List
                  className="options_list"
                  height={320}
                  itemCount={itemCount}
                  itemSize={36}
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
      ) : (
        <div key="loader" className="panel-loader-wrapper">
          <Loader type="oval" size="16px" className="panel-loader" />
          <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
            "Common:LoadingDescription"
          )}`}</Text>
        </div>
      )}
    </>
  );
};
FileListBody.defaultProps = {
  isModalView: false,
  isLoadingData: false,
};
export default FileListBody;
