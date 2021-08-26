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
import i18n from "./i18n";

import { I18nextProvider } from "react-i18next";

const FilesListBody = ({
  filesList,
  onSelectFile,
  loadNextPage,
  hasNextPage,
  isNextPageLoading,
  displayType,
  viewer,
  listHeight,
  needRowSelection,
  loadingText,
  selectedFolder,
  isMultiSelect,
  selectedFile,
}) => {
  const { t } = useTranslation(["SelectFile", "Common"]);
  const filesListRef = useRef(null);

  useEffect(() => {
    if (filesListRef && filesListRef.current) {
      filesListRef.current.resetloadMoreItemsCache(true);
    }
  }, [selectedFolder, displayType]);
  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < filesList.length;
    },
    [filesList, hasNextPage]
  );
  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? filesList.length + 1 : filesList.length;

  const loadMoreItems = useCallback(() => {
    if (isNextPageLoading) return;
    loadNextPage && loadNextPage();
  }, [isNextPageLoading, filesList, displayType]);

  const renderLoader = useCallback(
    (style) => {
      return (
        <div style={style}>
          <div key="loader" className="panel-loader-wrapper">
            <Loader type="oval" size="16px" className="panel-loader" />
            <Text as="span">{loadingText}</Text>
          </div>
        </div>
      );
    },
    [loadingText]
  );
  const isFileChecked = useCallback(
    (file) => {
      const checked = selectedFile ? file.id === selectedFile.id : false;
      return checked;
    },
    [selectedFile]
  );
  const Item = useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        return renderLoader(style);
      }

      const file = filesList[index];
      const fileName = file.title;
      const fileExst = file.fileExst;
      const modifyFileName = fileName.substring(
        0,
        fileName.indexOf(`${fileExst}`)
      );

      const fileOwner =
        file.createdBy &&
        ((viewer.id === file.createdBy.id && t("Common:MeLabel")) ||
          file.createdBy.displayName);

      const isChecked = isFileChecked(file);

      return (
        <div style={style}>
          <FilesListRow
            displayType={displayType}
            needRowSelection={needRowSelection}
            index={index}
            onSelectFile={onSelectFile}
            fileName={modifyFileName}
            fileExst={fileExst}
            isMultiSelect={isMultiSelect}
            isChecked={isChecked}
          >
            <Text data-index={index} className="files-list_file-owner">
              {fileOwner}
            </Text>
          </FilesListRow>
        </div>
      );
    },
    [filesList, selectedFile, displayType, renderLoader]
  );
  return (
    <>
      <AutoSizer>
        {({ width, height }) => (
          <InfiniteLoader
            ref={filesListRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <List
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

      {!hasNextPage && itemCount === 0 && (
        <div className="select-file-dialog_empty-container">
          <EmptyContainer
            headerText={t("Home:EmptyFolderHeader")}
            imageSrc="/static/images/empty_screen.png"
          />
        </div>
      )}
    </>
  );
};
FilesListBody.defaultProps = {
  listHeight: 300,
  isMultiSelect: false,
};
const FilesListBodyWrapper = inject(({ auth }) => {
  const { user } = auth.userStore;
  return {
    viewer: user,
  };
})(observer(withTranslation(["Common", "Home"])(FilesListBody)));

class FilesList extends React.Component {
  render() {
    return (
      <I18nextProvider i18n={i18n}>
        <FilesListBodyWrapper {...this.props} />
      </I18nextProvider>
    );
  }
}

export default FilesList;
