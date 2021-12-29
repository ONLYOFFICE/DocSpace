import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile } from "react-device-detect";
import moment from "moment";

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
    props.sectionWidth <= 1024 &&
    props.sectionWidth > 500 &&
    `
    .row-main-container-wrapper {
      display: flex;
      justify-content: space-between;
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
      margin: 7px 22px 0 0;
    }
  `}
  .edit {
    svg:not(:root) {
      width: 12px;
      height: 12px;
    }
  }
`;

const FilesRowContent = ({
  t,
  item,
  sectionWidth,
  titleWithoutExt,
  updatedDate,
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

  updatedDate = moment(new Date(updatedDate)).format("MM/D/YYYY	hh:mm A");

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
        </Link>
        <div className="badges">{badgesComponent}</div>
        {!!fileExst && (
          <Text
            containerMinWidth="200px"
            containerWidth="15%"
            fontSize="12px"
            fontWeight={400}
            color={sideColor}
            className="row_update-text"
          >
            {updatedDate && updatedDate}
          </Text>
        )}
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
          {!fileExst && !contentLength && !providerKey
            ? `${foldersCount} ${t("folders")} | ${filesCount} ${t("files")}`
            : ""}
        </Text>
      </SimpleFilesRowContent>
    </>
  );
};

export default withRouter(
  withTranslation(["Home", "Translations", "VersionBadge"])(
    withContent(withBadges(FilesRowContent))
  )
);
