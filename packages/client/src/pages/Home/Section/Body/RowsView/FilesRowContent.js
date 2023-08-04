import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import { isMobile, isTablet, isMobileOnly } from "react-device-detect";

import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import RowContent from "@docspace/components/row-content";

import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import { Base } from "@docspace/components/themes";
import { RoomsTypeTranslations } from "@docspace/common/constants";
import { desktop } from "@docspace/components/utils/device";
import { getFileTypeName } from "../../../../../helpers/filesUtils";
import { SortByFieldName } from "../../../../../helpers/constants";

const SimpleFilesRowContent = styled(RowContent)`
  .row-main-container-wrapper {
    width: 100%;
    max-width: min-content;
    min-width: inherit;
    margin-right: 0px;

    @media ${desktop} {
      margin-top: 0px;
    }
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .new-items {
    min-width: 12px;
    width: max-content;
    margin: 0 -2px -2px -2px;
  }

  .badge-version {
    width: max-content;
    margin: -2px 6px -2px -2px;
  }

  .badge-new-version {
    width: max-content;
  }

  ${(props) =>
    ((props.sectionWidth <= 1024 && props.sectionWidth > 500) || isTablet) &&
    css`
      .row-main-container-wrapper {
        display: flex;
        justify-content: space-between;
        max-width: inherit;
      }

      .badges {
        flex-direction: row-reverse;
      }

      .tablet-badge {
        margin-top: 5px;
      }

      .tablet-edit,
      .can-convert {
        margin-top: 6px;
        margin-right: 24px !important;
      }

      .badge-version {
        margin-right: 22px;
      }

      .new-items {
        min-width: 16px;
        margin: 5px 24px 0 0;
      }
    `}

  .row-content-link {
    padding: 12px 12px 0px 0px;
    margin-top: -12px;
  }
`;

SimpleFilesRowContent.defaultProps = { theme: Base };

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
  linkStyles,
  badgesComponent,
  quickButtons,
  theme,
  isRooms,
  isTrashFolder,
  filterSortBy,
  createdDate,
  fileOwner,
}) => {
  const {
    contentLength,
    fileExst,
    filesCount,
    foldersCount,
    providerKey,
    title,
    isRoom,
    daysRemaining,
    fileType,
    tags,
  } = item;

  const contentComponent = () => {
    switch (filterSortBy) {
      case SortByFieldName.Size:
        if (!contentLength) return "—";
        return contentLength;

      case SortByFieldName.CreationDate:
        return createdDate;

      case SortByFieldName.Author:
        return fileOwner;

      case SortByFieldName.Type:
        return getFileTypeName(fileType);

      case SortByFieldName.Tags:
        if (tags?.length === 0) return "—";
        return tags?.map((elem) => {
          return elem;
        });

      default:
        if (isTrashFolder)
          return t("Files:DaysRemaining", {
            daysRemaining,
          });

        return updatedDate;
    }
  };

  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        isFile={fileExst || contentLength}
        sideColor={theme.filesSection.rowView.sideColor}
      >
        <Link
          className="row-content-link"
          containerWidth="55%"
          type="page"
          title={title}
          fontWeight="600"
          fontSize="15px"
          target="_blank"
          {...linkStyles}
          isTextOverflow={true}
        >
          {titleWithoutExt}
        </Link>
        <div className="badges">
          {badgesComponent}
          {!isRoom && !isRooms && quickButtons}
        </div>

        <Text
          containerMinWidth="200px"
          containerWidth="15%"
          fontSize="12px"
          fontWeight={400}
          className="row_update-text"
        >
          {contentComponent()}
        </Text>

        <Text
          containerMinWidth="90px"
          containerWidth="10%"
          as="div"
          className="row-content-text"
          fontSize="12px"
          fontWeight={400}
          truncate={true}
        >
          {isRooms
            ? t(RoomsTypeTranslations[item.roomType])
            : !fileExst && !contentLength && !providerKey && !isMobileOnly
            ? `${foldersCount} ${t("Translations:Folders")} | ${filesCount} ${t(
                "Translations:Files"
              )}`
            : fileExst
            ? `${fileExst.toUpperCase().replace(/^\./, "")}`
            : ""}
        </Text>
      </SimpleFilesRowContent>
    </>
  );
};

export default inject(({ auth, treeFoldersStore, filesStore }) => {
  const { filter, roomsFilter } = filesStore;
  const { isRecycleBinFolder, isRoomsFolder, isArchiveFolder } =
    treeFoldersStore;

  const isRooms = isRoomsFolder || isArchiveFolder;
  const filterSortBy = isRooms ? roomsFilter.sortBy : filter.sortBy;

  return {
    filterSortBy,
    theme: auth.settingsStore.theme,
    isTrashFolder: isRecycleBinFolder,
  };
})(
  observer(
    withTranslation(["Files", "Translations", "Notifications"])(
      withContent(withBadges(FilesRowContent))
    )
  )
);
