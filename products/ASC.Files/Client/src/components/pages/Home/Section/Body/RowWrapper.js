import React, { memo } from "react";
import { areEqual } from "react-window";
import styled from "styled-components";
import { Row } from "asc-web-components";
import FilesRowContent from "./FilesRowContent";

const SimpleFilesRow = styled(Row)`
  margin-top: -2px;
  ${(props) =>
    !props.contextOptions &&
    `
    & > div:last-child {
        width: 0px;
      }
  `}

  .share-button-icon {
    margin-right: 7px;
    margin-top: -1px;
  }

  .share-button:hover,
  .share-button-icon:hover {
    cursor: pointer;
    color: #657077;
    path {
      fill: #657077;
    }
  }
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media (max-width: 1312px) {
    .share-button {
      padding-top: 3px;
    }
  }

  .styled-element {
    margin-right: 1px;
    margin-bottom: 2px;
  }
`;

const RowWrapper = memo(({ data, index, style }) => {
  const {
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
  } = data;

  const item = items[index];
  const { checked, isFolder, value, contextOptions, canShare } = item;
  const sectionWidth = context.sectionWidth;
  const isEdit =
    !!fileAction.type &&
    editingId === item.id &&
    item.fileExst === fileAction.extension;

  const contextOptionsProps =
    !isEdit && contextOptions && contextOptions.length > 0
      ? {
          contextOptions: getFilesContextOptions(contextOptions, item),
        }
      : {};

  const checkedProps = isEdit || item.id <= 0 ? {} : { checked };

  const element = getItemIcon(item, isEdit || item.id <= 0);

  const sharedButton =
    !canShare ||
    (isPrivacy && !item.fileExst) ||
    isEdit ||
    item.id <= 0 ||
    sectionWidth < 500
      ? null
      : getSharedButton(item.shared);

  const displayShareButton =
    sectionWidth < 500 ? "26px" : !canShare ? "38px" : "96px";

  return (
    <div style={style}>
      <SimpleFilesRow
        sectionWidth={sectionWidth}
        key={item.id}
        data={item}
        element={element}
        contentElement={sharedButton}
        onSelect={onContentRowSelect}
        editing={editingId}
        isPrivacy={isPrivacy}
        {...checkedProps}
        {...contextOptionsProps}
        needForUpdate={needForUpdate}
        selectItem={onSelectItem.bind(this, item)}
        contextButtonSpacerWidth={displayShareButton}
      >
        <FilesRowContent
          sectionWidth={sectionWidth}
          isMobile={isMobile}
          item={item}
          viewer={viewer}
          culture={settings.culture}
          onEditComplete={onEditComplete}
          onMediaFileClick={onMediaFileClick}
          onClickFavorite={onClickFavorite}
          onClickLock={lockFile}
          openDocEditor={openDocEditor}
        />
      </SimpleFilesRow>
    </div>
  );
}, areEqual);

export default RowWrapper;
