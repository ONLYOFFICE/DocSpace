import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import Badge from "@appserver/components/badge";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

import TileContent from "./TileContent";
import withContentActions from "../hoc/withContentActions";

const SimpleFilesTileContent = styled(TileContent)`
  .rowMainContainer {
    height: auto;
    max-width: 100%;
    align-self: flex-end;

    a {
      word-break: break-word;
    }
  }

  .mainIcons {
    align-self: flex-end;
  }

  .badge-ext {
    margin-left: -8px;
    margin-right: 8px;
  }

  .badge {
    margin-right: 8px;
  }

  .badges {
    display: flex;
    align-items: center;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  @media (max-width: 1024px) {
    display: inline-flex;
    height: auto;

    & > div {
      margin-top: 0;
    }
  }
`;

const FilesTileContent = ({
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
      <SimpleFilesTileContent
        sideColor="#333"
        isFile={fileExst}
        //onClick={onMobileRowClick}
        //disableSideInfo
      >
        <Link
          containerWidth="100%"
          type="page"
          title={titleWithoutExt}
          fontWeight="bold"
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
            </div>
          ) : (
            <div className="badges">
              {!!showNew && (
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
      </SimpleFilesTileContent>
    </>
  );
};

export default withRouter(
  withTranslation("Home")(withContentActions(FilesTileContent))
);
