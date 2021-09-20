import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile } from "react-device-detect";

import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import RowContent from "@appserver/components/row-content";

import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";

const sideColor = "#A3A9AE";

const SimpleFilesRowContent = styled(RowContent)`
  .badge-ext {
    margin-right: 8px;
  }

  .badge {
    height: 14px;
    width: 14px;
    margin-right: 6px;
  }
  .lock-file {
    cursor: ${(props) => (props.withAccess ? "pointer" : "default")};
  }
  .badges {
    display: flex;
    align-items: center;
    height: 19px;
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
  linkStyles,
  //onFilesClick,
  badgesComponent,
  isAdmin,
}) => {
  const {
    contentLength,
    fileExst,
    filesCount,
    foldersCount,
    providerKey,
    access,
  } = item;

  const withAccess = isAdmin || access === 0;

  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        sideColor={sideColor}
        isFile={fileExst || contentLength}
        withAccess={withAccess}
      >
        <Link
          containerWidth="55%"
          type="page"
          title={titleWithoutExt}
          fontWeight="600"
          fontSize="15px"
          target="_blank"
          href={item.href}
          {...linkStyles}
          color="#333"
          isTextOverflow={true}
        >
          {titleWithoutExt}
          {fileExst && (
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
          )}
        </Link>
        <div className="badges">{badgesComponent}</div>
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
  withTranslation(["Home", "Translations"])(
    withContent(withBadges(FilesRowContent))
  )
);
