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
  .row-main-container-wrapper {
    width: 100%;
  }

  .badge-ext {
    // margin-right: 8px;
  }

  .row_update-text {
    overflow: hidden;
    text-overflow: ellipsis;
  }

  ${(props) =>
    props.sectionWidth < 1025 &&
    props.sectionWidth > 500 &&
    `
    .row-main-container-wrapper {
      display: flex;
      justify-content: space-between;
    }
  `}
`;

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
  fileOwner,
  linkStyles,
  badgesComponent,
}) => {
  const {
    contentLength,
    fileExst,
    filesCount,
    foldersCount,
    providerKey,
    title,
  } = item;

  return (
    <>
      <SimpleFilesRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        sideColor={sideColor}
        isFile={fileExst || contentLength}
      >
        <Link
          containerWidth="55%"
          type="page"
          title={title}
          fontWeight="600"
          fontSize="15px"
          target="_blank"
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
          {updatedDate && updatedDate}
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
