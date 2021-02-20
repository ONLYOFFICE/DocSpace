import React from "react";
import { connect } from "react-redux";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import memoize from "memoize-one";

import { loadMoreFiles } from "../../../../../store/files/actions";
import RowWrapper from "./RowWrapper";

const FileList = ({
  items,
  context,
  fileAction,
  editingId,
  getFilesContextOptions,
  getItemIcon,
  isPrivacy,
  getSharedButton,
  onContentRowSelect,
  needForUpdate,
  onSelectItem,
  isMobile,
  viewer,
  settings,
  onEditComplete,
  onMediaFileClick,
  onClickFavorite,
  lockFile,
  openDocEditor,
  filter,
  loadMoreFiles,
}) => {
  const createItemData = memoize(
    (
      items,
      context,
      fileAction,
      editingId,
      getFilesContextOptions,
      getItemIcon,
      isPrivacy,
      getSharedButton,
      onContentRowSelect,
      needForUpdate,
      onSelectItem,
      isMobile,
      viewer,
      settings,
      onEditComplete,
      onMediaFileClick,
      onClickFavorite,
      lockFile,
      openDocEditor
    ) => ({
      items,
      context,
      fileAction,
      editingId,
      getFilesContextOptions,
      getItemIcon,
      isPrivacy,
      getSharedButton,
      onContentRowSelect,
      needForUpdate,
      onSelectItem,
      isMobile,
      viewer,
      settings,
      onEditComplete,
      onMediaFileClick,
      onClickFavorite,
      lockFile,
      openDocEditor,
    })
  );

  const itemData = createItemData(
    items,
    context,
    fileAction,
    editingId,
    getFilesContextOptions,
    getItemIcon,
    isPrivacy,
    getSharedButton,
    onContentRowSelect,
    needForUpdate,
    onSelectItem,
    isMobile,
    viewer,
    settings,
    onEditComplete,
    onMediaFileClick,
    onClickFavorite,
    lockFile,
    openDocEditor
  );

  const folderId = filter.folder;

  const isItemLoaded = (index) => !!items[index];

  const loadMoreItems = () => {
    loadMoreFiles(folderId, filter);
  };

  return (
    <AutoSizer>
      {({ height, width, style }) => (
        <InfiniteLoader
          isItemLoaded={isItemLoaded}
          itemCount={filter.total}
          loadMoreItems={loadMoreItems}
        >
          {({ onItemsRendered, ref }) => (
            <List
              className="hide-scrollbars"
              style={style}
              height={height}
              width={width}
              itemData={itemData}
              itemCount={items.length}
              itemSize={itemData.isMobile ? 57 : 48}
              onItemsRendered={onItemsRendered}
              ref={ref}
            >
              {RowWrapper}
            </List>
          )}
        </InfiniteLoader>
      )}
    </AutoSizer>
  );
};

const mapStateToProps = (state) => {
  const { filter } = state.files;

  return {
    filter,
  };
};

export default connect(mapStateToProps, {
  loadMoreFiles,
})(FileList);
