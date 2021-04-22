import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile } from "react-device-detect";

import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import RowContent from "@appserver/components/row-content";
import IconButton from "@appserver/components/icon-button";
import Badge from "@appserver/components/badge";

import withContentActions from "../hoc/withContentActions";
import {
  StyledFavoriteIcon,
  StyledFileActionsConvertEditDocIcon,
  StyledFileActionsLockedIcon,
} from "../sub-components/icons";

const sideColor = "#A3A9AE";

const SimpleFilesRowContent = styled(RowContent)`
  .badge-ext {
    margin-left: -8px;
    margin-right: 8px;
  }

  .badge {
    height: 14px;
    width: 14px;
    margin-right: 6px;
  }
  .lock-file {
    cursor: pointer;
  }
  .badges {
    display: flex;
    align-items: center;
  }

  .favorite {
    cursor: pointer;
    margin-right: 6px;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
  fileOwner,
  accessToEdit,
  linkStyles,
  newItems,
  showNew,
  canWebEdit,
  /* canConvert,*/
  isTrashFolder,
  onFilesClick,
  onShowVersionHistory,
  onBadgeClick,
  onClickLock,
  onClickFavorite,
  /*setConvertDialogVisible*/
}) => {
  const {
    contentLength,
    fileExst,
    filesCount,
    foldersCount,
    fileStatus,
    id,
    versionGroup,
    locked,
    providerKey,
  } = item;

  const onMobileRowClick = () => {
    if (isTrashFolder || window.innerWidth > 1024) return;
    onFilesClick();
  };

  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        sideColor={sideColor}
        isFile={fileExst || contentLength}
        //onClick={onMobileRowClick}
      >
        <Link
          containerWidth="55%"
          type="page"
          title={titleWithoutExt}
          fontWeight="600"
          fontSize="15px"
          {...linkStyles}
          color="#333"
          isTextOverflow
        >
          {titleWithoutExt}
        </Link>
        <>
          {fileExst ? (
            <div className="badges">
              <Text
                className="badge-ext"
                as="span"
                color="#A3A9AE"
                fontSize="15px"
                fontWeight={600}
                title={fileExst}
                truncate={true}
              >
                {fileExst}
              </Text>
              {/* TODO: Uncomment after fix conversation {canConvert && !isTrashFolder && (
                  <IconButton
                    onClick={setConvertDialogVisible}
                    iconName="FileActionsConvertIcon"
                    className="badge"
                    size="small"
                    isfill={true}
                    color="#A3A9AE"
                    hoverColor="#3B72A7"
                  />
                )} */}
              {canWebEdit && !isTrashFolder && accessToEdit && (
                <IconButton
                  onClick={onFilesClick}
                  iconName="/static/images/access.edit.react.svg"
                  className="badge"
                  size="small"
                  isfill={true}
                  color="#A3A9AE"
                  hoverColor="#3B72A7"
                />
              )}
              {locked && (
                <StyledFileActionsLockedIcon
                  className="badge lock-file"
                  size="small"
                  data-id={item.id}
                  data-locked={true}
                  onClick={onClickLock}
                />
              )}
              {fileStatus === 32 && !isTrashFolder && (
                <StyledFavoriteIcon
                  className="favorite"
                  size="small"
                  data-action="remove"
                  data-id={item.id}
                  data-title={item.title}
                  onClick={onClickFavorite}
                />
              )}
              {fileStatus === 1 && (
                <StyledFileActionsConvertEditDocIcon
                  className="badge"
                  size="small"
                />
              )}
              {versionGroup > 1 && (
                <Badge
                  className="badge-version"
                  backgroundColor="#A3A9AE"
                  borderRadius="11px"
                  color="#FFFFFF"
                  fontSize="10px"
                  fontWeight={800}
                  label={t("Version", {
                    version: versionGroup,
                  })}
                  maxWidth="50px"
                  onClick={onShowVersionHistory}
                  padding="0 5px"
                  data-id={id}
                />
              )}
              {showNew && (
                <Badge
                  className="badge-version"
                  backgroundColor="#ED7309"
                  borderRadius="11px"
                  color="#FFFFFF"
                  fontSize="10px"
                  fontWeight={800}
                  label={t("New")}
                  maxWidth="50px"
                  onClick={onBadgeClick}
                  padding="0 5px"
                  data-id={id}
                />
              )}
            </div>
          ) : (
            <div className="badges">
              {showNew && (
                <Badge
                  className="badge-version"
                  backgroundColor="#ED7309"
                  borderRadius="11px"
                  color="#FFFFFF"
                  fontSize="10px"
                  fontWeight={800}
                  label={newItems}
                  maxWidth="50px"
                  onClick={onBadgeClick}
                  padding="0 5px"
                  data-id={id}
                />
              )}
            </div>
          )}
        </>
        <Text
          containerMinWidth="120px"
          containerWidth="15%"
          as="div"
          color={sideColor}
          fontSize="12px"
          fontWeight={400}
          title={fileOwner}
          truncate={true}
        >
          {fileOwner}
        </Text>
        <Text
          containerMinWidth="200px"
          containerWidth="15%"
          title={updatedDate}
          fontSize="12px"
          fontWeight={400}
          color={sideColor}
          className="row_update-text"
        >
          {(fileExst || contentLength || !providerKey) &&
            updatedDate &&
            updatedDate}
        </Text>
        <Text
          containerMinWidth="90px"
          containerWidth="10%"
          as="div"
          color={sideColor}
          fontSize="12px"
          fontWeight={400}
          title=""
          truncate={true}
        >
          {fileExst || contentLength
            ? contentLength
            : !providerKey
            ? `${t("TitleDocuments")}: ${filesCount} | ${t(
                "TitleSubfolders"
              )}: ${foldersCount}`
            : ""}
        </Text>
      </SimpleFilesRowContent>
    </>
  );
};

export default withRouter(
  withTranslation("Home")(withContentActions(FilesRowContent))
);
